using Infrastructure;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Services.Mapping;
using EventManagment.Extension;
using Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Infrastructure.DbExecute;
using Services.Role;
using Hangfire;
using Services.SendEmail;
using EventManagment.Hubs;
using Services.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNet.SignalR.Hosting;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<EventManagmentDb>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("EventManagment")
    ));

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("EventManagment")));

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
var jwtSecretKey = builder.Configuration.GetSection("JwtSettings:SecretKey").Value;

builder.Services.AddScoped<IDbInitialize, DbInitialize>();
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddSingleton<IEmailService, SmtpEmailService>();
builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = true;
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    string redisConnectionString = builder.Configuration.GetConnectionString("Redis");

    options.Configuration = redisConnectionString;
    options.InstanceName = "leonard";   
});

#region
// Configure AutoMapper
var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfiles());
});

var mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
#endregion

#region Culture Localization
builder.Services.AddLocalization(opts => opts.ResourcesPath = "Resources");
builder.Services.AddMvc()
        .AddViewLocalization()
        .AddDataAnnotationsLocalization();
#endregion

builder.Services.RegisterBusinessLayerDependencies();
builder.Services.RegisterDataAccessLayerDependencies();

#region DataProtection
builder.Services.AddDataProtection();
builder.Services.AddSingleton<DataProtectionPurposeStrings>();
#endregion


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/AccessDenied";
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecretKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(5)
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireRole("Admin");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
SeedDatabase();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/notificationHub");
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});


app.UseHangfireDashboard();
//Add's The Schedule To HangFireServer
RecurringJob.AddOrUpdate<IDbInitialize>(x => x.DeleteExpiredEvents(), Cron.Hourly);
RecurringJob.AddOrUpdate<IDbInitialize>(x => x.UpdateTicketAvailability(), Cron.Minutely);
RecurringJob.AddOrUpdate<IDbInitialize>(x => x.CheckAndUpdateExpiredReservation(), Cron.Minutely);
//Executes the Background Schedule
app.UseHangfireServer();


app.Run();




void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitialize = scope.ServiceProvider.GetRequiredService<IDbInitialize>();
        dbInitialize.DbExecute();
        dbInitialize.CreateAdmin().GetAwaiter().GetResult();
    }
}


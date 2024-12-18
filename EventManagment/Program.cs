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
using dotenv.net;
using System.Security.Claims;
using static System.Formats.Asn1.AsnWriter;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
DotEnv.Load();

builder.Services.AddControllersWithViews();

// Get connection string from environment variables
var connectionString = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__EVENTMANAGEMENT");

// Configure Entity Framework with SQL Server
builder.Services.AddDbContext<EventManagmentDb>(options => options
    .UseSqlServer(connectionString)
    .EnableSensitiveDataLogging()
    .LogTo(Console.WriteLine));

// Configure Hangfire
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(connectionString));
// Register Hangfire server
builder.Services.AddHangfireServer(); 

var stripeSettings = new StripeSettings();
builder.Services.AddSingleton(stripeSettings);

builder.Services.AddScoped<IDbInitialize, DbInitialize>();
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddSingleton<IEmailService, SmtpEmailService>();
builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = true;
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    string redisConnectionString = Environment.GetEnvironmentVariable("REDIS");

    options.Configuration = redisConnectionString;
    options.InstanceName = "EventManagement";   
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

var jwtSecretKey = Environment.GetEnvironmentVariable("JWTSETTINGS__SECRETKEY");

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.SlidingExpiration = true;

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
    foreach(var permission in Enum.GetValues(typeof(PermissionType)))
    {
        options.AddPolicy(permission.ToString(),policy =>
        {
            policy.RequireClaim("Permission",permission.ToString());
        });
    }
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

app.Use(async (context, next) =>
{
    if (context.User.Identity.IsAuthenticated)
    {
        using (var scope = app.Services.CreateScope())
        {
            var _roleService = scope.ServiceProvider.GetRequiredService<IRoleService>();
            var claim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value !=null ? int.Parse(claim.Value) : 0;
            var roles = await _roleService.GetRolesForUserAsync(userId);
            foreach (var role in roles)
            {
                var permissions = await _roleService.GetPermissionsForRoleAsync(role.Id);
                foreach (var permission in permissions)
                {
                    ((ClaimsIdentity)context.User.Identity).AddClaim(new Claim("Permission", permission.Name));
                }
            }
        }
    }
    await next();
});


app.UseHangfireDashboard();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/notificationHub");
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});


//Add's The Schedule To HangFireServer
RecurringJob.AddOrUpdate<IDbInitialize>(
    recurringJobId: "DeleteExpiredEvents",
    methodCall: x => x.DeleteExpiredEvents(),
    cronExpression: Cron.Hourly,
    options: new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.Local,
    }
);
RecurringJob.AddOrUpdate<IDbInitialize>(
    recurringJobId: "UpdateTicketAvailability",
    methodCall: x => x.UpdateTicketAvailability(),
    cronExpression: Cron.Minutely,
    options: new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.Local,
    }
);

RecurringJob.AddOrUpdate<IDbInitialize>(
    recurringJobId: "CheckAndUpdateExpiredReservation", 
    methodCall: x => x.CheckAndUpdateExpiredReservation(),
    cronExpression: Cron.Minutely,
    options: new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.Local,
    }
);


app.Run();


void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitialize = scope.ServiceProvider.GetRequiredService<IDbInitialize>();
        dbInitialize.DbExecute().GetAwaiter().GetResult();
        dbInitialize.CreateAdmin().GetAwaiter().GetResult();
    }
}


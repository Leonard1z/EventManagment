using Infrastructure.Repositories;
using Services.Common;
using System.Reflection;

namespace EventManagment.Extension
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Ben regjistrimin e Dependency Injection ne Business Layer permes Reflection
        /// </summary>
        /// <param name="services"> Interface </param>
        public static void RegisterBusinessLayerDependencies(this IServiceCollection services)
        {
            var serviceInterfaceType = typeof(IService);

            var types = serviceInterfaceType
                     .Assembly
                     .GetExportedTypes()
                     .Where(t => t.IsClass && !t.IsAbstract)
                     .Select(t => new
                     {
                         Service = t.GetInterface($"I{t.Name}"),
                         Implementation = t
                     })
                     .Where(t => t.Service != null);

            foreach (var type in types)
            {
                services.AddTransient(type.Service, type.Implementation);
            }


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        }

        /// <summary>
        /// Ben regjistrimin e Dependency Injection ne Data Acces Layer permes Reflection
        /// </summary>
        /// <param name="services"> Interface </param>
        public static void RegisterDataAccessLayerDependencies(this IServiceCollection services)

        {
            var repositoryInterfaceType = typeof(IGenericRepository<>);

            var types = repositoryInterfaceType
                     .Assembly
                     .GetExportedTypes()
                     .Where(x => !x.Name.Contains("GenericRepository"))
                     .Select(t => new
                     {
                         Service = t.GetInterface($"I{t.Name}"),
                         Implementation = t
                     })
                     .Where(t => t.Service != null);

            foreach (var type in types)
            {
                services.AddTransient(type.Service, type.Implementation);
            }
        }

    }
}

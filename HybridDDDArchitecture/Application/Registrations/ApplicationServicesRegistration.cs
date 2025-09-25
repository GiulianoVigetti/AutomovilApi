using HybridDDDArchitecture.Core.Application.ApplicationServices;
using Microsoft.Extensions.DependencyInjection;

namespace HybridDDDArchitecture.Core.Application.Registrations
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Application Services
            services.AddScoped<IAutomovilService, AutomovilService>();

            return services;
        }
    }
}
/*using HybridDDDArchitecture.Core.Application.ApplicationServices;
using HybridDDDArchitecture.Core.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;

namespace HybridDDDArchitecture.Core.Application.Registrations
{
    /// <summary>
    /// Aquí se deben registrar todas las dependencias de la capa de aplicación
    /// </summary>
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Automapper 
            services.AddAutoMapper(typeof(AutomovilMappingProfile));

            // Application Services
            services.AddScoped<IAutomovilService, AutomovilService>();

            return services;
        }
    }
}*/

/*using HybridDDDArchitecture.Core.Application.ApplicationServices;
using Microsoft.Extensions.DependencyInjection;

namespace HybridDDDArchitecture.Core.Application.Registrations
{
    /// <summary>
    /// Aquí se deben registrar todas las dependencias de la capa de aplicación
    /// </summary>
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Automapper - Usar el que ya está configurado en Core.Application.Mapping 
            // services.AddAutoMapper(...); // Lo comentamos por ahora

            // Application Services 
            services.AddScoped<IAutomovilService, AutomovilService>();

            return services;
        }
    }
}*/

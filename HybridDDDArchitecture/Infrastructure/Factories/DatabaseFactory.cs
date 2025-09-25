using HybridDDDArchitecture.Core.Application.Repositories;
using Domain.Others.Utils;
using Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HybridDDDArchitecture.Core.Infraestructure.Repositories.Sql;
using static Domain.Enums.Enums;

namespace Infrastructure.Factories
{
    internal static class DatabaseFactory
    {
        public static void CreateDataBase(this IServiceCollection services, string dbType, IConfiguration configuration)
        {
            switch (dbType.ToEnum<DatabaseType>())
            {
                case DatabaseType.MYSQL:
                case DatabaseType.MARIADB:
                case DatabaseType.SQLSERVER:
                    services.AddSqlServerRepositories(configuration);
                    break;
                case DatabaseType.MONGODB:
                    throw new NotSupportedException("MongoDB no está configurado para este proyecto");
                default:
                    throw new NotSupportedException(InfrastructureConstants.DATABASE_TYPE_NOT_SUPPORTED);
            }
        }

        private static IServiceCollection AddSqlServerRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            // Registrar el contexto de Automovil
            services.AddDbContext<AutomovilDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("SqlConnection"));
            }, ServiceLifetime.Scoped);

            // Habilitar para trabajar con Migrations
            var serviceProvider = services.BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<AutomovilDbContext>();
            context.Database.Migrate();

            // Registrar repositorios
            services.AddTransient<IAutomovilRepository, AutomovilRepository>();

            return services;
        }
    }
}
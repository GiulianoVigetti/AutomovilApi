using HybridDDDArchitecture.Core.Application.Registrations;
using Infrastructure.Registrations;
using AutoMapper;
using Core.Application;
using Filters;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;

namespace API
{
    public class Startup
    {
        public IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddApplicationServices();
            services.AddInfrastructureServices(Configuration);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Automovil API", Version = "v1" });
            });

            services.AddMvc().AddMvcOptions(options =>
            {
                options.Filters.Add<BaseExceptionFilter>();
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                );
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Configura Swagger para que funcione en cualquier entorno
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Automovil API V1");
                c.RoutePrefix = "swagger";
            });

            app.UseRouting();
            app.UseCors("AllowSpecificOrigin");
            app.UseAuthentication();
            app.UseAuthorization();

            // Este es el último paso que mapea los controladores
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
/*using HybridDDDArchitecture.Core.Application.Registrations;
using Infrastructure.Registrations;
using AutoMapper;
using Core.Application;
using Filters;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;

namespace API
{
    public class Startup
    {
        public IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddApplicationServices();
            services.AddInfrastructureServices(Configuration);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hybrid Architecture Project", Version = "v1" });
            });

            services.AddMvc().AddMvcOptions(options =>
            {
                options.Filters.Add<BaseExceptionFilter>();
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                );
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Nota: Es mejor dejar el uso de DeveloperExceptionPage solo en desarrollo
            app.UseDeveloperExceptionPage();

            // Configura Swagger para que funcione en cualquier entorno
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                // Asegura que la UI de Swagger esté en la ruta raíz.
                // Si la URL es http://localhost:5000, la UI estará en http://localhost:5000/swagger
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hybrid Architecture Project V1");
                c.RoutePrefix = "swagger"; // La ruta que aparecerá en la URL
            });

            // Comentar temporalmente el CustomMapper hasta configurar AutoMapper correctamente
            // CustomMapper.Instance = app.ApplicationServices.GetRequiredService<IMapper>();

            app.UseRouting();
            app.UseCors("AllowSpecificOrigin");
            app.UseAuthentication();
            app.UseAuthorization();

            // Comentar el EventBus temporalmente
            // UseEventBus(app);

            // Este es el último paso que mapea los controladores
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        // Comentado temporalmente hasta configurar el EventBus
        /*
        private void UseEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<DummyEntityCreatedIntegrationEvent, DummyEntityCreatedIntegrationEventHandlerSub>();
        }
        */
/* }
}*/
/*using Application;
using Application.Registrations;
using AutoMapper;
using Core.Application;
using Filters;
using Infrastructure.Registrations;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder; 

namespace API
{
    public class Startup
    {
        public IConfiguration Configuration;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddApplicationServices();
            services.AddInfrastructureServices(Configuration);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hybrid Architecture Project", Version = "v1" });
            });
            services.AddMvc().AddMvcOptions(options =>
            {
                options.Filters.Add<BaseExceptionFilter>();
            });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                );
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Nota: Es mejor dejar el uso de DeveloperExceptionPage solo en desarrollo
            app.UseDeveloperExceptionPage();

            // Configura Swagger para que funcione en cualquier entorno
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                // Asegura que la UI de Swagger esté en la ruta raíz.
                // Si la URL es http://localhost:5000, la UI estará en http://localhost:5000/swagger
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hybrid Architecture Project V1");
                c.RoutePrefix = "swagger"; // La ruta que aparecerá en la URL
            });

            CustomMapper.Instance = app.ApplicationServices.GetRequiredService<IMapper>();

            // Si el problema persiste, revisa si el middleware de enrutamiento
            // está configurado correctamente. El orden es crucial.

            // app.UseHttpsRedirection(); // Ya lo comentaste, lo que es correcto.

            app.UseRouting();

            app.UseCors("AllowSpecificOrigin");

            app.UseAuthentication();
            app.UseAuthorization();

            UseEventBus(app);

            // Este es el último paso que mapea los controladores
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private void UseEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<DummyEntityCreatedIntegrationEvent, DummyEntityCreatedIntegrationEventHandlerSub>();
        }
    }
}*/
/*using Application;
using Application.Registrations;
using AutoMapper;
using Core.Application;
using Filters;
using Infrastructure.Registrations;
using Microsoft.OpenApi.Models;

namespace API
{
    public class Startup
    {
        public IConfiguration Configuration;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddApplicationServices();
            services.AddInfrastructureServices(Configuration);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hybrid Architecture Project", Version = "v1" });
            });
            services.AddMvc().AddMvcOptions(options =>
            {
                options.Filters.Add<BaseExceptionFilter>();
            });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                );
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
           // if (env.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI();
            //}

            CustomMapper.Instance = app.ApplicationServices.GetRequiredService<IMapper>();

            //app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowSpecificOrigin");
            app.UseAuthentication();
            app.UseAuthorization();
            UseEventBus(app);
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private void UseEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            // Aqui se registran las subscripciones a eventos del bus de eventos, vinculando
            //eventos con sus respectivos handlers
            eventBus.Subscribe<DummyEntityCreatedIntegrationEvent, DummyEntityCreatedIntegrationEventHandlerSub>();
        }
    }
}*/

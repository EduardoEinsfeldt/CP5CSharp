using Loggu.Infraestructure.Context;
using Loggu.Domain.Repositories;
using Loggu.Infrastructure.Repositories;
using Loggu.Infraestructure.Mapping;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;


using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Loggu
{

    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var desc in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(desc.GroupName, new OpenApiInfo
                {
                    Title = "Loggu API",
                    Version = desc.ApiVersion.ToString(),
                    Description = "CP5 - MongoDB + Health Check + Swagger"
                });
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            var mongo = builder.Configuration.GetSection("Mongo");
            var cs = mongo.GetValue<string>("ConnectionString")
                ?? throw new InvalidOperationException("Mongo:ConnectionString não configurado.");
            var db = mongo.GetValue<string>("DatabaseName")
                ?? throw new InvalidOperationException("Mongo:DatabaseName não configurado.");


            builder.Services.AddSingleton(new LogguContext(cs, db));


            builder.Services.AddScoped<IMotoRepository, MotoRepository>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<IMovimentoPatioRepository, MovimentoPatioRepository>();

            builder.Services.AddScoped<ICheckRepository, CheckRepository>();
            builder.Services.AddScoped<IOcorrenciaRepository, OcorrenciaRepository>();


            builder.Services.AddHealthChecks().AddMongoDb(sp => new MongoClient(cs), name: "mongodb");

            builder.Services.AddControllers();
            

            builder.Services.AddApiVersioning(o =>
            {
                o.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.ReportApiVersions = true;
                o.ApiVersionReader = new UrlSegmentApiVersionReader();
            });
            

            builder.Services.AddVersionedApiExplorer(o =>
            {
                o.GroupNameFormat = "'v'VVV";
                o.SubstituteApiVersionInUrl = true;
            });


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            builder.Services.AddSwaggerGen(o => o.OperationFilter<SwaggerDefaultValues>());

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<LogguContext>();
                MotoMapping.Ensure(ctx);
                UsuarioMapping.Ensure(ctx);
                MovimentoPatioMapping.Ensure(ctx);

                CheckMapping.Ensure(ctx);
                OcorrenciaMapping.Ensure(ctx);
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var desc in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", desc.GroupName.ToUpperInvariant());
                }
                c.RoutePrefix = "swagger";
            });

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapHealthChecks("/health");
            app.MapControllers();

            app.Run();
        }
    }

    public class SwaggerDefaultValues : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;
            operation.Deprecated |= apiDescription.IsDeprecated();

            if (operation.Parameters == null) return;

            foreach (var parameter in operation.Parameters)
            {
                var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);
                if (parameter.Description == null)
                {
                    parameter.Description = description.ModelMetadata?.Description;
                }
                parameter.Required |= description.IsRequired;
            }
        }
    }
}
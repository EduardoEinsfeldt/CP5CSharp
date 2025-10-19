using Loggu.Infraestructure.Context;
using Loggu.Domain.Repositories;
using Loggu.Infrastructure.Repositories;
using Loggu.Infraestructure.Mapping;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

namespace Loggu
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ===== MongoDB (Config + DI) =====
            // appsettings.json:
            // "Mongo": { "ConnectionString": "mongodb://localhost:27017", "DatabaseName": "loggu_cp5" }
            var mongo = builder.Configuration.GetSection("Mongo");
            var cs = mongo.GetValue<string>("ConnectionString")
                ?? throw new InvalidOperationException("Mongo:ConnectionString não configurado.");
            var db = mongo.GetValue<string>("DatabaseName")
                ?? throw new InvalidOperationException("Mongo:DatabaseName não configurado.");

            // Contexto (Mongo)
            builder.Services.AddSingleton(new LogguContext(cs, db));

            // Repositórios
            builder.Services.AddScoped<IMotoRepository, MotoRepository>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<IMovimentoPatioRepository, MovimentoPatioRepository>();

            // HealthCheck (Mongo)
            builder.Services.AddHealthChecks().AddMongoDb(sp => new MongoClient(cs), name: "mongodb");

            // ===== API / Swagger =====
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Loggu API",
                    Version = "v1",
                    Description = "CP5 - MongoDB + Health Check + Swagger"
                });
            });

            var app = builder.Build();

            // Garantir coleções, validators e índices
            using (var scope = app.Services.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<LogguContext>();
                MotoMapping.Ensure(ctx);
                UsuarioMapping.Ensure(ctx);
                MovimentoPatioMapping.Ensure(ctx);
            }

            // ===== Pipeline =====
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Loggu API v1");
                c.RoutePrefix = "swagger";
            });

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapHealthChecks("/health");
            app.MapControllers();

            app.Run();
        }
    }
}

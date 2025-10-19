using Loggu.Domain.Entity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Loggu.Infraestructure.Context
{
    public class LogguContext
    {
        public IMongoDatabase Database { get; }

        public LogguContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            Database = client.GetDatabase(databaseName);
        }

        // Coleções (equivalentes aos DbSet<>)
        public IMongoCollection<Moto> Motos => Database.GetCollection<Moto>("motos");
        public IMongoCollection<Usuario> Usuarios => Database.GetCollection<Usuario>("usuarios");
        public IMongoCollection<MovimentoPatio> MovimentosPatio => Database.GetCollection<MovimentoPatio>("movimentos_patio");
        public IMongoCollection<Check> Checks => Database.GetCollection<Check>("checks");
        public IMongoCollection<Ocorrencia> Eventos => Database.GetCollection<Ocorrencia>("eventos");

        // Contadores para IDs incrementais (usados pelos repositórios)
        public IMongoCollection<BsonDocument> Counters => Database.GetCollection<BsonDocument>("_counters");

        // Helper genérico (útil para futuros agregados)
        public IMongoCollection<T> GetCollection<T>(string name) => Database.GetCollection<T>(name);
    }
}

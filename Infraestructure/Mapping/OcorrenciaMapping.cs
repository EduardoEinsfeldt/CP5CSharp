using Loggu.Domain.Entity;
using Loggu.Infraestructure.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Loggu.Infraestructure.Mapping
{
    public static class OcorrenciaMapping
    {
        private const string CollectionName = "ocorrencias";

        public static void Ensure(LogguContext ctx)
        {
            var db = ctx.Database;
            var validator = new BsonDocument {
                { "$jsonSchema", new BsonDocument {
                    { "bsonType", "object" },
                   
                    { "required", new BsonArray { "_id", "MotoId", "Categoria", "AcontecidoEm" } },
                    { "properties", new BsonDocument {
                        { "_id", new BsonDocument { { "bsonType", "int" } } },
                        { "MotoId", new BsonDocument { { "bsonType", "int" } } },
                        { "Categoria", new BsonDocument { { "bsonType", "string" }, { "maxLength", 40 } } },
                        { "AcontecidoEm", new BsonDocument { { "bsonType", "date" } } },
                        { "Descricao", new BsonDocument { { "bsonType", new BsonArray { "string", "null" } }, { "maxLength", 500 } } }
                    }}
                }}
            };

            var collections = db.ListCollectionNames().ToList();
            if (!collections.Contains(CollectionName))
                db.CreateCollection(CollectionName, new CreateCollectionOptions<BsonDocument> { Validator = new BsonDocumentFilterDefinition<BsonDocument>(validator) });
            else
                db.RunCommand<BsonDocument>(new BsonDocument { { "collMod", CollectionName }, { "validator", validator }, { "validationLevel", "moderate" } });

            var col = db.GetCollection<Ocorrencia>(CollectionName);
            var indexes = new List<CreateIndexModel<Ocorrencia>>
            {
       
                new(Builders<Ocorrencia>.IndexKeys.Ascending(x => x.MotoId).Descending(x => x.AcontecidoEm), new CreateIndexOptions { Name = "ix_ocorrencia_motoid_acontecidoem" })
            };
            col.Indexes.CreateMany(indexes);
        }
    }
}
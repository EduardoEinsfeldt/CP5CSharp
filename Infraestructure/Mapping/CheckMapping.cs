using Loggu.Domain.Entity;
using Loggu.Infraestructure.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Loggu.Infraestructure.Mapping
{
    public static class CheckMapping
    {
        private const string CollectionName = "checks";

        public static void Ensure(LogguContext ctx)
        {
            var db = ctx.Database;
            var validator = new BsonDocument {
                { "$jsonSchema", new BsonDocument {
                    { "bsonType", "object" },
                    // Required fields from Check.cs
                    { "required", new BsonArray { "Id", "MotoId", "PneuOk", "FreioOk", "LuzesOk", "DocumentosOk", "AptaParaUso", "RealizadoEm" } },
                    { "properties", new BsonDocument {
                        { "Id", new BsonDocument { { "bsonType", "int" } } },
                        { "MotoId", new BsonDocument { { "bsonType", "int" } } },
                        { "RealizadoPorUsuarioId", new BsonDocument { { "bsonType", new BsonArray { "int", "null" } } } },
                        { "Quilometragem", new BsonDocument { { "bsonType", new BsonArray { "int", "null" } } } },
                        // Range(0, 1) validation
                        { "PneuOk", new BsonDocument { { "bsonType", "int" }, { "minimum", 0 }, { "maximum", 1 } } },
                        { "FreioOk", new BsonDocument { { "bsonType", "int" }, { "minimum", 0 }, { "maximum", 1 } } },
                        { "LuzesOk", new BsonDocument { { "bsonType", "int" }, { "minimum", 0 }, { "maximum", 1 } } },
                        { "DocumentosOk", new BsonDocument { { "bsonType", "int" }, { "minimum", 0 }, { "maximum", 1 } } },
                        { "AptaParaUso", new BsonDocument { { "bsonType", "int" }, { "minimum", 0 }, { "maximum", 1 } } },
                        { "Observacao", new BsonDocument { { "bsonType", new BsonArray { "string", "null" } }, { "maxLength", 500 } } },
                        { "RealizadoEm", new BsonDocument { { "bsonType", "date" } } }
                    }}
                }}
            };
            
            var collections = db.ListCollectionNames().ToList();
            if (!collections.Contains(CollectionName))
                db.CreateCollection(CollectionName, new CreateCollectionOptions<BsonDocument> { Validator = new BsonDocumentFilterDefinition<BsonDocument>(validator) });
            else
                db.RunCommand<BsonDocument>(new BsonDocument { { "collMod", CollectionName }, { "validator", validator }, { "validationLevel", "moderate" } });

            var col = db.GetCollection<Check>(CollectionName);
            var indexes = new List<CreateIndexModel<Check>>
            {
                new(Builders<Check>.IndexKeys.Ascending(x => x.Id), new CreateIndexOptions { Name = "pk_check_id", Unique = true }),
                new(Builders<Check>.IndexKeys.Ascending(x => x.MotoId).Descending(x => x.RealizadoEm), new CreateIndexOptions { Name = "ix_check_motoid_realizadoem" })
            };
            col.Indexes.CreateMany(indexes);
        }
    }
}
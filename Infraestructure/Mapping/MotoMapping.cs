using Loggu.Domain.Entity;
using Loggu.Infraestructure.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Loggu.Infraestructure.Mapping
{
    public static class MotoMapping
    {
        private const string CollectionName = "motos";

        public static void Ensure(LogguContext ctx)
        {
            var db = ctx.Database;

            var validator = new BsonDocument
            {
                {
                    "$jsonSchema", new BsonDocument
                    {
                        { "bsonType", "object" },
                        { "required", new BsonArray { "Id", "Placa", "Status", "EmPatio" } },
                        {
                            "properties", new BsonDocument
                            {
                                { "Id", new BsonDocument { { "bsonType", "int" } } },
                                {
                                    "Placa", new BsonDocument
                                    {
                                        { "bsonType", "string" },
                                        { "minLength", 7 }, { "maxLength", 7 },
                                        { "pattern", "^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$" }
                                    }
                                },
                                {
                                    "Chassi", new BsonDocument
                                    {
                                        { "bsonType", new BsonArray { "string", "null" } },
                                        { "maxLength", 17 },
                                        { "pattern", "^[A-HJ-NPR-Z0-9]{17}$" }
                                    }
                                },
                                { "Status", new BsonDocument { { "bsonType", "int" } } },
                                {
                                    "EmPatio", new BsonDocument
                                    {
                                        { "bsonType", "int" }, { "minimum", 0 }, { "maximum", 1 }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var collections = db.ListCollectionNames().ToList();
            if (!collections.Contains(CollectionName))
            {
                db.CreateCollection(
                    CollectionName,
                    new CreateCollectionOptions<BsonDocument>
                    {
                        Validator = new BsonDocumentFilterDefinition<BsonDocument>(validator)
                    }
                );
            }
            else
            {
                var cmd = new BsonDocument
                {
                    { "collMod", CollectionName },
                    { "validator", validator },
                    { "validationLevel", "moderate" }
                };
                db.RunCommand<BsonDocument>(cmd);
            }

            var col = db.GetCollection<Moto>(CollectionName);
            var indexes = new List<CreateIndexModel<Moto>>
            {
                new(
                    Builders<Moto>.IndexKeys.Ascending(m => m.Id),
                    new CreateIndexOptions { Name = "pk_moto_id", Unique = true }
                ),
                new(
                    Builders<Moto>.IndexKeys.Ascending(m => m.Placa),
                    new CreateIndexOptions { Name = "uk_moto_placa", Unique = true }
                )
            };
            col.Indexes.CreateMany(indexes);
        }
    }
}

using Loggu.Domain.Entity;
using Loggu.Infraestructure.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Loggu.Infraestructure.Mapping
{
    public static class MovimentoPatioMapping
    {
        private const string CollectionName = "movimentos_patio";

        public static void Ensure(LogguContext ctx)
        {
            var db = ctx.Database;


            var validator = new BsonDocument
            {
                {
                    "$jsonSchema", new BsonDocument
                    {
                        { "bsonType", "object" },
                        { "required", new BsonArray { "_id", "MotoId", "Tipo", "Quando" } },
                        {
                            "properties", new BsonDocument
                            {
                                { "_id", new BsonDocument { { "bsonType", "int" } } },
                                { "MotoId", new BsonDocument { { "bsonType", "int" } } },
                                { "RealizadoPorUsuarioId", new BsonDocument { { "bsonType", new BsonArray { "int", "null" } } } },
                                { "Tipo", new BsonDocument { { "bsonType", "int" } } },     
                                { "Quando", new BsonDocument { { "bsonType", "date" } } }, 
                                {
                                    "Observacao", new BsonDocument
                                    {
                                        { "bsonType", new BsonArray { "string", "null" } },
                                        { "maxLength", 500 }
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

          
            var col = db.GetCollection<MovimentoPatio>(CollectionName);
            var indexes = new List<CreateIndexModel<MovimentoPatio>>
            {
   
                new(
                    Builders<MovimentoPatio>.IndexKeys.Ascending(x => x.MotoId).Ascending(x => x.Quando),
                    new CreateIndexOptions { Name = "ix_movimentopatio_motoid_quando" }
                )
            };

            
            indexes.Add(
                new CreateIndexModel<MovimentoPatio>(
                    Builders<MovimentoPatio>.IndexKeys.Ascending(x => x.Tipo),
                    new CreateIndexOptions { Name = "ix_movimentopatio_tipo" }
                )
            );

            col.Indexes.CreateMany(indexes);
        }
    }
}

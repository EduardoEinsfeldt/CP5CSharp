
using Loggu.Domain.Entity;
using Loggu.Infraestructure.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Loggu.Infraestructure.Mapping
{
    public static class UsuarioMapping
    {
        private const string CollectionName = "usuarios";

        public static void Ensure(LogguContext ctx)
        {
            var db = ctx.Database;
            var validator = new BsonDocument
            {
                {
                    "$jsonSchema", new BsonDocument
                    {
                        { "bsonType", "object" },
                        { "required", new BsonArray { "_id", "Nome", "Perfil", "Ativo" } },
                        {
                            "properties", new BsonDocument
                            {
                                { "_id", new BsonDocument { { "bsonType", "int" } } },
                                {
                                    "Nome", new BsonDocument
                                    {
                                        { "bsonType", "string" },
                                        { "minLength", 2 }, { "maxLength", 120 }
                                    }
                                },
                                {
                                    "Email", new BsonDocument
                                    {
                                        { "bsonType", new BsonArray { "string", "null" } },
                                        { "maxLength", 160 }
        
                                    }
                                },
                                { "Perfil", new BsonDocument { { "bsonType", "int" } } }, 
                                {
                                    "Ativo", new BsonDocument
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

            var col = db.GetCollection<Usuario>(CollectionName);
            var indexes = new List<CreateIndexModel<Usuario>>
            {
     
                new(
                    Builders<Usuario>.IndexKeys.Ascending(u => u.Email),
                    new CreateIndexOptions { Name = "uk_usuario_email", Unique = true, Sparse = true }
                )
            };
            col.Indexes.CreateMany(indexes);
        }
    }
}

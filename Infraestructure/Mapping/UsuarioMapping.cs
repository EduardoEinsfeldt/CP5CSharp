// src/Infrastructure/Mapping/UsuarioMapping.cs
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

            // ----- Validator (equivalente às constraints do EF) -----
            var validator = new BsonDocument
            {
                {
                    "$jsonSchema", new BsonDocument
                    {
                        { "bsonType", "object" },
                        { "required", new BsonArray { "Id", "Nome", "Perfil", "Ativo" } },
                        {
                            "properties", new BsonDocument
                            {
                                { "Id", new BsonDocument { { "bsonType", "int" } } },
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
                                        // opcional: regex simples de e-mail (comente se preferir)
                                        // { "pattern", "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$" }
                                    }
                                },
                                { "Perfil", new BsonDocument { { "bsonType", "int" } } }, // enum como int
                                {
                                    "Ativo", new BsonDocument
                                    {
                                        { "bsonType", "int" }, { "minimum", 0 }, { "maximum", 1 } // 0/1
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // Cria ou atualiza a coleção com o validator (sem usar enums de outro assembly)
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

            // ----- Índices -----
            var col = db.GetCollection<Usuario>(CollectionName);
            var indexes = new List<CreateIndexModel<Usuario>>
            {
                // PK equivalente
                new(
                    Builders<Usuario>.IndexKeys.Ascending(u => u.Id),
                    new CreateIndexOptions { Name = "pk_usuario_id", Unique = true }
                ),
                // Email único e "sparse": permite múltiplos nulls/não informados
                new(
                    Builders<Usuario>.IndexKeys.Ascending(u => u.Email),
                    new CreateIndexOptions { Name = "uk_usuario_email", Unique = true, Sparse = true }
                )
            };
            col.Indexes.CreateMany(indexes);
        }
    }
}

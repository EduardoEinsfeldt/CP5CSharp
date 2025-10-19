// src/Infraestructure/Mapping/MovimentoPatioMapping.cs
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

            // ---- Validator equivalente ao mapeamento EF ----
            // Requer: Id, MotoId, Tipo, Quando
            // Observacao: string (até 500) ou null
            var validator = new BsonDocument
            {
                {
                    "$jsonSchema", new BsonDocument
                    {
                        { "bsonType", "object" },
                        { "required", new BsonArray { "Id", "MotoId", "Tipo", "Quando" } },
                        {
                            "properties", new BsonDocument
                            {
                                { "Id", new BsonDocument { { "bsonType", "int" } } },
                                { "MotoId", new BsonDocument { { "bsonType", "int" } } },
                                { "RealizadoPorUsuarioId", new BsonDocument { { "bsonType", new BsonArray { "int", "null" } } } },
                                { "Tipo", new BsonDocument { { "bsonType", "int" } } },     // enum como int
                                { "Quando", new BsonDocument { { "bsonType", "date" } } }, // DateTime -> date
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

            // Cria/atualiza coleção com validator
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

            // ---- Índices (equivalentes aos do EF) ----
            var col = db.GetCollection<MovimentoPatio>(CollectionName);
            var indexes = new List<CreateIndexModel<MovimentoPatio>>
            {
                // PK em Id
                new(
                    Builders<MovimentoPatio>.IndexKeys.Ascending(x => x.Id),
                    new CreateIndexOptions { Name = "pk_movimentopatio_id", Unique = true }
                ),
                // Índice útil: (MotoId, Quando)
                new(
                    Builders<MovimentoPatio>.IndexKeys.Ascending(x => x.MotoId).Ascending(x => x.Quando),
                    new CreateIndexOptions { Name = "ix_movimentopatio_motoid_quando" }
                )
            };

            // Opcional: se você costuma filtrar por Tipo
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

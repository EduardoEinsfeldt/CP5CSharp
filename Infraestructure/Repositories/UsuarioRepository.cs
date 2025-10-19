using System.Text.RegularExpressions;
using Loggu.Domain.Entity;
using Loggu.Domain.Repositories;
using Loggu.Infraestructure.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Loggu.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IMongoCollection<Usuario> _col;
        private readonly IMongoCollection<BsonDocument> _counters;

        public UsuarioRepository(LogguContext ctx)
        {
            _col = ctx.Usuarios;
            _counters = ctx.Database.GetCollection<BsonDocument>("_counters");

    
        }

        private async Task<int> NextIdAsync(CancellationToken ct)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", "usuarios_seq");
            var update = Builders<BsonDocument>.Update.Inc("seq", 1);
            var opts = new FindOneAndUpdateOptions<BsonDocument>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };
            var doc = await _counters.FindOneAndUpdateAsync(filter, update, opts, ct);
            return doc["seq"].AsInt32;
        }

        public async Task<IEnumerable<Usuario>> GetAsync(
            string? nome = null,
            string? email = null,
            int? ativo = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default)
        {
            var filter = Builders<Usuario>.Filter.Empty;

            if (!string.IsNullOrWhiteSpace(nome))
            {
                var n = nome.Trim();
                filter &= Builders<Usuario>.Filter.Regex(u => u.Nome, new BsonRegularExpression(n, "i"));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                var e = email.Trim();
                filter &= Builders<Usuario>.Filter.Regex(
                    u => u.Email,
                    new BsonRegularExpression($"^{Regex.Escape(e)}$", "i")
                );
            }

            if (ativo.HasValue)
                filter &= Builders<Usuario>.Filter.Eq(u => u.Ativo, ativo.Value);

            return await _col.Find(filter)
                .SortBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(ct);
        }

        public async Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _col.Find(u => u.Id == id).FirstOrDefaultAsync(ct);

        public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            var e = email.Trim();
            var filter = Builders<Usuario>.Filter.Regex(
                u => u.Email,
                new BsonRegularExpression($"^{Regex.Escape(e)}$", "i")
            );
            return await _col.Find(filter).FirstOrDefaultAsync(ct);
        }

        public async Task<int> CreateAsync(Usuario entity, CancellationToken ct = default)
        {
            entity.Id = await NextIdAsync(ct);
            entity.Nome = (entity.Nome ?? string.Empty).Trim();
            entity.Email = string.IsNullOrWhiteSpace(entity.Email)
                ? null
                : entity.Email.Trim().ToLowerInvariant();

            await _col.InsertOneAsync(entity, cancellationToken: ct);
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Usuario entity, CancellationToken ct = default)
        {
            entity.Nome = (entity.Nome ?? string.Empty).Trim();
            entity.Email = string.IsNullOrWhiteSpace(entity.Email)
                ? null
                : entity.Email.Trim().ToLowerInvariant();

            var res = await _col.ReplaceOneAsync(u => u.Id == entity.Id, entity, cancellationToken: ct);
            return res.IsAcknowledged && res.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var res = await _col.DeleteOneAsync(u => u.Id == id, ct);
            return res.IsAcknowledged && res.DeletedCount > 0;
        }
    }
}
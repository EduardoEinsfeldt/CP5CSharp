using Loggu.Domain.Entity;
using Loggu.Domain.Enums;
using Loggu.Domain.Repositories;
using Loggu.Infraestructure.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Loggu.Infrastructure.Repositories
{
    public class MotoRepository : IMotoRepository
    {
        private readonly IMongoCollection<Moto> _col;
        private readonly IMongoCollection<BsonDocument> _counters;

        public MotoRepository(LogguContext ctx)
        {
            _col = ctx.Motos;
            _counters = ctx.Database.GetCollection<BsonDocument>("_counters");
        }

        private async Task<int> NextIdAsync(CancellationToken ct)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", "motos_seq");
            var update = Builders<BsonDocument>.Update.Inc("seq", 1);
            var opts = new FindOneAndUpdateOptions<BsonDocument>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };
            var doc = await _counters.FindOneAndUpdateAsync(filter, update, opts, ct);
            return doc["seq"].AsInt32;
        }

        public async Task<IEnumerable<Moto>> GetAsync(int? status, string? placa, int page, int pageSize, CancellationToken ct)
        {
            var filter = Builders<Moto>.Filter.Empty;

            if (status.HasValue)
                filter &= Builders<Moto>.Filter.Eq(m => m.Status, (StatusMoto)status.Value);

            if (!string.IsNullOrWhiteSpace(placa))
                filter &= Builders<Moto>.Filter.Regex(m => m.Placa, new BsonRegularExpression(placa.Trim(), "i"));

            return await _col.Find(filter)
                .SortBy(m => m.Id)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(ct);
        }

        public async Task<Moto?> GetByIdAsync(int id, CancellationToken ct)
            => await _col.Find(m => m.Id == id).FirstOrDefaultAsync(ct);

        public async Task<IEnumerable<Moto>> SearchByPlacaAsync(string placa, CancellationToken ct)
        {
            var p = (placa ?? string.Empty).Trim();
            var filter = string.IsNullOrEmpty(p)
                ? Builders<Moto>.Filter.Empty
                : Builders<Moto>.Filter.Regex(m => m.Placa, new BsonRegularExpression(p, "i"));

            return await _col.Find(filter).ToListAsync(ct);
        }

        public async Task<int> CreateAsync(Moto entity, CancellationToken ct)
        {
            entity.Id = await NextIdAsync(ct);
            entity.Placa = (entity.Placa ?? string.Empty).Trim().ToUpperInvariant();
            entity.Chassi = string.IsNullOrWhiteSpace(entity.Chassi) ? null : entity.Chassi!.Trim().ToUpperInvariant();

            await _col.InsertOneAsync(entity, cancellationToken: ct);
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Moto entity, CancellationToken ct)
        {
            entity.Placa = (entity.Placa ?? string.Empty).Trim().ToUpperInvariant();
            entity.Chassi = string.IsNullOrWhiteSpace(entity.Chassi) ? null : entity.Chassi!.Trim().ToUpperInvariant();

            var res = await _col.ReplaceOneAsync(m => m.Id == entity.Id, entity, cancellationToken: ct);
            return res.IsAcknowledged && res.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var res = await _col.DeleteOneAsync(m => m.Id == id, ct);
            return res.IsAcknowledged && res.DeletedCount > 0;
        }
    }
}

using Loggu.Domain.Entity;
using Loggu.Domain.Repositories;
using Loggu.Infraestructure.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Loggu.Infrastructure.Repositories
{
    public class MovimentoPatioRepository : IMovimentoPatioRepository
    {
        private readonly IMongoCollection<MovimentoPatio> _col;
        private readonly IMongoCollection<BsonDocument> _counters;

        public MovimentoPatioRepository(LogguContext ctx)
        {
            _col = ctx.MovimentosPatio;
            _counters = ctx.Database.GetCollection<BsonDocument>("_counters");

 
        }

        private async Task<int> NextIdAsync(CancellationToken ct)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", "movimentos_patio_seq");
            var update = Builders<BsonDocument>.Update.Inc("seq", 1);
            var opts = new FindOneAndUpdateOptions<BsonDocument>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };
            var doc = await _counters.FindOneAndUpdateAsync(filter, update, opts, ct);
            return doc["seq"].AsInt32;
        }

        public async Task<IEnumerable<MovimentoPatio>> GetAsync(
            int? motoId = null,
            int? realizadoPorUsuarioId = null,
            int? tipo = null,
            DateTime? de = null,
            DateTime? ate = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default)
        {
            var filter = Builders<MovimentoPatio>.Filter.Empty;

            if (motoId.HasValue)
                filter &= Builders<MovimentoPatio>.Filter.Eq(x => x.MotoId, motoId.Value);

            if (realizadoPorUsuarioId.HasValue)
                filter &= Builders<MovimentoPatio>.Filter.Eq(x => x.RealizadoPorUsuarioId, realizadoPorUsuarioId.Value);

            if (tipo.HasValue)
                filter &= Builders<MovimentoPatio>.Filter.Eq("Tipo", tipo.Value); 

            if (de.HasValue)
            {
                var from = EnsureUtc(de.Value);
                filter &= Builders<MovimentoPatio>.Filter.Gte(x => x.Quando, from);
            }

            if (ate.HasValue)
            {
                var to = EnsureUtc(ate.Value);
                filter &= Builders<MovimentoPatio>.Filter.Lte(x => x.Quando, to);
            }

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            return await _col.Find(filter)
                .SortByDescending(x => x.Quando).ThenByDescending(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(ct);
        }

        public async Task<MovimentoPatio?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _col.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

        public async Task<int> CreateAsync(MovimentoPatio entity, CancellationToken ct = default)
        {
            entity.Id = await NextIdAsync(ct);
            entity.Quando = EnsureUtc(entity.Quando);

            await _col.InsertOneAsync(entity, cancellationToken: ct);
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(MovimentoPatio entity, CancellationToken ct = default)
        {
            entity.Quando = EnsureUtc(entity.Quando);

            var res = await _col.ReplaceOneAsync(x => x.Id == entity.Id, entity, cancellationToken: ct);
            return res.IsAcknowledged && res.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var res = await _col.DeleteOneAsync(x => x.Id == id, ct);
            return res.IsAcknowledged && res.DeletedCount > 0;
        }


        private static DateTime EnsureUtc(DateTime dt)
        {
            if (dt.Kind == DateTimeKind.Utc) return dt;
            if (dt.Kind == DateTimeKind.Local) return dt.ToUniversalTime();
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        }
    }
}
using Loggu.Domain.Entity;
using Loggu.Domain.Repositories;
using Loggu.Infraestructure.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Loggu.Infrastructure.Repositories
{
    public class CheckRepository : ICheckRepository
    {
        private readonly IMongoCollection<Check> _col;
        private readonly IMongoCollection<BsonDocument> _counters;

        public CheckRepository(LogguContext ctx)
        {
            _col = ctx.Checks; 
            _counters = ctx.Database.GetCollection<BsonDocument>("_counters");
        }

        private async Task<int> NextIdAsync(CancellationToken ct)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", "checks_seq");
            var update = Builders<BsonDocument>.Update.Inc("seq", 1);
            var opts = new FindOneAndUpdateOptions<BsonDocument> { IsUpsert = true, ReturnDocument = ReturnDocument.After };
            var doc = await _counters.FindOneAndUpdateAsync(filter, update, opts, ct);
            return doc["seq"].AsInt32;
        }

        public async Task<int> CreateAsync(Check entity, CancellationToken ct)
        {
            entity.Id = await NextIdAsync(ct);
            entity.RealizadoEm = (entity.RealizadoEm == default) ? DateTime.UtcNow : entity.RealizadoEm.ToUniversalTime();
            await _col.InsertOneAsync(entity, cancellationToken: ct);
            return entity.Id;
        }

        public async Task<Check?> GetByIdAsync(int id, CancellationToken ct)
            => await _col.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

        public async Task<IEnumerable<Check>> GetByMotoIdAsync(int motoId, int page, int pageSize, CancellationToken ct)
        {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 20;
            
            return await _col.Find(x => x.MotoId == motoId)
                .SortByDescending(x => x.RealizadoEm)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(ct);
        }
    }
}
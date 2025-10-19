using Loggu.Domain.Entity;
using Loggu.Domain.Repositories;
using Loggu.Infraestructure.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Loggu.Infrastructure.Repositories
{
    public class OcorrenciaRepository : IOcorrenciaRepository
    {
        private readonly IMongoCollection<Ocorrencia> _col;
        private readonly IMongoCollection<BsonDocument> _counters;

        public OcorrenciaRepository(LogguContext ctx)
        {
            _col = ctx.Ocorrencias; 
            _counters = ctx.Database.GetCollection<BsonDocument>("_counters");
        }

        private async Task<int> NextIdAsync(CancellationToken ct)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", "ocorrencias_seq");
            var update = Builders<BsonDocument>.Update.Inc("seq", 1);
            var opts = new FindOneAndUpdateOptions<BsonDocument> { IsUpsert = true, ReturnDocument = ReturnDocument.After };
            var doc = await _counters.FindOneAndUpdateAsync(filter, update, opts, ct);
            return doc["seq"].AsInt32;
        }

        public async Task<int> CreateAsync(Ocorrencia entity, CancellationToken ct)
        {
            entity.Id = await NextIdAsync(ct);
            entity.AcontecidoEm = (entity.AcontecidoEm == default) ? DateTime.UtcNow : entity.AcontecidoEm.ToUniversalTime();
            await _col.InsertOneAsync(entity, cancellationToken: ct);
            return entity.Id;
        }

        public async Task<Ocorrencia?> GetByIdAsync(int id, CancellationToken ct)
            => await _col.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

        public async Task<IEnumerable<Ocorrencia>> GetByMotoIdAsync(int motoId, int page, int pageSize, CancellationToken ct)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            return await _col.Find(x => x.MotoId == motoId)
                .SortByDescending(x => x.AcontecidoEm)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(ct);
        }
    }
}
using Loggu.Domain.Entity;

namespace Loggu.Domain.Repositories
{
    public interface IOcorrenciaRepository
    {
        Task<Ocorrencia?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Ocorrencia>> GetByMotoIdAsync(int motoId, int page, int pageSize, CancellationToken ct = default);
        Task<int> CreateAsync(Ocorrencia entity, CancellationToken ct = default);
    }
}
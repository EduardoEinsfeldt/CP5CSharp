using Loggu.Domain.Entity;

namespace Loggu.Domain.Repositories
{
    public interface ICheckRepository
    {
        Task<Check?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Check>> GetByMotoIdAsync(int motoId, int page, int pageSize, CancellationToken ct = default);
        Task<int> CreateAsync(Check entity, CancellationToken ct = default);
    }
}
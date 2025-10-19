using Loggu.Domain.Entity;

namespace Loggu.Domain.Repositories
{
    public interface IMotoRepository
    {
        Task<IEnumerable<Moto>> GetAsync(int? status, string? placa, int page, int pageSize, CancellationToken ct);
        Task<Moto?> GetByIdAsync(int id, CancellationToken ct);
        Task<IEnumerable<Moto>> SearchByPlacaAsync(string placa, CancellationToken ct);
        Task<int> CreateAsync(Moto entity, CancellationToken ct);
        Task<bool> UpdateAsync(Moto entity, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
    }
}
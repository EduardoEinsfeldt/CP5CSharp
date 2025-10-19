// src/Domain/Repositories/IMovimentoPatioRepository.cs
using Loggu.Domain.Entity;

namespace Loggu.Domain.Repositories
{
    public interface IMovimentoPatioRepository
    {
        // Listagem com filtros e paginação (usar só os filtros que você realmente precisar)
        Task<IEnumerable<MovimentoPatio>> GetAsync(
            int? motoId = null,
            int? realizadoPorUsuarioId = null,
            int? tipo = null,               // enum TipoMovimento (int)
            DateTime? de = null,            // filtra Quando >= de
            DateTime? ate = null,           // filtra Quando <= ate
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default);

        Task<MovimentoPatio?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<int> CreateAsync(MovimentoPatio entity, CancellationToken ct = default);
        Task<bool> UpdateAsync(MovimentoPatio entity, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}

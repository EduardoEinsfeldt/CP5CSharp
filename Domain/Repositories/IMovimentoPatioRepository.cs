
using Loggu.Domain.Entity;

namespace Loggu.Domain.Repositories
{
    public interface IMovimentoPatioRepository
    {
    
        Task<IEnumerable<MovimentoPatio>> GetAsync(
            int? motoId = null,
            int? realizadoPorUsuarioId = null,
            int? tipo = null,             
            DateTime? de = null,           
            DateTime? ate = null,           
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default);

        Task<MovimentoPatio?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<int> CreateAsync(MovimentoPatio entity, CancellationToken ct = default);
        Task<bool> UpdateAsync(MovimentoPatio entity, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}

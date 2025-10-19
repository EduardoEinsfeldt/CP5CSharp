using Loggu.Domain.Entity;

namespace Loggu.Domain.Repositories
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> GetAsync(
            string? nome = null,
            string? email = null,
            int? ativo = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default);

        Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default);

        Task<int> CreateAsync(Usuario entity, CancellationToken ct = default);
        Task<bool> UpdateAsync(Usuario entity, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}

using System.ComponentModel.DataAnnotations;
using Loggu.Domain.Enums;

namespace Loggu.Domain.Entity
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(120, MinimumLength = 2)]
        public string Nome { get; set; } = string.Empty;

        [EmailAddress, StringLength(160)]
        public string? Email { get; set; }

        [Required]
        public PerfilUsuario Perfil { get; set; } = PerfilUsuario.OPERADOR;

        // 0 = inativo, 1 = ativo
        [Required, Range(0, 1, ErrorMessage = "Ativo deve ser 0 (false) ou 1 (true).")]
        public int Ativo { get; set; } = 1;
    }
}

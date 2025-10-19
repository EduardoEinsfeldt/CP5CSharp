// src/Application/DTOs/MovimentoPatioUpdateDto.cs
using System.ComponentModel.DataAnnotations;

namespace Loggu.Application.DTOs
{
    public class MovimentoPatioUpdateDto
    {
        // Se o seu domínio permitir correção do tipo depois, mantenha; caso contrário, remova.
        [Required]
        public int Tipo { get; set; }               // enum como int

        // Pode ser opcional; quando informado, o controller normaliza para UTC
        public DateTime? Quando { get; set; }

        [StringLength(500)]
        public string? Observacao { get; set; }
    }
}

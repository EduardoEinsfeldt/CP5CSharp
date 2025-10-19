using System.ComponentModel.DataAnnotations;
using Loggu.Domain.Enums;

namespace Loggu.Domain.Entity
{
    public class MovimentoPatio
    {
        [Key]
        public int Id { get; set; }

        
        [Required]
        public int MotoId { get; set; }

        public int? RealizadoPorUsuarioId { get; set; }

        [Required]
        public TipoMovimento Tipo { get; set; } = TipoMovimento.ENTRADA;


        [Required]
        public DateTime Quando { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Observacao { get; set; }
    }
}

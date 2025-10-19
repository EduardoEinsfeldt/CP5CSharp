using System.ComponentModel.DataAnnotations;

namespace Loggu.Domain.Entity
{
    public class Ocorrencia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MotoId { get; set; }

        // Ex.: "DANO", "MULTA", "PANE"
        [Required, StringLength(40, MinimumLength = 3)]
        public string Categoria { get; set; } = "DANO";

        [Required]
        public DateTime AcontecidoEm { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Descricao { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Loggu.Domain.Entity
{
    public class Check
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MotoId { get; set; }

        
        public int? RealizadoPorUsuarioId { get; set; }

       
        [Range(0, 1000000, ErrorMessage = "Quilometragem inválida.")]
        public int? Quilometragem { get; set; }

       
        [Required, Range(0, 1, ErrorMessage = "PneuOk deve ser 0 ou 1.")]
        public int PneuOk { get; set; } = 1;

        [Required, Range(0, 1, ErrorMessage = "FreioOk deve ser 0 ou 1.")]
        public int FreioOk { get; set; } = 1;

        [Required, Range(0, 1, ErrorMessage = "LuzesOk deve ser 0 ou 1.")]
        public int LuzesOk { get; set; } = 1;

        [Required, Range(0, 1, ErrorMessage = "DocumentosOk deve ser 0 ou 1.")]
        public int DocumentosOk { get; set; } = 1;

        
        [Required, Range(0, 1, ErrorMessage = "AptaParaUso deve ser 0 ou 1.")]
        public int AptaParaUso { get; set; } = 1;

      
        [StringLength(500)]
        public string? Observacao { get; set; }

        
        [Required]
        public DateTime RealizadoEm { get; set; } = DateTime.UtcNow;
    }
}

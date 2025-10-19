using System.ComponentModel.DataAnnotations;

namespace Loggu.Domain.Entity
{
    public class Check
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MotoId { get; set; }

        // quem realizou (opcional)
        public int? RealizadoPorUsuarioId { get; set; }

        // Medidas / Itens objetivos
        [Range(0, 1000000, ErrorMessage = "Quilometragem inválida.")]
        public int? Quilometragem { get; set; }

        // Itens de segurança/condição — 0 = não ok, 1 = ok
        [Required, Range(0, 1, ErrorMessage = "PneuOk deve ser 0 ou 1.")]
        public int PneuOk { get; set; } = 1;

        [Required, Range(0, 1, ErrorMessage = "FreioOk deve ser 0 ou 1.")]
        public int FreioOk { get; set; } = 1;

        [Required, Range(0, 1, ErrorMessage = "LuzesOk deve ser 0 ou 1.")]
        public int LuzesOk { get; set; } = 1;

        [Required, Range(0, 1, ErrorMessage = "DocumentosOk deve ser 0 ou 1.")]
        public int DocumentosOk { get; set; } = 1;

        // Resultado geral (apta para uso?) — 0 = não, 1 = sim
        [Required, Range(0, 1, ErrorMessage = "AptaParaUso deve ser 0 ou 1.")]
        public int AptaParaUso { get; set; } = 1;

        // Observações livres (arranhões, avarias, etc.)
        [StringLength(500)]
        public string? Observacao { get; set; }

        // Momento da vistoria (mantive por utilidade operacional)
        [Required]
        public DateTime RealizadoEm { get; set; } = DateTime.UtcNow;
    }
}

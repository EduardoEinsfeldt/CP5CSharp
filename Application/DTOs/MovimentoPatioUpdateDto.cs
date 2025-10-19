
using System.ComponentModel.DataAnnotations;

namespace Loggu.Application.DTOs
{
    public class MovimentoPatioUpdateDto
    {
        
        [Required]
        public int Tipo { get; set; }              

     
        public DateTime? Quando { get; set; }

        [StringLength(500)]
        public string? Observacao { get; set; }
    }
}

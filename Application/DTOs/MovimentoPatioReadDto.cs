
namespace Loggu.Application.DTOs
{
    public class MovimentoPatioReadDto
    {
        public int Id { get; set; }
        public int MotoId { get; set; }
        public int? RealizadoPorUsuarioId { get; set; }
        public int Tipo { get; set; }          
        public DateTime Quando { get; set; }   
        public string? Observacao { get; set; }
    }
}

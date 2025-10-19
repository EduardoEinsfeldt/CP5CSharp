namespace Loggu.Application.DTOs
{

    public class MovimentoPatioCreateDto
    {
        public int MotoId { get; set; }
        public int? RealizadoPorUsuarioId { get; set; }
        public int Tipo { get; set; } = 0;
        public DateTime Quando { get; set; } = DateTime.UtcNow;
        public string? Observacao { get; set; }
    }
}

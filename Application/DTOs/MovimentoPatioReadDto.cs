// src/Application/DTOs/MovimentoPatioReadDto.cs
namespace Loggu.Application.DTOs
{
    public class MovimentoPatioReadDto
    {
        public int Id { get; set; }
        public int MotoId { get; set; }
        public int? RealizadoPorUsuarioId { get; set; }
        public int Tipo { get; set; }          // enum como int
        public DateTime Quando { get; set; }   // UTC recomendado
        public string? Observacao { get; set; }
    }
}

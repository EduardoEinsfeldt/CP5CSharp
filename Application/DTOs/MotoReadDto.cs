namespace Loggu.Application.DTOs
{
    public class MotoReadDto
    {
        public int Id { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string? Chassi { get; set; }
        public int Status { get; set; }
        public int EmPatio { get; set; }
    }
}

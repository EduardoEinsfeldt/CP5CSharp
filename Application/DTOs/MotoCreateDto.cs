namespace Loggu.Application.DTOs
{
    public class MotoCreateDto
    {
        public string Placa { get; set; } = string.Empty;
        public string? Chassi { get; set; }
        public int Status { get; set; } = 0;
        public int EmPatio { get; set; } = 0;
    }
}

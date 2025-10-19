namespace Loggu.Application.DTOs
{
    public class MotoUpdateDto
    {
        public string Placa { get; set; } = string.Empty;
        public string? Chassi { get; set; }
        public int Status { get; set; } = 0;
    }
}

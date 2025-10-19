namespace Loggu.Application.DTOs
{
    public class UsuarioCreateDto
    {
        public string Nome { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int Perfil { get; set; } = 0;
        public int Ativo { get; set; } = 1;
    }
}

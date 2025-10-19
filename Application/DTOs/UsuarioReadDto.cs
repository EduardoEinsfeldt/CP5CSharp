namespace Loggu.Application.DTOs
{
    public class UsuarioReadDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int Perfil { get; set; }
        public int Ativo { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using Loggu.Domain.Enums;

namespace Loggu.Domain.Entity
{
    public class Moto
    {
        [Key]
        public int Id { get; set; }

        //Colocando um Comentário para testar Devops <- Foi adicionado antes e tinha ido.
        //Adicionando novo comentário. Não sei o que havia acontecido. Não sei o que está acontecendo mais.
       
        [Required, StringLength(7, MinimumLength = 7)]
        [RegularExpression(@"^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$",
            ErrorMessage = "Placa inválida (padrão Mercosul, ex: BRA2E19).")]
        public string Placa { get; set; } = string.Empty;

      
        [StringLength(17, MinimumLength = 17)]
        [RegularExpression(@"^[A-HJ-NPR-Z0-9]{17}$",
            ErrorMessage = "Chassi inválido (17 caracteres, sem I, O, Q).")]
        public string? Chassi { get; set; }

        [Required]
        public StatusMoto Status { get; set; } = StatusMoto.ATIVA;

        
        [Required, Range(0, 1, ErrorMessage = "EmPatio deve ser 0 (false) ou 1 (true).")]
        public int EmPatio { get; set; } = 0;
    }

}

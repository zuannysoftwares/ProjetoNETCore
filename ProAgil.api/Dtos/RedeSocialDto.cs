using System.ComponentModel.DataAnnotations;

namespace ProAgil.api.Dtos
{
    public class RedeSocialDto
    {
        public int Id { get; set; }
        [Required (ErrorMessage="Por favor, digite um {0} para a Rede Social")]
        public string Nome { get; set; }
        [Required (ErrorMessage="Insira a {0} da Rede Social")]
        public string URL { get; set; }
    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProAgil.api.Dtos
{
    public class EventoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage="Insira um {0} para o evento")]
        public string Local { get; set; }
        public string DataEvento { get; set; }

        [Required (ErrorMessage="Insira um Tema para o evento")]
        [StringLength(200, MinimumLength=4, ErrorMessage="O {0} deve ter no Mínimo 4 caracteres e no Máximo 200 caracteres")]
        public string Tema { get; set; }

        [Range(1, 500, ErrorMessage="A quantidade Máxima de pessoas para o evento é de 500 pessoas")]
        public int QtdPessoas { get; set; }
        public string ImageUrl { get; set; }
        [Phone]
        public string Telefone { get; set; }

        [Required (ErrorMessage="Digite um {0}")]
        [EmailAddress]
        public string Email { get; set; }
        public List<LoteDto> Lotes { get; }
        public List<RedeSocialDto> RedesSociais { get; }
        public List<PalestranteDto> Palestrantes { get; set; }
    }
}
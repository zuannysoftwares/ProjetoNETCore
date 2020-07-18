using System.Linq;
using AutoMapper;
using ProAgil.api.Dtos;
using ProAgil.Domain;
using ProAgil.Domain.Identity;

namespace ProAgil.api.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()//Construtor
        {
            //Mapeia a entidade do banco de dados (Domain) e DTO: o que será mostrado para o usuário
            //Relacionamento de Muitos pra Muitos: Veja abaixo a explicação
            //ForMember: para o membro Palestrantes que está dentro de EventoDto
            //MapFrom: Mapeia de PalestranteEventos que está dentro de Evento
            //Select: E selecione os Palestrantes do Domain PalestranteEventos numa lista (ToList())
            CreateMap<Evento, EventoDto>()
                .ForMember(dest => dest.Palestrantes, opt => {
                    opt.MapFrom(src => src.PalestranteEventos.Select(x => x.Palestrante).ToList());
                }).ReverseMap();
            CreateMap<Palestrante, PalestranteDto>()
                .ForMember(dest => dest.Eventos, opt => {
                    opt.MapFrom(src => src.PalestranteEventos.Select(x => x.Evento).ToList());
                }).ReverseMap();
            
            //<Lote, LoteDto>: Mapeia de Lote PARA LoteDto
            CreateMap<Lote, LoteDto>().ReverseMap(); //ReverseMap(): Inversão de mapeamento => Mapeia de LoteDto PARA Lote
            CreateMap<RedeSocial, RedeSocialDto>().ReverseMap();

            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserLoginDto>().ReverseMap();
        }
    }
}
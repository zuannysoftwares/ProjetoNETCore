using System.Threading.Tasks;
using ProAgil.Domain;

namespace ProAgil.Repository
{
    public interface IProAgilRepository
    {
        //Métodos gerais
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;

        Task<bool> SaveChangesAsync();

        //EVENTOS
        Task<Evento[]> GetAllEventoAsyncByTema(string tema, bool incluiPalestrantes);
        Task<Evento[]> GetAllEventoAsync(bool incluiPalestrantes);
        Task<Evento> GetAllEventoAsyncById(int eventoId, bool incluiPalestrantes);

        //PALESTRANTES
        Task<Palestrante[]> GetAllPalestrantesAsyncByName(string nome, bool incluiEventos);
        Task<Palestrante> GetAllPalestranteAsync(int palestranteId, bool incluiEventos);
    }
}
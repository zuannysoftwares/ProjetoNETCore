using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProAgil.Domain;

namespace ProAgil.Repository
{
    public class ProAgilRepository : IProAgilRepository
    {
        private readonly ProAgilContext _context;
        public ProAgilRepository(ProAgilContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class//Onde T é uma CLASSE
        {
            _context.Add(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<Evento[]> GetAllEventoAsync(bool incluiPalestrantes = false)
        {
            IQueryable<Evento> query = _context.Eventos
                .Include( c => c.Lotes)
                .Include(c => c.RedesSociais);

                if(incluiPalestrantes)
                {
                    query = query
                        .Include(pe => pe.PalestranteEventos)
                        .ThenInclude(p => p.Palestrante); //ThenInclude = Inclua também
                }

                query = query.OrderByDescending(c => c.DataEvento);

                return await query.ToArrayAsync();
        }

        public async Task<Evento[]> GetAllEventoAsyncByTema(string tema, bool incluiPalestrantes = false)
        {
            IQueryable<Evento> query = _context.Eventos
                .Include( c => c.Lotes)
                .Include(c => c.RedesSociais);

                if(incluiPalestrantes)
                {
                    query = query
                        .Include(pe => pe.PalestranteEventos)
                        .ThenInclude(p => p.Palestrante); //ThenInclude = Inclua também
                }

                query = query.OrderByDescending(c => c.DataEvento)
                            .Where(c => c.Tema.ToLower().Contains(tema.ToLower()));

                return await query.ToArrayAsync();
        }

        public async Task<Evento> GetAllEventoAsyncById(int eventoId, bool incluiPalestrantes)
        {
            IQueryable<Evento> query = _context.Eventos
                .Include( c => c.Lotes)
                .Include(c => c.RedesSociais);

                if(incluiPalestrantes)
                {
                    query = query
                        .Include(pe => pe.PalestranteEventos)
                        .ThenInclude(p => p.Palestrante); //ThenInclude = Inclua também
                }

                query = query.OrderByDescending(c => c.DataEvento)
                            .Where(c => c.Id == eventoId);

                return await query.FirstOrDefaultAsync();
        }

        public async Task<Palestrante> GetAllPalestranteAsync(int palestranteId, bool incluiEventos = false)
        {
            IQueryable<Palestrante> query = _context.Palestrantes
                .Include(c => c.RedesSociais);

                if(incluiEventos)
                {
                    query = query
                        .Include(pe => pe.PalestranteEventos)
                        .ThenInclude(e => e.Evento); //ThenInclude = Inclua também
                }

                query = query.OrderBy(p => p.Nome)
                            .Where(pid => pid.Id == palestranteId);

                return await query.FirstOrDefaultAsync();
        }

        public async Task<Palestrante[]> GetAllPalestrantesAsyncByName(string nome, bool incluiEventos = false)
        {
            IQueryable<Palestrante> query = _context.Palestrantes
                .Include(c => c.RedesSociais);

                if(incluiEventos)
                {
                    query = query
                        .Include(pe => pe.PalestranteEventos)
                        .ThenInclude(e => e.Evento); //ThenInclude = Inclua também
                }

                query = query.Where(pn => pn.Nome.ToLower().Contains(nome.ToLower()));

                return await query.ToArrayAsync();
        }
    }
}
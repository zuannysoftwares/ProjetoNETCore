using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProAgil.api.Dtos;
using ProAgil.Domain;
using ProAgil.Repository;

namespace ProAgil.api.Controllers
{
    [ApiController]//Com isso, eu faço as validações com o DataAnotation nas entidades e não preciso usar o ModelState.Isvalid e o FromBody dos métodos POST e PUT
    [Route("[controller]")]
    public class EventoController : ControllerBase
    {
        private readonly IProAgilRepository _repo;
        private readonly IMapper _mapper;
        public EventoController(IProAgilRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetEventos()
        {
            try
            {
                var evento = await _repo.GetAllEventoAsync(true);
                var results = _mapper.Map<IEnumerable<EventoDto>>(evento);

                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar informações no banco de dados.");                
            }
        }

        [HttpGet("EventoId")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var evento = await _repo.GetAllEventoAsyncById(id, true);
                var results = _mapper.Map<EventoDto>(evento);

                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar informações no banco de dados.");                
            }
        }

        [HttpGet("getByTema/{tema}")]
        public async Task<IActionResult> Get(string tema)
        {
            try
            {
                var evento = await _repo.GetAllEventoAsyncByTema(tema, true);
                var results = _mapper.Map<IEnumerable<EventoDto>>(evento);
                
                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar informações no banco de dados.");                
            }
        }
 
        [HttpPost]
        public async Task<IActionResult> Post(EventoDto model)
        {
            try
            {
                //Inversão de mapeamento
                var evento = _mapper.Map<Evento>(model);

                _repo.Add(evento);
                if(await _repo.SaveChangesAsync())
                {
                    return Created($"/api/evento/{model.Id}", _mapper.Map<EventoDto>(evento));
                      
                }
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Erro ao salvar o Evento: " + model.Tema);                
            }

            return BadRequest();
        }

        [HttpPut("{EventoId}")]
        public async Task<IActionResult> Put(int eventoId, EventoDto model)
        {
            try
            {
                //Se NÃO encontrar na base o evento com esse ID, RETORNA NotFound(), Se não, ATUALIZA
                var evento = await _repo.GetAllEventoAsyncById(eventoId, false);
                var idLotes = new List<int>();
                var idRedesSociais = new List<int>();

                if(evento == null) return NotFound();

                model.Lotes.ForEach(item => idLotes.Add(item.Id));
                model.RedesSociais.ForEach(item => idRedesSociais.Add(item.Id));

                var lotes = evento.Lotes.Where(
                    lote => !idLotes.Contains(lote.Id)
                ).ToArray();
                
                var redesSociais = evento.RedesSociais.Where(
                    rede => !idRedesSociais.Contains(rede.Id)
                ).ToArray();
                
                if(lotes.Length > 0) _repo.DeleteRange(lotes);
                if(redesSociais.Length > 0) _repo.DeleteRange(redesSociais);

                _mapper.Map(model, evento);

                _repo.Update(evento);

                if(await _repo.SaveChangesAsync())
                {
                    return Created($"/api/evento/{model.Id}", _mapper.Map<EventoDto>(evento));
                }
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Erro ao Atualizar o Evento: " + model.Tema);                
            }

            return BadRequest();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int eventoId)
        {
            try
            {
                var evento = await _repo.GetAllEventoAsyncById(eventoId, false);
                if(evento == null) return NotFound();

                _repo.Delete(evento);

                if(await _repo.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Não foi possível excluir o Evento: " + eventoId);                
            }

            return BadRequest();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload()
        {
            try
            {
                var file = Request.Form.Files[0]; //Pego o arquivo
                var folderName = Path.Combine("Resources", "Images"); // O diretório/pasta onde será armazenado o arquivo
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName); //Combinando o diretorio da minha aplicação com a pasta que quero armazenar

                if(file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;// Pego o nome do meu arquivo
                    var fullPath = Path.Combine(pathToSave, fileName.Replace("\"", " ").Trim());// Se no nome vier aspas ou espaço, eu removo isso

                    using(var stream = new FileStream(fullPath, FileMode.Create))//Crio um arquivo no FileStream
                    {
                        file.CopyTo(stream);
                    }
                }

                return Ok();
            }
            catch(System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Não foi possível realizar o Upload da imagem");
            }

            return BadRequest("Ocorreu um erro. Tente novamente mais tarde");
        }
    }
}
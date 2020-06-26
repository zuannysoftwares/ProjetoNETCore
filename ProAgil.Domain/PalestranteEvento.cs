namespace ProAgil.Domain
{
    //Entidade criada para o relacionamento N -> N 
    //Ou seja, N Palestrantes podem participar de N Eventos
    public class PalestranteEvento
    {
        public int PalestranteId { get; set; }
        public Palestrante Palestrante { get; set; }
        public int EventoId { get; set; }
        public Evento Evento { get; set; }
    }
}
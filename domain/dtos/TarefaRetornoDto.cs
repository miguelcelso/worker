namespace Domain.dtos
{
    public class TarefaRetornoDto
    {
        public int Id { get; set; }
        public string? Descricao { get; set; }
        public DateTime? DataTarefa { get; set; }
        public char Status { get; set; }
    }
}

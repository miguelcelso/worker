namespace Domain.dtos
{
    public class TarefaCreateDto
    {
        public string? Descricao { get; set; }
        public DateTime? DataTarefa { get; set; }
        public char Status { get; set; }
    }
}

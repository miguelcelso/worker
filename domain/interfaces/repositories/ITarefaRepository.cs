using Domain.models;

namespace Domain.interfaces.repositories
{
    public interface ITarefaRepository
    {
        Task CreateTarefa(Tarefa tarefa);
    }
}

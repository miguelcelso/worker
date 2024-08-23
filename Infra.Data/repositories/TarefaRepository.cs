using Domain.interfaces.repositories;
using Domain.models;
using Infra.Data.contexts;

namespace Infra.Data.repositories
{
    public class TarefaRepository : ITarefaRepository
    {
        private readonly ContextoTarefa _contextoTarefa;

        public TarefaRepository(ContextoTarefa contextoTarefa) => _contextoTarefa = contextoTarefa ?? throw new ArgumentNullException(nameof(contextoTarefa));

        public async Task CreateTarefa(Tarefa tarefa)
        {
            _= tarefa ?? throw new ArgumentNullException(nameof(tarefa));
            try
            {
                await _contextoTarefa.Tarefas.AddAsync(tarefa);
                await Salvar();
            }
            catch
            {
                throw;
            }
        }

        #region "Métodos privados"
        private async Task<int> Salvar()
        {
            var resultado = await _contextoTarefa.SaveChangesAsync();
            if(resultado < 1)
            {
                throw new Exception("Por favor verificar o banco de dados, tarefa não foi salva!");
            }
            return resultado;
        }
        #endregion
    }
}
 
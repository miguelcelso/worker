using AutoMapper;
using Domain.dtos;
using Domain.interfaces.messagebrokers;
using Domain.interfaces.repositories;
using Domain.interfaces.services;
using Domain.models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Service.tarefas
{
    public class TarefaService(IMapper mapper, ITarefaRepository tarefaRepository, IMensagemService mensagemService, ILogger<TarefaService> logger, IServiceScopeFactory scopeFactory) : ITarefaService
    {
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly ITarefaRepository _tarefaRepository = tarefaRepository ?? throw new ArgumentNullException(nameof(tarefaRepository));
        private readonly IMensagemService _mensagemService = mensagemService ?? throw new ArgumentNullException(nameof(mensagemService));
        private readonly ILogger<TarefaService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));

        public async Task CreateTarefa()
        {
            try
            {
                await mensagemService.LerMensagem(async (string mensagem) =>
                {
                    _logger.LogInformation("Inicio da leitura da tarefa: {message}", mensagem);
                    try
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            _logger.LogInformation("Leitura da fila do rabbitmq finaliza com sucesso");
                            var tarefa = JsonConvert.DeserializeObject<Tarefa>(mensagem);
                            _logger.LogInformation("Inicio da criacao da tarefa: {tarefa}", tarefa);
                            await _tarefaRepository.CreateTarefa(tarefa);
                            _logger.LogInformation("Tarefa criada com sucesso, tarefa: {tarefa}", tarefa);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Erro ao salvar a Tarefa, mesagem: {mensagem} e trace: {trace}", ex.Message, ex.StackTrace);
                        throw new Exception("Por favor verificar, a tarefa não foi salva!");

                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao enviar Tarefa para o rabbitmq, mesagem: {mensagem} e trace: {trace}", ex.Message, ex.StackTrace);
                throw new Exception("Por favor verificar o funcionamento do rabbitmq");
            }
          
        }
    }
}

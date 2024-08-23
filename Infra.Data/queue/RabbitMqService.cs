using Domain.interfaces.messagebrokers;
using Infra.Data.configurations.rabbitmq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Infra.Data.queue
{
    public class RabbitMqService: IMensagemService
    {
        private readonly IOptions<RabbitMqConfiguracao> options;
        private readonly IModel _model;
        private readonly IConnection _connection;

        const string _queueName = "tarefas";
        const string _exchangeName = "store.exchange";

        public RabbitMqService(IOptions<RabbitMqConfiguracao> options, ILoggerFactory loggerFactory)
        {
            this.options = options;
            _connection = CreateChannel();
            _model = _connection.CreateModel();
            _model.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);
            _model.ExchangeDeclare(_exchangeName, ExchangeType.Fanout, durable: true, autoDelete: false);
            _model.QueueBind(_queueName, _exchangeName, string.Empty);
        }
        #region "Métodos publícos"
        public async Task LerMensagem(Func<string, Task> action)
        {
            var consumer = new AsyncEventingBasicConsumer(_model);
            consumer.Received += async (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var text = Encoding.UTF8.GetString(body);
                await action(text);
                _model.BasicAck(ea.DeliveryTag, false);
            };
            _model.BasicConsume(_queueName, false, consumer);
            await Task.CompletedTask;
        }

        public async Task EnviarMensagem(string message)
        {
            using var model = _connection.CreateModel();
            var body = Encoding.UTF8.GetBytes(message);
            model.BasicPublish(_exchangeName,
                                 string.Empty,
                                 basicProperties: null,
                                 body: body);
            await Task.CompletedTask;
        }
        public void Dispose()
        {
            if (_model.IsOpen)
                _model.Close();
            if (_connection.IsOpen)
                _connection.Close();
        }
        #endregion

        #region "Métodos privados"
        private IConnection CreateChannel()
        {
            ConnectionFactory connection = new ConnectionFactory()
            {
                UserName = options.Value.Username,
                Password = options.Value.Password,
                HostName = options.Value.HostName
            };

            connection.DispatchConsumersAsync = true;
            var channel = connection.CreateConnection();
            return channel;
        }
        #endregion
    }
}

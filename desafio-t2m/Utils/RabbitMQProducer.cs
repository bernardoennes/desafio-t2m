using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace desafio_t2m.Utils
{
    public class RabbitMQProducer
    {
        private readonly IModel _channel;
        private readonly string _queueName;

        public RabbitMQProducer(RabbitMQConnection connection, IConfiguration config)
        {
            _channel = connection.GetChannel();
            _queueName = config["RabbitMQ:QueueName"] 
                ?? throw new ArgumentNullException(nameof(config), "QueueName não pode ser nulo.");
        }

        public void Publish<T>(T message)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            _channel.BasicPublish(exchange: "",
                                  routingKey: _queueName,
                                  basicProperties: null,
                                  body: body);
        }
    }
}

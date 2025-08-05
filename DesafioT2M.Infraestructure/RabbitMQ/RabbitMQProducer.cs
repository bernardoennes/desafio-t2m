using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace desafioT2m.Infraestructure.RabbitMQ
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        private readonly IModel _channel;
        private readonly string _exchange;

        public RabbitMQProducer(RabbitMQConnection connection, IConfiguration config)
        {
            _channel = connection.GetChannel();
            _exchange = config["RabbitMQ:Exchange"] ?? "product_exchange";
            _channel.ExchangeDeclare(exchange: _exchange, type: ExchangeType.Direct, durable: true);
        }

        public void Publish<T>(T message, string routingKey)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var props = _channel.CreateBasicProperties();
            props.Persistent = true;

            _channel.BasicPublish(
                exchange: _exchange,
                routingKey: routingKey,
                basicProperties: props,
                body: body
            );

            Console.WriteLine($"[✔] Mensagem publicada - RoutingKey: {routingKey}");
        }
    }
}

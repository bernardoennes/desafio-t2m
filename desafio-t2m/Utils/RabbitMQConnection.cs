using System;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace desafio_t2m.Utils
{
    public class RabbitMQConnection
    {
        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private IModel? _channel;

        public RabbitMQConnection(IConfiguration configuration)
        {
            _configuration = configuration;
            Connect();
        }

        private void Connect()
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"],
                Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = _configuration["RabbitMQ:UserName"],
                Password = _configuration["RabbitMQ:Password"]
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            var queueName = _configuration["RabbitMQ:QueueName"];
            _channel.QueueDeclare(queue: queueName,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        public IModel GetChannel() => _channel ?? throw new InvalidOperationException("RabbitMQ channel não inicializado.");
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desafioT2m.Infraestructure.RabbitMQ
{
    public interface IRabbitMQProducer
    {
        void Publish<T>(T message, string routingKey);
    }
}

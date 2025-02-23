using System;
namespace AuthorizationService.Infrastructure.MessageBroker
{
    public interface IRabbitMqService 
    {
        void Send<T>(T message, string queue);
    }
}


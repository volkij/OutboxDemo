using Microsoft.Extensions.Configuration;

namespace OutboxDemo.Outbox.Contracts;

public interface IOutboxBuilder
{
    IOutboxBuilder SetMessageBroker(string hostName, int port, string userName, string password);
    IOutboxBuilder SetSubsrieber(string queueName);
    IOutboxBuilder SetPublisher(string exchangeName);

    IOutboxBuilder ConfigureFromSection(IConfigurationSection section);
    IOutboxConfiguration Build();
    
}
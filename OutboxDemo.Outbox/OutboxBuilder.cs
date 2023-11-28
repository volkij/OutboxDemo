using OutboxDemo.Outbox.Configuration;
using OutboxDemo.Outbox.Contracts;
using Microsoft.Extensions.Configuration;

namespace OutboxDemo.Outbox;

public class OutboxBuilder : IOutboxBuilder
{
    private RabbitConfiguration _configuration;

    public OutboxBuilder(RabbitConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public IOutboxBuilder  SetMessageBroker(string hostName, int port, string userName, string password)
    {
        _configuration.HostName = hostName;
        _configuration.Port = port;
        _configuration.UserName = userName;
        _configuration.Password = password;
        return this;
    }
    
    public IOutboxBuilder  SetSubsrieber(string queueName)
    {
        _configuration.QueueName = queueName;
        return this;
    }
    
    public IOutboxBuilder  SetPublisher(string exchangeName)
    {
        _configuration.ExchangeName = exchangeName;
        return this;
    }

    public IOutboxBuilder ConfigureFromSection(IConfigurationSection section)
    {
        section.Bind(_configuration);
        return this;
    }
    
    public IOutboxConfiguration Build()
    {
        return _configuration;
    }
}
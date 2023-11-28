using OutboxDemo.Outbox.Configuration;
using OutboxDemo.Outbox.Contracts;
using OutboxDemo.Outbox.Publishers;
using OutboxDemo.Outbox.Subscribers;
using Microsoft.Extensions.Logging;
using OutboxDemo.Messages;

namespace OutboxDemo.Outbox;

public class OutboxFactory : IOutboxFactory
{
    private RabbitConfiguration _configuration;
    private ILogger _logger;
    private OutboxFactory(RabbitConfiguration configuration, ILogger logger = null)
    {
        _configuration = configuration;
        _logger = logger;
    }
    public static IOutboxFactory Create(Action<IOutboxBuilder> configure, ILogger logger = null)
    {
        RabbitConfiguration configuration = new RabbitConfiguration();
        OutboxBuilder builder = new OutboxBuilder(configuration);

        configure(builder);
        return new OutboxFactory(configuration, logger);
    }

    public IOutboxConfiguration CreateConfiguration()
    {
        return _configuration;
    }

    public RabbitSubscriber<T> CreateSubscriber<T>() where T : OutboxMessage
    {
        RabbitSubscriber<T> subscriber = new RabbitSubscriber<T>(_configuration, _logger);
        return subscriber;
    }
    
    public RabbitPublisher<T> CreatePublisher<T>() where T : OutboxMessage
    {
        RabbitPublisher<T> publisher = new RabbitPublisher<T>(_configuration, _logger);
        return publisher;
    }
}
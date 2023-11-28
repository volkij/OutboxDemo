using System.Runtime.ExceptionServices;
using OutboxDemo.Messages;
using OutboxDemo.Outbox.Messages;
using OutboxDemo.Outbox.Publishers;
using OutboxDemo.Outbox.Subscribers;

namespace OutboxDemo.Outbox.Contracts;

public interface IOutboxFactory
{
    IOutboxConfiguration CreateConfiguration();

    RabbitSubscriber<T> CreateSubscriber<T>() where T : OutboxMessage;

    RabbitPublisher<T> CreatePublisher<T>() where T : OutboxMessage;
}
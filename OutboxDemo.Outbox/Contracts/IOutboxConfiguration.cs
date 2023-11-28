namespace OutboxDemo.Outbox.Contracts;

public interface IOutboxConfiguration
{
    string HostName { get; set; }

    int Port { get; set; }

    string UserName { get; set; }
    string Password { get; set; }

    string QueueName { get; set; }

    string ExchangeName { get; set; }

    string RoutingKey { get; set; }

    bool EnableAutoReconnect { get; set; }

    int AutoReconnectTimeout { get; set; }

    bool AutoAddProperties { get; set; }

    int? MessageExpiration { get; set; }

    string AppID { get; set; }

    bool IsCompressed { get; set; }
}
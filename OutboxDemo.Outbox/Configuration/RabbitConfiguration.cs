using OutboxDemo.Outbox.Contracts;

namespace OutboxDemo.Outbox.Configuration;

public class RabbitConfiguration : IOutboxConfiguration
{
    public string HostName { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }

    public string QueueName { get; set; }

    public string ExchangeName { get; set; }

    public string RoutingKey { get; set; }

    public bool EnableAutoReconnect { get; set; } = false;

    public int AutoReconnectTimeout { get; set; } = 30;

    public bool AutoAddProperties { get; set; } = false;

    /// <summary>
    /// in seconds
    /// </summary>
    public int? MessageExpiration { get; set; }

    public string AppID { get; set; }

    public bool IsCompressed { get; set; } = false;
}
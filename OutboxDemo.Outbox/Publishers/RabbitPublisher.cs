using System.IO.Compression;
using System.Text;
using System.Text.Json;
using OutboxDemo.Outbox.Client;
using OutboxDemo.Outbox.Configuration;
using OutboxDemo.Outbox.Contracts;
using OutboxDemo.Outbox.Messages.Exceptions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using OutboxDemo.Messages;

namespace OutboxDemo.Outbox.Publishers;

public sealed class RabbitPublisher<T> : RabbitClient, IOutboxPublisher<T>, IDisposable where T : OutboxMessage
{
    private OutQueue<T> _queue = new OutQueue<T>();


    public RabbitPublisher(RabbitConfiguration rabbitConfiguration, ILogger logger) : base(rabbitConfiguration, logger)
    {
        this.ReconnectAsync = true;
        InitializeConnection();
        
        if (string.IsNullOrEmpty(rabbitConfiguration.HostName) ||
            string.IsNullOrEmpty(rabbitConfiguration.ExchangeName))
        {
            throw new ClientConfigurationException("HostName or ExchangeName not specified");
        }
    }
    
    public override void OnConnected()
    {
        base.OnConnected();
        SendMessagesFromQueue();
    }       
    public void Publish(T message)
    {
        Logger.LogInformation("Sending message to RabbitMQ. Messages in queue: {count}", _queue.Count());
    
        if (!IsConnected)
        {
            _queue.Enqueue(message);
            ProcessFailedConnection();
            return;
        }

        try
        {
            IBasicProperties properties = GetMessageProperties(message);

            string json = JsonSerializer.Serialize(message);
            byte[] body;

            if (RabbitConfiguration.IsCompressed)
            {
                body = Compress(Encoding.UTF8.GetBytes(json));
            }
            else
            {
                body = Encoding.UTF8.GetBytes(json);
            }

            Channel.BasicPublish(exchange: RabbitConfiguration.ExchangeName,
                routingKey: RabbitConfiguration.RoutingKey,
                basicProperties: properties,
                body: body);

            OnMessageSent(message);
            Logger.LogInformation($"Sent to RabbitMQ - Message.ID : {message.ID}");
            Logger.LogDebug(json);
        }
        catch (Exception ex)
        {
            _queue.Enqueue(message);
            Logger.LogError(ex, "An error occurred while sending a message to RabbitMQ.");
            ProcessFailedConnection(ex);
        }
    }

    private byte[] Compress(byte[] data)
    {
        using (var compressedStream = new MemoryStream())
        using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
        {
            zipStream.Write(data, 0, data.Length);
            zipStream.Close();
            return compressedStream.ToArray();
        }
    }


    private IBasicProperties GetMessageProperties(T message)
    {
        var properties = Channel.CreateBasicProperties();
        properties.Persistent = true;

        if (RabbitConfiguration.AutoAddProperties)
        {
            properties.Type = message.Type.ToString();
            properties.MessageId = message.ID.ToString();
            properties.AppId = RabbitConfiguration.AppID;
            properties.ContentEncoding = RabbitConfiguration.IsCompressed ? "GZIP" : "UTF-8";
            properties.ContentType = "application/json";
            properties.Priority = Convert.ToByte(message.Priority);
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }

        if (RabbitConfiguration.MessageExpiration != null)
        {
            properties.Expiration = (RabbitConfiguration.MessageExpiration * 1000).ToString();
        }

        return properties;
    }

    private void SendMessagesFromQueue()
    {
        while (_queue.Count() > 0)
        {
            var message = _queue.Dequeue();
            if (message != null)
            {
                Publish(message);
            }
        }
    }
    
    public int MessagesInQueue
    {
        get => _queue.Count();
    }
    
    public bool TryEmptyQueue()
    { 
        SendMessagesFromQueue();
        return _queue.Count() == 0;
    }
   
    public delegate void MessageSentHandler(T message);
    public event MessageSentHandler MessageSent;
    private void OnMessageSent(T message) => MessageSent?.Invoke(message);


}
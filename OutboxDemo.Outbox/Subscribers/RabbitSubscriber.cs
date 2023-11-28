using OutboxDemo.Outbox.Client;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Reflection;
using OutboxDemo.Outbox.Configuration;
using OutboxDemo.Outbox.Messages.Exceptions;
using System.IO.Compression;
using OutboxDemo.Messages;
using OutboxDemo.Messages.Attributes;

namespace OutboxDemo.Outbox.Subscribers
{
    public sealed class RabbitSubscriber<T> : RabbitClient, IDisposable where T : OutboxMessage
    {
        private bool _isReadyForConsume = false;

        public RabbitSubscriber(RabbitConfiguration rabbitConfiguration, ILogger logger) : base(rabbitConfiguration, logger)
        {
            this.ReconnectAsync = false;
        }
        public void Start()
        {

            if (_isReadyForConsume) return;

            _isReadyForConsume = true;
            InitializeConnection();

            while (_isReadyForConsume)
            {
                try
                {
                    if (!this.IsConnected)
                    {
                        throw new Exception("Connection to RabbitMQ not exist");
                    }

                    if (!_isConsumerRegistered)
                    {
                        _isConsumerRegistered = true;
                        ConsumeMessages();
                    }

                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to connect to RabbitMQ.");
                    _isConsumerRegistered = false;


                    ProcessFailedConnection(ex);

                }
            }
            
        }

        

        

        private CancellationTokenSource _cancellationTokenSource;

        private bool _isConsumerRegistered = false;

        private void ConsumeMessages()
        {
            Channel.BasicQos(prefetchSize: 0, prefetchCount: 1000, global: false);
           _cancellationTokenSource = new CancellationTokenSource();
            var consumer = new EventingBasicConsumer(Channel);

            consumer.Received += (bc, ea) =>
                {
                    if (!_cancellationTokenSource.IsCancellationRequested)
                    {
                        string body = "";
                        try
                        {
                            var properties = ea.BasicProperties;
                            var messageTypeInfo = typeof(T).GetCustomAttribute<MessageTypeAttribute>();
                            bool isMessageTypeSpecified = false;

                            //check message type from RabbitMQ message type
                            if (messageTypeInfo != null && !string.IsNullOrEmpty(properties.Type))
                            {
                                if (!Enum.TryParse(properties.Type, out OutboxMessageType messageTypeFromProperties) || messageTypeInfo.MessageType != messageTypeFromProperties)
                                {
                                    throw new InvalidMessageTypeException("Invalid message type from Rabbit properties: " + properties.Type);
                                }

                            }

                            if (RabbitConfiguration.IsCompressed)
                            {
                                body = Encoding.UTF8.GetString(DeCompress(ea.Body.ToArray()));
                            }
                            else
                            {
                                body = Encoding.UTF8.GetString(ea.Body.ToArray());
                            }

                            var message = JsonSerializer.Deserialize<T>(body);

                            if (message == null) throw new NullReferenceException(nameof(message));

                            // check message type from body
                            if (messageTypeInfo != null && (!Enum.TryParse(message.Type.ToString(), out OutboxMessageType messageTypeFromBody) || messageTypeInfo.MessageType != messageTypeFromBody))
                            {
                                throw new InvalidMessageTypeException("Invalid message type from Rabbit body: " + properties.Type);
                            }
                                                        

                            OnMessageReceived(message);
                        }
                        catch (InvalidMessageTypeException)
                        {
                            OnMessageFormatFailed(body);
                        }
                        catch (JsonException)
                        {
                            OnMessageFormatFailed(body);
                        }
                        catch (Exception ex)
                        {
                            if (!_cancellationTokenSource.IsCancellationRequested)
                            {
                                _cancellationTokenSource.Cancel();
                                if (Channel?.IsOpen == true)
                                {
                                    Channel.Close();
                                }
                                if (Connection?.IsOpen == true)
                                {
                                    Connection.Close();
                                }
                            }
                        }
                        finally
                        {
                            Channel.BasicAck(ea.DeliveryTag, false);
                        }
                    }

                };
            Channel.BasicConsume(queue: RabbitConfiguration.QueueName, autoAck: false, consumer: consumer);
         }

        private byte[] DeCompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data)) 
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }

        public delegate void MessageReceivedHandler(T message);
        public event MessageReceivedHandler MessageReceived;
        private void OnMessageReceived(T message) => MessageReceived?.Invoke(message);

        public delegate void MessageFormatFailedHandler(string message);
        public event MessageFormatFailedHandler MessageFormatFailed;
        private void OnMessageFormatFailed(string message) => MessageFormatFailed?.Invoke(message);
        
    }
}

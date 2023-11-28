using OutboxDemo.Outbox;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutboxDemo.Outbox.Configuration;
using OutboxDemo.Outbox.Contracts;

namespace OutboxDemo.Outbox.Client
{
    public class RabbitClient : IClient
    {
        protected readonly IOutboxConfiguration RabbitConfiguration;
        protected readonly ILogger Logger;
        protected IConnection Connection;
        protected IModel Channel;
        private bool _isTryingToReconnect = false; 
        protected bool ReconnectAsync { get; set; } = true;
        public RabbitClient(IOutboxConfiguration rabbitConfiguration, ILogger logger)
        {
            Logger = logger;
            RabbitConfiguration = rabbitConfiguration;
        }
        protected void InitializeConnection()
        {
            Logger.LogInformation("Connecting to RabbitMQ.");
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = RabbitConfiguration.HostName,
                    Port = RabbitConfiguration.Port,
                    UserName = RabbitConfiguration.UserName,
                    Password = RabbitConfiguration.Password
                };

                Connection = factory.CreateConnection();
                Channel = Connection.CreateModel();
               
                Logger.LogInformation("Connected to RabbitMQ.");
                OnConnected();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to connect to RabbitMQ.");
                ProcessFailedConnection(ex);

            }
        }

        public virtual void OnConnected()
        {
            
        }

        public bool IsConnected
        {
            get
            {
                return Connection?.IsOpen == true && Channel?.IsOpen == true;
            }
        }

        protected void ProcessFailedConnection(Exception? exception = null)
        {
            if (RabbitConfiguration.EnableAutoReconnect)
            {
                if (ReconnectAsync)
                {
                    TryReconnectAsync();
                }
                else
                {
                    TryReconnect();
                }
            }
            else
            {
                if (exception != null) throw exception;
                else throw new Exception("RabbitMQ connection is closed.");
            }
        }

        protected void TryReconnect()
        {
            if (!_isTryingToReconnect)
            {
                Logger.LogInformation("Trying to reconnect to RabbitMQ in {timeout} seconds.", RabbitConfiguration.AutoReconnectTimeout);
                _isTryingToReconnect = true;
                System.Threading.Thread.Sleep(RabbitConfiguration.AutoReconnectTimeout * 1000);
                InitializeConnection();
                _isTryingToReconnect = false;
            }
        }

        protected async void TryReconnectAsync()
        {
            if (!_isTryingToReconnect)
            {
                Logger.LogInformation("Trying to reconnect to RabbitMQ in {timeout} seconds.", RabbitConfiguration.AutoReconnectTimeout);
                _isTryingToReconnect = true;
                await Task.Delay(RabbitConfiguration.AutoReconnectTimeout * 1000);
                InitializeConnection();
                _isTryingToReconnect = false;
            }
        }

        protected bool IsTryingToReconnect
        {
            get => _isTryingToReconnect;
        }
        
        public void Dispose()
        {
            Channel.Close();
            Connection.Close();
        }
    }
}

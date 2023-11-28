using OutboxDemo.Messages.Attributes;
using System.Reflection;
using System.Text.Json.Serialization;

namespace OutboxDemo.Messages
{
    public abstract class OutboxMessage
    {
        public OutboxMessage(string description = null, int? priority = null)
        {
            ID = Guid.NewGuid();
            DateCreate = DateTime.Now.ToUniversalTime();
            if (priority != null) Priority = 1;
            Description = description;

            var messageTypeAttr = this.GetType().GetCustomAttribute<MessageTypeAttribute>();
            if (messageTypeAttr != null)
            {
                Type = messageTypeAttr.MessageType;
            }
        }

        public OutboxMessage()
        {

        }        

        public Guid ID { get; set; }
        public DateTime DateCreate { get; set; }
        public OutboxMessageType Type { get; set; }
        public int Priority { get; set; } = 1;
        public string? Description { get; set; }
        
        public int Version { get; } = 1;
        
        [JsonPropertyOrder(10)]
        public object? Data { get; set; }

       
    }
}
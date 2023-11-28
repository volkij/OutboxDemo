using System.Text.Json.Serialization;
using OutboxDemo.Messages.Attributes;
using OutboxDemo.Messages.Data;

namespace OutboxDemo.Messages;

[MessageTypeAttribute(OutboxMessageType.CARD_SYNC)]
public class OutboxCardSync : OutboxMessage
{
    public OutboxCardSync(List<OutboxCardData> objToData, string description = null, int? priority = null) : base(description, priority)
    {
        Data = objToData;
    }

    public OutboxCardSync() : base() { }

  
    
    [JsonPropertyOrder(10)]
    public new List<OutboxCardData> Data { get; set; }

   
}
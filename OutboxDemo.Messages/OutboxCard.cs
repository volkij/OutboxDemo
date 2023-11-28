using OutboxDemo.Messages.Attributes;
using OutboxDemo.Messages.Data;
using System.Text.Json.Serialization;


namespace OutboxDemo.Messages;

[MessageTypeAttribute(OutboxMessageType.CARD)]
public class OutboxCard : OutboxMessage
{
   public OutboxCard(OutboxCardData objToData, string description = null, int? priority = null) : base(description, priority)
    {
        Data = objToData;
    }

    public OutboxCard() : base() { }
        

    [JsonPropertyOrder(10)]
    public new OutboxCardData? Data { get; set; }

   
}
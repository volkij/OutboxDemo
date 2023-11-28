using System.Text.Json.Serialization;

namespace OutboxDemo.Messages
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OutboxMessageType
    {
        CARD,
        CARD_SYNC,
        TRANSACTION,
        CLIENT_MESSAGE
    }
}

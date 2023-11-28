
namespace OutboxDemo.Messages.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class MessageTypeAttribute : Attribute
    {
        public OutboxMessageType MessageType { get; }

        public MessageTypeAttribute(OutboxMessageType messageType)
        {
            MessageType = messageType;
        }
    }
}

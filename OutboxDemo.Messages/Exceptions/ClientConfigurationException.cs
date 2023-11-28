namespace OutboxDemo.Outbox.Messages.Exceptions;

public class ClientConfigurationException : Exception
{
    public ClientConfigurationException(string message) : base(message)
    {
    }
}
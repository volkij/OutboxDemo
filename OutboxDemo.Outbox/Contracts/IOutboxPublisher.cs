using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutboxDemo.Messages;
using OutboxDemo.Outbox.Messages;

namespace OutboxDemo.Outbox.Contracts
{
    public interface IOutboxPublisher<T> where T : OutboxMessage
    {
        void Publish(T message);
       
    }
}

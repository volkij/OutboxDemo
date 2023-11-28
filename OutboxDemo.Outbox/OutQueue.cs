using OutboxDemo.Messages;
using System.Collections.Concurrent;

namespace OutboxDemo.Outbox
{
    public class OutQueue<T> where T : OutboxMessage
    {
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

        public void Enqueue(T item)
        {
            _queue.Enqueue(item);
        }

        public int Count()
        {
            return _queue.Count();
        }
        

        public T? Dequeue()
        {
            if (_queue.TryDequeue(out T item))
            {
                return item;
            }
            return null;
        }
    }
}

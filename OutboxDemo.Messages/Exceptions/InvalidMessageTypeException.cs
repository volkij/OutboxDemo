using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutboxDemo.Outbox.Messages.Exceptions
{
    public class InvalidMessageTypeException : Exception
    {
        public InvalidMessageTypeException(string message) : base(message)
        {
        }
    }
}

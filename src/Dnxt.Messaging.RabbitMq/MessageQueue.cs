using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Dnxt.Messaging.Rabbit
{
    public class MessageQueue
    {
        public MessageQueue([NotNull] string name, bool durable = false, bool exclusive = false, bool autoDelete = false,
            IDictionary<string, object> arguments = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            Name = name;
            Durable = durable;
            Exclusive = exclusive;
            AutoDelete = autoDelete;
            Arguments = arguments;
        }

        public string Name { get; }
        public bool Durable { get; }
        public bool Exclusive { get; }
        public bool AutoDelete { get; }
        public IDictionary<string, object> Arguments { get; }
    }
}
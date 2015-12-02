using System;
using JetBrains.Annotations;
using RabbitMQ.Client;

namespace Dnxt.Messaging.Rabbit
{
    public class SendingMessage
    {
        public SendingMessage([NotNull] PublicationAddress address, [NotNull] byte[] body, Action<IBasicProperties> basicPropertiesUpdater = null)
        {
            if (address == null) throw new ArgumentNullException(nameof(address));
            if (body == null) throw new ArgumentNullException(nameof(body));

            Address = address;
            BasicPropertiesUpdater = basicPropertiesUpdater ?? (p => { });
            Body = body;
        }

        [NotNull]
        public PublicationAddress Address { get; }

        [NotNull]
        public Action<IBasicProperties> BasicPropertiesUpdater { get; }

        [NotNull]
        public byte[] Body { get; }
    }
}
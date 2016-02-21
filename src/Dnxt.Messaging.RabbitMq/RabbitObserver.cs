using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dnxt.RxAsync;
using JetBrains.Annotations;
using RabbitMQ.Client;

namespace Dnxt.Messaging.Rabbit
{
    public class RabbitObserver : IAsyncObserver<byte[]>
    {
        [NotNull] private readonly IModel _channel;
        [NotNull] private readonly string _queueName;

        public RabbitObserver([NotNull] IModel channel, [NotNull] string queueName, bool durable = false, bool exclusive = false, bool autoDelete = false, IDictionary<string, object> arguments = null)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (queueName == null) throw new ArgumentNullException(nameof(queueName));
            _channel = channel;
            _queueName = queueName;

            channel.QueueDeclare(queueName, durable, exclusive, autoDelete, arguments);
        }

        public Task OnNextAsync(byte[] msg, CancellationToken token)
        {
            return Task.Factory.StartNew(() =>
            {
                _channel.BasicPublish(exchange: "",
                    routingKey: _queueName,
                    basicProperties: null,
                    body: msg);
            });
        }

        public Task OnErrorAsync(Exception e, CancellationToken token)
        {
            return Task.FromResult(1);
        }

        public Task OnCompletedAsync(CancellationToken token)
        {
            return Task.FromResult(1);
        }
    }
}

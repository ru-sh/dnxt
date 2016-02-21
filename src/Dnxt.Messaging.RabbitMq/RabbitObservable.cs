using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dnxt.Extensions;
using Dnxt.Logging;
using Dnxt.RxAsync;
using Dnxt.RxAsync.Subjects;
using JetBrains.Annotations;
using RabbitMQ.Client;

namespace Dnxt.Messaging.Rabbit
{
    public class RabbitObservable : IAsyncObservable<byte[]>
    {
        [NotNull]
        private readonly IModel _channel;
        [NotNull]
        private readonly string _queueName;

        [NotNull]
        private readonly IAsyncSubject<byte[]> _subject = new ConcurrentSubject<byte[]>();

        public RabbitObservable(
            [NotNull] IModel channel,
            [NotNull] string queueName,
            bool durable = false,
            bool exclusive = false,
            bool autoDelete = false,
            IDictionary<string, object> arguments = null)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (queueName == null) throw new ArgumentNullException(nameof(queueName));
            _channel = channel;
            _queueName = queueName;

            channel.QueueDeclare(queueName, durable, exclusive, autoDelete, arguments);
        }

        public async Task Listen([NotNull] ILogger logger, CancellationToken token)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            var consumer = new QueueingBasicConsumer(_channel);
            var consumerTag = _channel.BasicConsume(_queueName, false, consumer);
            logger.Log("RabbitMQ Consuming", new { _queueName, consumerTag });

            while (!token.IsCancellationRequested)
            {
                var e = consumer.Queue.DequeueNoWait(null);
                if (e == null)
                {
                    await Task.Delay(50).ConfigureAwait(false);
                    continue;
                }

                var body = e.Body;
                _subject.OnNextAsync(body, CancellationToken.None).ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        logger.Log(t.Exception);
                    }
                    else
                    {
                        _channel.BasicAck(e.DeliveryTag, false);
                    }
                }).NotWaitAndLogExceptions(logger);
            }
        }

        public IDisposable Subscribe(IAsyncObserver<byte[]> observer)
        {
            return _subject.Subscribe(observer);
        }
    }
}
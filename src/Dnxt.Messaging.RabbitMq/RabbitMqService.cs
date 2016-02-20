using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dnxt.Extensions;
using Dnxt.Logging;
using JetBrains.Annotations;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Dnxt.Messaging.Rabbit
{
    public class RabbitMqService
    {
        [NotNull]
        private readonly ConnectionFactory _factory;

        [NotNull]
        private readonly ILogger _logger;

        [NotNull]
        private readonly ConcurrentQueue<ulong> _toAck = new ConcurrentQueue<ulong>();

        [NotNull]
        private readonly ConcurrentQueue<Tuple<SendingMessage, TaskCompletionSource<object>>> _toSend =
            new ConcurrentQueue<Tuple<SendingMessage, TaskCompletionSource<object>>>();

        private readonly TimeSpan _sleepDelay;

        public RabbitMqService([NotNull] ConnectionFactory factory, [NotNull] ILogger logger, TimeSpan? sleepDelay = null)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            _factory = factory;
            _logger = logger;

            _sleepDelay = sleepDelay ?? TimeSpan.FromMilliseconds(100);
        }


        public async Task ConnectAsync([NotNull] Dictionary<MessageQueue, Action<BasicDeliverEventArgs>> rxQueues, CancellationToken token, [NotNull] ILogger logger)
        {
            if (rxQueues == null) throw new ArgumentNullException(nameof(rxQueues));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            
            using (var conn = _factory.CreateConnection())
            {
                // IModel instance (channel) should not be used by more than one thread simultaneously
                using (var channel = conn.CreateModel())
                {
                    var consumers = rxQueues
                        .Select(r =>
                        {
                            var queue = r.Key;
                            channel.QueueDeclare(queue.Name, queue.Durable, queue.Exclusive, queue.AutoDelete, queue.Arguments);
                            var consumer = new QueueingBasicConsumer();
                            var tag = channel.BasicConsume(queue.Name, false, consumer);
                            return Tuple.Create(consumer, r.Value);
                        }).ToList();

                    ConsumeQueueAsync(consumers, token).NotWaitAndLogExceptions(logger);

                    while (!token.IsCancellationRequested)
                    {
                        var isEmpty = true;

                        ulong deliveryTag;
                        while (_toAck.TryDequeue(out deliveryTag))
                        {
                            channel.BasicAck(deliveryTag, false);
                            isEmpty = false;
                        }

                        Tuple<SendingMessage, TaskCompletionSource<object>> toSend;
                        while (_toSend.TryDequeue(out toSend))
                        {
                            var msg = toSend.Item1;
                            var props = channel.CreateBasicProperties();
                            msg.BasicPropertiesUpdater.Invoke(props);
                            channel.BasicPublish(msg.Address, props, msg.Body);
                            channel.BasicPublish("e", "r", props, msg.Body);

                            var callback = toSend.Item2;
                            callback.SetResult(true);
                            isEmpty = false;
                        }

                        if (isEmpty)
                        {
                            await Task.Delay(_sleepDelay);
                        }
                    }

                    channel.Close(200, "Goodbye");

                    Tuple<SendingMessage, TaskCompletionSource<object>> notSend;
                    while (_toSend.TryDequeue(out notSend))
                    {
                        var tcs = notSend.Item2;
                        tcs.SetException(new InvalidOperationException("Connection is closed."));
                    }
                }

                conn.Close();
            }
        }

        public Task Send(SendingMessage sendingMessage)
        {
            var tcs = new TaskCompletionSource<object>();
            _toSend.Enqueue(Tuple.Create(sendingMessage, tcs));

            return tcs.Task;
        }

        public Task Send(string exchangeName, byte[] body)
        {
            var address = new PublicationAddress("", exchangeName, "");
            var sendingMessage = new SendingMessage(address, body);
            return Send(sendingMessage);
        }

        private async Task ConsumeQueueAsync(
            IReadOnlyCollection<Tuple<QueueingBasicConsumer, Action<BasicDeliverEventArgs>>> consumers,
            CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var isEmpty = true;
                foreach (var tuple in consumers)
                {
                    var consumer = tuple.Item1;
                    var onReceive = tuple.Item2;
                    var ea = consumer.Queue.DequeueNoWait(null);

                    if (ea == null) continue;

                    isEmpty = false;
                    try
                    {
                        onReceive(ea);
                        _toAck.Enqueue(ea.DeliveryTag);
                    }
                    catch (Exception e)
                    {
                        _logger.Log(e);
                    }
                }

                if (isEmpty)
                {
                    await Task.Delay(_sleepDelay).ConfigureAwait(false);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dnxt.Environmenting;
using Dnxt.Logging;
using Dnxt.Messaging.Rabbit;
using NUnit.Framework;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Dnxt.Tests
{
    [TestFixture]
    public class RabbitTests
    {
        [Test]
        public async Task Test()
        {
            var logger = new Logger(new ConsoleLogObserver(), new DefaultDateTimeProvider());

            var consumers = new Dictionary<MessageQueue, Action<BasicDeliverEventArgs>>()
            {
                {
                    new MessageQueue("test"),
                    args =>
                    {
                        var body = args.Body;
                        var msg = Encoding.UTF8.GetString(body, 0, body.Length);
                        logger.Log("Received", new {msg});
                    }
                }
            };

            var connectionFactory = new ConnectionFactory() { HostName = "amqp://localhost/" };
            var provider = new RabbitMqService(connectionFactory, logger);
            var cts = new CancellationTokenSource();

            provider.ConnectAsync(consumers, cts.Token, logger);

            await provider.Send(new SendingMessage(new PublicationAddress("d", "test", ""), Encoding.UTF8.GetBytes("Hi! Test!")));
            var tcs = new TaskCompletionSource<object>();

            await tcs.Task;
        }
    }
}
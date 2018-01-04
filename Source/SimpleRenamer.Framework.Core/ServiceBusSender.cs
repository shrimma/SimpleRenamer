using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Sarjee.SimpleRenamer.Common.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.Core
{
    public class ServiceBusSender : IMessageSender
    {
        private QueueClient _queueClient;
        private string serviceBusNamespace = "sb://sb-js-simplerenamer-test.servicebus.windows.net/";
        private string serviceBusEntityPath = "testing123";
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHubSender"/> class.
        /// </summary>
        public ServiceBusSender()
        {
            //MessagingFactorySettings with token provider and AMQP transport type
            MessagingFactorySettings factorySettings = new MessagingFactorySettings()
            {
                TransportType = TransportType.Amqp,
                TokenProvider = TokenProvider.CreateSharedSecretTokenProvider("WriteOnly", "BOclQZs38Vg6piG8h6DtnbRi/6lnNpJvuuXWtGFbSgk=", TokenScope.Namespace)
            };

            //create messaging factory with the SB namespace and the factory settings
            MessagingFactory messagingFactory = MessagingFactory.CreateAsync(serviceBusNamespace, factorySettings).GetAwaiter().GetResult();
            _queueClient = messagingFactory.CreateQueueClient(serviceBusEntityPath);
            _queueClient.RetryPolicy = RetryPolicy.Default;
        }

        public void SendAsync(string jsonPayload)
        {
            Task t = Task.Run(async () =>
            {
                try
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(jsonPayload);
                    MemoryStream memoryStream = new MemoryStream(byteArray);
                    BrokeredMessage brokeredMessage = new BrokeredMessage(memoryStream);

                    brokeredMessage.Properties.Add(new KeyValuePair<string, object>(nameof(Environment.MachineName), Environment.MachineName));
                    brokeredMessage.Properties.Add(new KeyValuePair<string, object>(nameof(Environment.OSVersion), Environment.OSVersion));

                    await _queueClient.SendAsync(brokeredMessage);
                }
                catch (Exception ex)
                {
                    string ko = ex.ToString();
                }
            }, CancellationToken.None);
            t.GetAwaiter().GetResult();
        }
    }
}

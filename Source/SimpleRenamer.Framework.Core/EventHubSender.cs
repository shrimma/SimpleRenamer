using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Sarjee.SimpleRenamer.Common.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Interface.IMessageSender" />
    public class EventHubSender : IMessageSender
    {
        private EventHubClient _eventHubClient;
        private string serviceBusNamespace = "sb://eh-js-simplerenamer-test.servicebus.windows.net";
        private string serviceBusEntityPath = "testing123";
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHubSender"/> class.
        /// </summary>
        public EventHubSender()
        {
            //MessagingFactorySettings with token provider and AMQP transport type
            MessagingFactorySettings factorySettings = new MessagingFactorySettings()
            {
                TransportType = TransportType.Amqp,
                TokenProvider = TokenProvider.CreateSharedSecretTokenProvider("WriteOnly", "D7xMxm1ElDCp2vMKFwvMSFTk1xU7utZYqvdQQcPJGCU=", TokenScope.Namespace)
            };

            //create messaging factory with the SB namespace and the factory settings
            MessagingFactory messagingFactory = MessagingFactory.CreateAsync(serviceBusNamespace, factorySettings).GetAwaiter().GetResult();
            _eventHubClient = messagingFactory.CreateEventHubClient(serviceBusEntityPath);
            _eventHubClient.RetryPolicy = RetryPolicy.Default;
        }

        public void SendAsync(string jsonPayload)
        {
            Task t = Task.Run(async () =>
            {
                try
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(jsonPayload);
                    MemoryStream memoryStream = new MemoryStream(byteArray);
                    EventData eventData = new EventData(memoryStream);

                    eventData.Properties.Add(new KeyValuePair<string, object>(nameof(Environment.MachineName), Environment.MachineName));
                    eventData.Properties.Add(new KeyValuePair<string, object>(nameof(Environment.OSVersion), Environment.OSVersion));

                    await _eventHubClient.SendAsync(eventData);
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

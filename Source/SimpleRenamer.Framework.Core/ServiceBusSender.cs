using Microsoft.Azure.ServiceBus;
using Sarjee.SimpleRenamer.Common.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.Core
{
    public class ServiceBusSender : IMessageSender
    {
        private ILogger _logger;
        private IQueueClient _queueClient;
        private const string connectionString = "Endpoint=sb://sb-js-simplerenamer-test.servicebus.windows.net/;SharedAccessKeyName=Write;SharedAccessKey=blmnxHEbTBpI6FeZrXH4I5nAFYDIGrgAhacIcCA3QbA=;EntityPath=testing123";

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBusSender"/> class.
        /// </summary>
        public ServiceBusSender(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            ServiceBusConnectionStringBuilder sbcsb = new ServiceBusConnectionStringBuilder(connectionString)
            {
                TransportType = TransportType.Amqp,
            };
            _queueClient = new QueueClient(sbcsb, retryPolicy: RetryPolicy.Default);
        }

        public async Task SendAsync(string jsonPayload)
        {
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(jsonPayload);
                Message brokeredMessage = new Message(byteArray);
                brokeredMessage.UserProperties.Add(new KeyValuePair<string, object>(nameof(Environment.MachineName), Environment.MachineName.ToString()));
                brokeredMessage.UserProperties.Add(new KeyValuePair<string, object>(nameof(Environment.OSVersion), Environment.OSVersion.ToString()));

                await _queueClient.SendAsync(brokeredMessage);
            }
            catch (Exception ex)
            {
                _logger.TraceException(ex);
            }
        }
    }
}

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
        private ILogger _logger;
        private QueueClient _queueClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBusSender"/> class.
        /// </summary>
        public ServiceBusSender(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _queueClient = QueueClient.CreateFromConnectionString("Endpoint=sb://sb-js-simplerenamer-test.servicebus.windows.net/;SharedAccessKeyName=Write;SharedAccessKey=blmnxHEbTBpI6FeZrXH4I5nAFYDIGrgAhacIcCA3QbA=;EntityPath=testing123;TransportType=Amqp");
            _queueClient.RetryPolicy = RetryPolicy.Default;
        }

        public async Task SendAsync(string jsonPayload)
        {
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(jsonPayload);
                MemoryStream memoryStream = new MemoryStream(byteArray);
                BrokeredMessage brokeredMessage = new BrokeredMessage(memoryStream);

                brokeredMessage.Properties.Add(new KeyValuePair<string, object>(nameof(Environment.MachineName), Environment.MachineName.ToString()));
                brokeredMessage.Properties.Add(new KeyValuePair<string, object>(nameof(Environment.OSVersion), Environment.OSVersion.ToString()));

                await _queueClient.SendAsync(brokeredMessage);
            }
            catch (Exception ex)
            {
                _logger.TraceException(ex);
            }
        }
    }
}

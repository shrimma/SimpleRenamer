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
        private ILogger _logger;
        private EventHubClient _eventHubClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHubSender"/> class.
        /// </summary>
        public EventHubSender(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventHubClient = EventHubClient.CreateFromConnectionString("Endpoint=sb://eh-js-simplerenamer-test.servicebus.windows.net/;SharedAccessKeyName=Write;SharedAccessKey=/Wt6GcOuUsuH7NXHB4sa3vRNRnDhQEldCt9ayLb7vCM=;EntityPath=testing123");
            _eventHubClient.RetryPolicy = RetryPolicy.Default;
        }

        public async Task SendAsync(string jsonPayload)
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
                _logger.TraceException(ex);
            }
        }
    }
}

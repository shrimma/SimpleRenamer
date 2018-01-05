using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Sarjee.SimpleRenamer.Common.Helpers;
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
        private IHelper _helper;
        private EventHubClient _eventHubClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHubSender"/> class.
        /// </summary>
        public EventHubSender(ILogger logger, IHelper helper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _eventHubClient = EventHubClient.CreateFromConnectionString("Endpoint=sb://eh-js-simplerenamer-test.servicebus.windows.net/;SharedAccessKeyName=Write;SharedAccessKey=/Wt6GcOuUsuH7NXHB4sa3vRNRnDhQEldCt9ayLb7vCM=;EntityPath=testing123");
            _eventHubClient.RetryPolicy = RetryPolicy.Default;
        }

        public async Task SendAsync(string jsonPayload)
        {
            //setup the variables
            bool trySend = true;
            int retryCount = 0;
            int offset = ThreadLocalRandom.Next(100, 300);

            //create our eventData
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonPayload);
            MemoryStream memoryStream = new MemoryStream(byteArray);
            EventData eventData = new EventData(memoryStream);
            eventData.Properties.Add(new KeyValuePair<string, object>(nameof(Environment.MachineName), Environment.MachineName.ToString()));
            eventData.Properties.Add(new KeyValuePair<string, object>(nameof(Environment.OSVersion), Environment.OSVersion.ToString()));

            while (trySend)
            {
                try
                {

                    await _eventHubClient.SendAsync(eventData);
                    trySend = false;
                    break;
                }
                catch (Exception ex)
                {
                    _logger.TraceException(ex);

                    //increment the retry counter
                    retryCount++;

                    try
                    {
                        //try and create the eventhubclient again
                        _eventHubClient = EventHubClient.CreateFromConnectionString("Endpoint=sb://eh-js-simplerenamer-test.servicebus.windows.net/;SharedAccessKeyName=Write;SharedAccessKey=/Wt6GcOuUsuH7NXHB4sa3vRNRnDhQEldCt9ayLb7vCM=;EntityPath=testing123");
                        _eventHubClient.RetryPolicy = RetryPolicy.Default;
                    }
                    catch
                    {
                        await _helper.ExponentialDelayAsync(offset, retryCount, 2);
                    }
                }
            }
        }
    }
}

using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Sarjee.SimpleRenamer.Common.Model;

namespace SimpleRenamer.Function
{
    public static class SimpleRenamerQueueTrigger
    {
        [FunctionName("SimpleRenamerQueueTrigger")]
        public static void Run(BrokeredMessage mySbMsg, TraceWriter log, IAsyncCollector<StatsFile> outputTvShow, IAsyncCollector<StatsFile> outputMovie)
        {
            log.Info($"C# Queue trigger function processed");
        }
    }
}

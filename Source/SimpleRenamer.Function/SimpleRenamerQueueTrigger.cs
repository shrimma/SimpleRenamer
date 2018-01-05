using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Sarjee.SimpleRenamer.Common.Model;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SimpleRenamer.Function
{
    public static class SimpleRenamerQueueTrigger
    {
        [FunctionName("SimpleRenamerQueueTrigger")]
        public static async Task Run([ServiceBusTrigger("testing123", Connection = "ServiceBusConnection")] BrokeredMessage mySbMsg, TraceWriter log, [DocumentDB("SimpleRenamerStats", "RenamedTvShows", ConnectionStringSetting = "myCosmosDB")]IAsyncCollector<StatsFile> outputTvShow, [DocumentDB("SimpleRenamerStats", "RenamedMovies", ConnectionStringSetting = "myCosmosDB")]IAsyncCollector<StatsFile> outputMovie)
        {
            log.Info($"C# Queue trigger function processed");
            string responseBody = string.Empty;
            //get the stream from the body of the message
            using (StreamReader bodyStream = new StreamReader(mySbMsg.GetBody<Stream>()))
            {
                responseBody = await bodyStream.ReadToEndAsync();
            }
            log.Info(responseBody);

            List<StatsFile> files = JsonConvert.DeserializeObject<List<StatsFile>>(responseBody);

            log.Info($"Found {files.Count.ToString()} files.");

            foreach (StatsFile file in files)
            {
                if (file.MediaType == FileType.Movie)
                {
                    await outputMovie.AddAsync(file);
                    log.Info("Found Movie");
                }
                else if (file.MediaType == FileType.TvShow)
                {
                    await outputTvShow.AddAsync(file);
                    log.Info("Found TVSHOW");
                }
                else
                {
                    log.Info("Found unprocessable");
                }
            }
        }
    }
}

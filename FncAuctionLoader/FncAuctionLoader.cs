using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using WowAuctionAggrigator;
using System.Configuration;

namespace FncAuctionLoader
{
    public static class FncAuctionLoader
    {
        [FunctionName("FncAuctionLoader")]
        public static void Run([TimerTrigger("0 50 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            using (AuctionLoader al = new AuctionLoader(ConfigurationManager.ConnectionStrings["Database"].ConnectionString, ConfigurationManager.AppSettings.Get("BlizzAPIKey"), "stormrage"))
            {
                al.loadDataTabe();
                al.clearPriorLoadedData();
                al.bulkCopyAuctions();
            }
        }
    }
}

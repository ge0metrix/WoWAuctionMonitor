using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace FncAuctionLoader
{
    public static class FncAuctionLoader
    {
        [FunctionName("FncAuctionLoader")]
        public static void Run([TimerTrigger("0 50 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            WowAuctionAggrigator.AuctionLoader.LoadAuctionData();
        }
    }
}

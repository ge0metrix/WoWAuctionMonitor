using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowAuctionAggrigator;
using System.Configuration;

namespace WoWAuctionMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            using (AuctionLoader al = new AuctionLoader(ConfigurationManager.ConnectionStrings["Database"].ConnectionString, ConfigurationManager.AppSettings.Get("BlizzAPIKey"), "stormrage")) 
            {
                al.loadDataTabe();
                al.clearPriorLoadedData();
                al.bulkCopyAuctions();
                al.aggAuctionData();
            }
        }
    }
}

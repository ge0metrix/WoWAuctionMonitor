using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowAuctionAggrigator;

namespace WoWAuctionMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            AuctionLoader.LoadAuctionData();
        }
    }
}

using System;
using WowDotNetAPI;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

namespace WowAuctionAggrigator
{
    public class AuctionLoader
    {
        public static void LoadAuctionData()
        {

            var str = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;

            WowExplorer explorer = new WowExplorer(Region.US, Locale.en_US, ConfigurationManager.AppSettings.Get("BlizzAPIKey"));
            var ah = explorer.GetAuctions("stormrage");
            var af = explorer.GetAuctionFiles("stormrage");
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(af.Files.Take(1).Select(x => x.LastModified).ToArray()[0]);

            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("ItemID");
            dt.Columns.Add("Owner");
            dt.Columns.Add("Bid");
            dt.Columns.Add("BuyOut");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("TimeLeft");
            dt.Columns.Add("LoadDate");
            dt.Columns["LoadDate"].DataType = typeof(DateTime);
            dt.Columns["LoadDate"].DefaultValue = dateTimeOffset.DateTime;


            DataRow dr;
            int i = 0;
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand { Connection = conn, CommandTimeout = 0 })
                {
                    cmd.CommandText = "DELETE FROM Auctions WHERE LoadDate = '" + dateTimeOffset.DateTime.ToString() + "';";
                    cmd.ExecuteNonQuery();
                }
            }

            foreach (var auction in ah.CurrentAuctions)
            {
                i++;
                dr = dt.NewRow();
                dr["ID"] = auction.Id;
                dr["ItemID"] = auction.ItemId;
                dr["Owner"] = auction.Owner;
                dr["Bid"] = auction.Bid;
                dr["BuyOut"] = auction.Buyout;
                dr["Quantity"] = auction.Quantity;
                dr["TimeLeft"] = auction.TimeLeft;
                dt.Rows.Add(dr);
            }
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                using (SqlBulkCopy bcp = new SqlBulkCopy(conn))
                {
                    bcp.BulkCopyTimeout = 0;
                    bcp.DestinationTableName = "Auctions";
                    bcp.ColumnMappings.Add(0, 1);
                    bcp.ColumnMappings.Add(1, 2);
                    bcp.ColumnMappings.Add(2, 3);
                    bcp.ColumnMappings.Add(3, 4);
                    bcp.ColumnMappings.Add(4, 5);
                    bcp.ColumnMappings.Add(5, 6);
                    bcp.ColumnMappings.Add(6, 7);
                    bcp.ColumnMappings.Add(7, 8);
                    bcp.WriteToServer(dt);
                }
            }

        }
    }
}

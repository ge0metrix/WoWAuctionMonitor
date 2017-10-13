using System;
using WowDotNetAPI;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

namespace WowAuctionAggrigator
{
    public class AuctionLoader : IDisposable
    {
        private IExplorer _explorer;
        private WowDotNetAPI.Models.Auctions _ah;
        private WowDotNetAPI.Models.AuctionFiles _af;

        private string _connstr;
        private DateTimeOffset _dateTimeOffset;
        private SqlConnection _conn;
        private DataTable _dt;
        bool disposed = false;

        public AuctionLoader(string connstr, string apikey, string realmname)
        {
            _connstr = connstr;
            _conn = new SqlConnection(_connstr);
            _conn.Open();

            _explorer = new WowExplorer(Region.US, Locale.en_US, apikey);
            _af = _explorer.GetAuctionFiles(realmname);
            _ah = _explorer.GetAuctions(realmname);

            _dt = new DataTable();
            _dt.Columns.Add("ID");
            _dt.Columns.Add("ItemID");
            _dt.Columns.Add("Owner");
            _dt.Columns.Add("Bid");
            _dt.Columns.Add("BuyOut");
            _dt.Columns.Add("Quantity");
            _dt.Columns.Add("TimeLeft");
            _dt.Columns.Add("LoadDate");
            _dt.Columns["LoadDate"].DataType = typeof(DateTime);
            _dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(_af.Files.Take(1).Select(x => x.LastModified).ToArray()[0]);
            _dt.Columns["LoadDate"].DefaultValue = _dateTimeOffset.DateTime;

        }

        public void loadDataTabe()
        {
            if(_ah == null)
            {
                //TODO Create a real Exception here!
                throw new Exception();
            }

            DataRow dr;
            int i = 0;
            foreach (var auction in _ah.CurrentAuctions)
            {
                i++;
                dr = _dt.NewRow();
                dr["ID"] = auction.Id;
                dr["ItemID"] = auction.ItemId;
                dr["Owner"] = auction.Owner;
                dr["Bid"] = auction.Bid;
                dr["BuyOut"] = auction.Buyout;
                dr["Quantity"] = auction.Quantity;
                dr["TimeLeft"] = auction.TimeLeft;
                _dt.Rows.Add(dr);
            }
        }

        public void bulkCopyAuctions()
        {
            if(_dt.Rows.Count <= 0)
            {
                throw new Exception();
            }

            using (SqlBulkCopy bcp = new SqlBulkCopy(_conn))
            {
                bcp.BulkCopyTimeout = 0;
                bcp.DestinationTableName = "AuctionRaw";
                bcp.ColumnMappings.Add(0, 1);
                bcp.ColumnMappings.Add(1, 2);
                bcp.ColumnMappings.Add(2, 3);
                bcp.ColumnMappings.Add(3, 4);
                bcp.ColumnMappings.Add(4, 5);
                bcp.ColumnMappings.Add(5, 6);
                bcp.ColumnMappings.Add(6, 7);
                bcp.ColumnMappings.Add(7, 8);
                bcp.WriteToServer(_dt);
            }

        }

        public void clearPriorLoadedData()
        {
            if (_dateTimeOffset == null)
            {
                throw new Exception();
            }
 
            using (SqlCommand cmd = new SqlCommand { Connection = _conn, CommandTimeout = 0 })
            {
                cmd.CommandText = "DELETE FROM AuctionRaw WHERE LoadDate = '" + _dateTimeOffset.DateTime.ToString() + "';";
                cmd.ExecuteNonQuery();
            }
        }


        public void aggAuctionData()
        {
            using (SqlCommand cmd = new SqlCommand { Connection = _conn, CommandTimeout = 0 })
            {
                cmd.CommandText = "EXECUTE [dbo].[AggAuctionData];";
                cmd.ExecuteNonQuery();
            }
        }

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (_conn != null)
                {
                    _conn.Dispose();
                }
            }

            // Free any unmanaged objects here.
            //

            disposed = true;
        }


        //public static void LoadAuctionData()
        //{

        //    var str = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;

        //    WowExplorer explorer = new WowExplorer(Region.US, Locale.en_US, ConfigurationManager.AppSettings.Get("BlizzAPIKey"));
        //    var ah = explorer.GetAuctions("stormrage");
        //    var af = explorer.GetAuctionFiles("stormrage");
        //    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(af.Files.Take(1).Select(x => x.LastModified).ToArray()[0]);

        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("ID");
        //    dt.Columns.Add("ItemID");
        //    dt.Columns.Add("Owner");
        //    dt.Columns.Add("Bid");
        //    dt.Columns.Add("BuyOut");
        //    dt.Columns.Add("Quantity");
        //    dt.Columns.Add("TimeLeft");
        //    dt.Columns.Add("LoadDate");
        //    dt.Columns["LoadDate"].DataType = typeof(DateTime);
        //    dt.Columns["LoadDate"].DefaultValue = dateTimeOffset.DateTime;


        //    DataRow dr;
        //    int i = 0;
        //    using (SqlConnection conn = new SqlConnection(str))
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = new SqlCommand { Connection = conn, CommandTimeout = 0 })
        //        {
        //            cmd.CommandText = "DELETE FROM Auctions WHERE LoadDate = '" + dateTimeOffset.DateTime.ToString() + "';";
        //            cmd.ExecuteNonQuery();
        //        }
        //    }

        //    foreach (var auction in ah.CurrentAuctions)
        //    {
        //        i++;
        //        dr = dt.NewRow();
        //        dr["ID"] = auction.Id;
        //        dr["ItemID"] = auction.ItemId;
        //        dr["Owner"] = auction.Owner;
        //        dr["Bid"] = auction.Bid;
        //        dr["BuyOut"] = auction.Buyout;
        //        dr["Quantity"] = auction.Quantity;
        //        dr["TimeLeft"] = auction.TimeLeft;
        //        dt.Rows.Add(dr);
        //    }
        //    using (SqlConnection conn = new SqlConnection(str))
        //    {
        //        conn.Open();
        //        using (SqlBulkCopy bcp = new SqlBulkCopy(conn))
        //        {
        //            bcp.BulkCopyTimeout = 0;
        //            bcp.DestinationTableName = "Auctions";
        //            bcp.ColumnMappings.Add(0, 1);
        //            bcp.ColumnMappings.Add(1, 2);
        //            bcp.ColumnMappings.Add(2, 3);
        //            bcp.ColumnMappings.Add(3, 4);
        //            bcp.ColumnMappings.Add(4, 5);
        //            bcp.ColumnMappings.Add(5, 6);
        //            bcp.ColumnMappings.Add(6, 7);
        //            bcp.ColumnMappings.Add(7, 8);
        //            bcp.WriteToServer(dt);
        //        }
        //    }

        //}

    }
}

using Mpp.UTILITIES;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.BUSINESS.DataLibrary
{
    /// <summary>
    /// Class to handle all operations related to Dashboard
    /// </summary>
    public class DashboardData
    {
        #region Class Variables and Constants
       
        #endregion

        #region Constructors
        public DashboardData()
        {
            
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Get Campaigns Data by Range Selection
        ///<param name="userID">user ID</param>
        ///<param name="type">type</param>
        ///<param name="range">user ID</param>
        /// </summary>
        /// <returns>Campaigns</returns>
        public List<Campaign> GetCampaignData(Int32 userID)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = @"SELECT c.Name,sum(i.Spend) as Spend,sum(i.Sales) as Sales, SUM(i.Impressions) as Impressions,SUM(i.Clicks) as Clicks, SUM(i.Orders) as Orders from Reports r 
                               INNER JOIN ProductsInventory i on r.ReportID = i.ReportID
                               INNER JOIN Campaigns c on c.RecordID = i.RecordID
                               WHERE i.RecordType = 1 and r.MppUserID=@UserID group by c.Name";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
            dt = DataAccess.GetTable(cmd);
            return dt.AsEnumerable().Select(r => ConvertToCampaign(r)).ToList();
        }

        /// <summary>
        /// Get Keywords Data by Range Selection
        ///<param name="userID">user ID</param>
        ///<param name="type">type</param>
        ///<param name="range">user ID</param>
        /// </summary>
        /// <returns>Keywords</returns>
        public List<Log> GetOptimization(Int32 type , Int32 id , String KeyName ,Int32 direc,Int32 userID)
        {
            DataTable dt = new DataTable();
            
            SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "sbsp_CampPerformance";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
            cmd.Parameters.Add("@KeyName", SqlDbType.Char).Value =  KeyName;
            cmd.Parameters.Add("@order", SqlDbType.Int).Value = direc;
            cmd.Parameters.Add("@CampID", SqlDbType.Int).Value = id;

            dt = DataAccess.GetTable(cmd);
            var log1 = new List<Log>();
            log1= dt.AsEnumerable().Select(r => ConvertToOptimizedData(r,type)).ToList();
            return log1;
        }
       
    public List<Keyword> GetKeywordData(Int32 userID)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = @"SELECT k.Keyword,c.Name as CampaignName,a.Name as AdGroupName, sum(i.Spend) as Spend,sum(i.Sales) as Sales, SUM(i.Impressions) as Impressions,SUM(i.Clicks) as Clicks, SUM(i.Orders) as Orders,
                                CASE WHEN SUM(i.Sales)>0 THEN
                                ROUND(((SUM(i.Spend) / SUM(i.Sales)) * 100),2,1)
                                ELSE '0' END AS ACoS
                                from Reports r 
	                            INNER JOIN ProductsInventory i on r.ReportID = i.ReportID 
	                            INNER JOIN Keywords k on k.RecordID = i.RecordID
	                            INNER JOIN Campaigns c on c.RecordID = k.CampaignID
	                            INNER JOIN AdGroups a on a.RecordID = k.AdGroupID
	                            WHERE i.RecordType = 3 and r.MppUserID=@UserID and k.Status = 'enabled' group by k.Keyword,c.Name,a.Name ";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
            dt = DataAccess.GetTable(cmd);
            return dt.AsEnumerable().Select(r => ConvertToOptimizeCamp(r)).ToList();
        }

        /// <summary>
        /// Get AdGroups Data by Range Selection
        ///<param name="userID">user ID</param>
        ///<param name="type">type</param>
        ///<param name="range">user ID</param>
        /// </summary>
        /// <returns>AdGroups</returns>
        public List<AdGroup> GetAdGroupData(Int32 userID)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = @"SELECT c.Name as CampaignName,a.Name as AdGroupName, sum(i.Spend) as Spend,sum(i.Sales) as Sales, SUM(i.Impressions) as Impressions,SUM(i.Clicks) as Clicks, SUM(i.Orders) as Orders from Reports r 
	                            INNER JOIN ProductsInventory i on r.ReportID = i.ReportID 
	                            INNER JOIN AdGroups a on a.RecordID = i.RecordID
	                            INNER JOIN Campaigns c on c.RecordID = a.CampaignID
	                            WHERE i.RecordType = 2 and r.MppUserID=@UserID group by c.Name,a.Name";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
            dt = DataAccess.GetTable(cmd);
            return dt.AsEnumerable().Select(r => ConvertToAdGroup(r)).ToList();
        }

        /// <summary>
        /// Get Campaigns Data by Custom Range Selection
        ///<param name="userID">user ID</param>
        ///<param name="startDate">start Date</param>
        ///<param name="endDate">end Date</param>
        /// </summary>
        /// <returns>Campaigns</returns>
        public List<Campaign> GetCustCampaignData(Int32 userID, DateTime startDate, DateTime endDate)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();            
            cmd.CommandText = @"SELECT c.Name,sum(i.Spend) as Spend,sum(i.Sales) as Sales, SUM(i.Impressions) as Impressions,SUM(i.Clicks) as Clicks, SUM(i.Orders) as Orders from Reports r 
	                             INNER JOIN ProductsInventory i on r.ReportID = i.ReportID 
	                             INNER JOIN Campaigns c on c.RecordID = i.RecordID
	                             WHERE i.RecordType = 1 and r.MppUserID=@UserID and r.ReportDate BETWEEN @Stdate AND @Endate group by c.Name";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
            cmd.Parameters.Add("@Stdate", SqlDbType.SmallDateTime).Value = startDate.ToShortDateString();
            cmd.Parameters.Add("@Endate", SqlDbType.SmallDateTime).Value = endDate.ToShortDateString();
            dt = DataAccess.GetTable(cmd);
            return dt.AsEnumerable().Select(r => ConvertToCampaign(r)).ToList();
        }

        /// <summary>
        /// Get Keywords Data by Custom Range Selection
        ///<param name="userID">user ID</param>
        ///<param name="startDate">start Date</param>
        ///<param name="endDate">end Date</param>
        /// </summary>
        /// <returns>Keywords</returns>
        public List<Keyword> GetCustKeywordData(Int32 userID, DateTime startDate, DateTime endDate)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = @"SELECT k.Keyword,c.Name as CampaignName,a.Name as AdGroupName, sum(i.Spend) as Spend,sum(i.Sales) as Sales, SUM(i.Impressions) as Impressions,SUM(i.Clicks) as Clicks, SUM(i.Orders) as Orders,
                                CASE WHEN SUM(i.Sales)>0 THEN
                                ROUND(((SUM(i.Spend) / SUM(i.Sales)) * 100),2,1)
                                ELSE '0' END AS ACoS
                                from Reports r 
	                            INNER JOIN ProductsInventory i on r.ReportID = i.ReportID 
	                            INNER JOIN Keywords k on k.RecordID = i.RecordID
	                            INNER JOIN Campaigns c on c.RecordID = k.CampaignID
	                            INNER JOIN AdGroups a on a.RecordID = k.AdGroupID
	                            WHERE i.RecordType = 3 and ReportDate BETWEEN @Stdate AND @Endate and r.MppUserID=@UserID and k.Status = 'enabled' group by k.Keyword,c.Name,a.Name";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
            cmd.Parameters.Add("@Stdate", SqlDbType.SmallDateTime).Value = startDate.ToShortDateString();
            cmd.Parameters.Add("@Endate", SqlDbType.SmallDateTime).Value = endDate.ToShortDateString();
            dt = DataAccess.GetTable(cmd);
            return dt.AsEnumerable().Select(r => ConvertToOptimizeCamp(r)).ToList();
        }


        /// <summary>
        /// Get AdGroups Data by Custom Range Selection
        ///<param name="userID">user ID</param>
        ///<param name="startDate">start Date</param>
        ///<param name="endDate">end Date</param>
        /// </summary>
        /// <returns>AdGroups</returns>
        public List<AdGroup> GetCustAdGroupData(Int32 userID, DateTime startDate, DateTime endDate)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = @"SELECT c.Name as CampaignName,a.Name as AdGroupName, sum(i.Spend) as Spend,sum(i.Sales) as Sales, SUM(i.Impressions) as Impressions,SUM(i.Clicks) as Clicks, SUM(i.Orders) as Orders from Reports r 
	                            INNER JOIN ProductsInventory i on r.ReportID = i.ReportID 
	                            INNER JOIN AdGroups a on a.RecordID = i.RecordID
	                            INNER JOIN Campaigns c on c.RecordID = a.CampaignID
	                            WHERE i.RecordType = 2 and r.MppUserID=@UserID and ReportDate BETWEEN @Stdate AND @Endate group by c.Name,a.Name";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
            cmd.Parameters.Add("@Stdate", SqlDbType.SmallDateTime).Value = startDate.ToShortDateString();
            cmd.Parameters.Add("@Endate", SqlDbType.SmallDateTime).Value = endDate.ToShortDateString();
            dt = DataAccess.GetTable(cmd);
            return dt.AsEnumerable().Select(r => ConvertToAdGroup(r)).ToList();
        }

        /// <summary>
        /// Get Graphs Data 
        ///<param name="userID">user ID</param>
        ///<param name="startDate">start Date</param>
        ///<param name="endDate">end Date</param>
        /// </summary>
        /// <returns>Spend</returns>
        public List<GraphInventoryModel> GetChartData(Int32 userID, DateTime startdate, DateTime endDate)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT r.ReportDate,sum(i.Spend) as Spend,sum(i.Sales) as Sales  ,sum(i.Impressions) as Impressions, sum(i.Clicks) as Click  from Reports r INNER JOIN ProductsInventory i on r.ReportID = i.ReportID WHERE r.MppUserID=@userID and i.RecordType =1 and r.ReportDate BETWEEN @startdate and @endDate group by r.ReportDate";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
            cmd.Parameters.Add("@startdate", SqlDbType.SmallDateTime).Value = startdate.ToShortDateString();
            cmd.Parameters.Add("@endDate", SqlDbType.SmallDateTime).Value = endDate.ToShortDateString();
            DataTable dt = DataAccess.GetTable(cmd);
            return dt.AsEnumerable().Select(r => ConvertToInventory(r)).ToList();
        }

        /// <summary>
        /// Get Spend chart Data 
        ///<param name="userID">user ID</param>
        ///<param name="startDate">start Date</param>
        ///<param name="endDate">end Date</param>
        /// </summary>
        /// <returns>Spend</returns>
        public List<SpendModel> GetSpendChartData(Int32 userID, DateTime startDate, DateTime endDate)
        {
            DataTable dt;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = @"SELECT r.ReportDate,sum(i.Spend) as Spend from Reports r INNER JOIN ProductsInventory i on r.ReportID = i.ReportID WHERE r.MppUserID=@userID and i.RecordType =1 and r.ReportDate BETWEEN @startdate and @endDate group by r.ReportDate";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
            cmd.Parameters.Add("@startDate", SqlDbType.SmallDateTime).Value = startDate.ToShortDateString();
            cmd.Parameters.Add("@endDate", SqlDbType.SmallDateTime).Value = endDate.ToShortDateString();
            dt = DataAccess.GetTable(cmd);
            return dt.AsEnumerable().Select(r => ConvertToSpend(r)).ToList();
        }

        /// <summary>
        /// Get Sale chart Data 
        ///<param name="userID">user ID</param>
        ///<param name="startDate">start Date</param>
        ///<param name="endDate">end Date</param>
        /// </summary>
        /// <returns>Spend</returns>
        public List<SalesModel> GetSaleChartData(Int32 userID, DateTime startDate, DateTime endDate)
        {
            DataTable dt;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = @"SELECT r.ReportDate,sum(i.Sales) as Sales from Reports r INNER JOIN ProductsInventory i on r.ReportID = i.ReportID WHERE r.MppUserID=@userID and i.RecordType =1 and r.ReportDate BETWEEN @startdate and @endDate group by r.ReportDate";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
            cmd.Parameters.Add("@startDate", SqlDbType.SmallDateTime).Value = startDate.ToShortDateString();
            cmd.Parameters.Add("@endDate", SqlDbType.SmallDateTime).Value = endDate.ToShortDateString();
            dt = DataAccess.GetTable(cmd);
            return dt.AsEnumerable().Select(r => ConvertToSale(r)).ToList();
        }

        /// <summary>
        /// Get Acos chart Data 
        ///<param name="userID">user ID</param>
        ///<param name="startDate">start Date</param>
        ///<param name="endDate">end Date</param>
        /// </summary>
        /// <returns>Acos</returns>
        public List<AcosModel> GetAcosChartData(Int32 userID, DateTime startDate, DateTime endDate)
        {
            DataTable dt;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT r.ReportDate,sum(i.Spend) as Spend,sum(i.Sales) as Sales from Reports r INNER JOIN ProductsInventory i on r.ReportID = i.ReportID WHERE r.MppUserID=@userID and i.RecordType =1 and r.ReportDate BETWEEN @startdate and @endDate group by r.ReportDate";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
            cmd.Parameters.Add("@startdate", SqlDbType.SmallDateTime).Value = startDate.ToShortDateString();
            cmd.Parameters.Add("@endDate", SqlDbType.SmallDateTime).Value = endDate.ToShortDateString();
            dt = DataAccess.GetTable(cmd);
            return dt.AsEnumerable().Select(r => ConvertToAcos(r)).ToList();
        }

        /// <summary>
        /// Get Impressions chart Data 
        ///<param name="userID">user ID</param>
        ///<param name="startDate">start Date</param>
        ///<param name="endDate">end Date</param>
        /// </summary>
        /// <returns>Impressions</returns>
        public List<ImpressionModel> GetImpressionsChartData(Int32 userID, DateTime startDate, DateTime endDate)
        {
            DataTable dt;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT r.ReportDate,sum(i.Impressions) as Impressions from Reports r INNER JOIN ProductsInventory i on r.ReportID = i.ReportID WHERE r.MppUserID=@userID and i.RecordType =1 and r.ReportDate BETWEEN @startdate and @endDate group by r.ReportDate";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
            cmd.Parameters.Add("@startdate", SqlDbType.SmallDateTime).Value = startDate.ToShortDateString();
            cmd.Parameters.Add("@endDate", SqlDbType.SmallDateTime).Value = endDate.ToShortDateString();
            dt = DataAccess.GetTable(cmd);
            return dt.AsEnumerable().Select(r => ConvertToImpressions(r)).ToList();
        }
        /// <summary>
        /// Get Clicks chart Data 
        ///<param name="userID">user ID</param>
        ///<param name="startDate">start Date</param>
        ///<param name="endDate">end Date</param>
        /// </summary>
        /// <returns>Clicks</returns>
        public List<ClickModel> GetClicksChartData(Int32 userID, DateTime startDate, DateTime endDate)
        {
            DataTable dt;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT r.ReportDate, sum(i.Clicks) as Clicks from Reports r INNER JOIN ProductsInventory i on r.ReportID = i.ReportID WHERE r.MppUserID=@userID and i.RecordType =1 and r.ReportDate BETWEEN @startdate and @endDate group by r.ReportDate";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
            cmd.Parameters.Add("@startdate", SqlDbType.SmallDateTime).Value = startDate.ToShortDateString();
            cmd.Parameters.Add("@endDate", SqlDbType.SmallDateTime).Value = endDate.ToShortDateString();
            dt = DataAccess.GetTable(cmd);
            return dt.AsEnumerable().Select(r => ConvertToClicks(r)).ToList();
        }

        /// <summary>
        /// Get CTR chart Data 
        ///<param name="userID">user ID</param>
        ///<param name="startDate">start Date</param>
        ///<param name="endDate">end Date</param>
        /// </summary>
        /// <returns>CTR</returns>
        public List<CTRModel> GetCTRChartData(Int32 userID, DateTime startDate, DateTime endDate)
        {
            DataTable dt;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT r.ReportDate, sum(i.Clicks) as Clicks ,sum(i.Impressions) as Impressions  from Reports r INNER JOIN ProductsInventory i on r.ReportID = i.ReportID WHERE r.MppUserID=@userID and i.RecordType =1 and r.ReportDate BETWEEN @startdate and @endDate group by r.ReportDate";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
            cmd.Parameters.Add("@startdate", SqlDbType.SmallDateTime).Value = startDate.ToShortDateString();
            cmd.Parameters.Add("@endDate", SqlDbType.SmallDateTime).Value = endDate.ToShortDateString();
            dt = DataAccess.GetTable(cmd);
            return dt.AsEnumerable().Select(r => ConvertToCTR(r)).ToList();
        }

        /// <summary>
        /// Get CPC chart Data 
        ///<param name="userID">user ID</param>
        ///<param name="startDate">start Date</param>
        ///<param name="endDate">end Date</param>
        /// </summary>
        /// <returns>CPC</returns>
        public List<CPCModel> GetCPCChartData(Int32 userID, DateTime startDate, DateTime endDate)
        {
            DataTable dt;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT r.ReportDate,sum(i.Spend) as Spend,sum(i.Clicks) as Clicks from Reports r INNER JOIN ProductsInventory i on r.ReportID = i.ReportID WHERE r.MppUserID=@userID and i.RecordType =1 and r.ReportDate BETWEEN @startdate and @endDate group by r.ReportDate";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
            cmd.Parameters.Add("@startdate", SqlDbType.SmallDateTime).Value = startDate.ToShortDateString();
            cmd.Parameters.Add("@endDate", SqlDbType.SmallDateTime).Value = endDate.ToShortDateString();
            dt = DataAccess.GetTable(cmd);
            return dt.AsEnumerable().Select(r => ConvertToCPC(r)).ToList();
        }
        /// <summary>
        /// Get Alerts Data 
        ///<param name="userID">user ID</param>
        /// </summary>
        /// <returns>Alerts</returns>
        public List<Alert> GetAlertData(Int32 userID)
        {
            DataTable dt;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "select ul.Id,ul.AlertDate,al.Description from UserAlerts as ul inner join Alerts as al on ul.AlertID=al.AlertID where ul.MppUserId = @userID";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
            dt = DataAccess.GetTable(cmd);
            List<Alert> list = dt.AsEnumerable().Select(r => ConvertToAlertType(r)).ToList();
            return list;
        }

        /// <summary>
        /// Clear Alerts Data 
        ///<param name="userID">user ID</param>
        /// </summary>
        /// <returns>Alerts</returns>
        public String DeleteAlertData(Int32 AlertID)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "delete from UserAlerts where Id = @alertID";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@alertID", SqlDbType.Int).Value = AlertID;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Delete AlertData - " + AlertID + ": " + msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Delete AlertData - " + AlertID + ": " + msg);
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
            return msg;

        }

        #endregion

        #region Private Methods
        private Campaign ConvertToCampaign(DataRow row)
        {
            return new Campaign()
            {
                CampaignName = row.Field<String>("Name"),
                Impressions = row.Field<Int32>("Impressions"),
                Clicks = row.Field<Int32>("Clicks"),
                Spend = row.Field<Decimal>("Spend"),
                Orders = row.Field<Int32>("Orders"),
                Sales = row.Field<Decimal>("Sales"),
                ACoS = row.Field<Decimal>("Sales") != 0 ? System.Math.Round(((row.Field<Decimal>("Spend") / row.Field<Decimal>("Sales")) * 100), 2, MidpointRounding.AwayFromZero) : 0,
                CTR = row.Field<Int32>("Impressions") != 0 ? System.Math.Round(((Decimal)row.Field<Int32>("Clicks") / row.Field<Int32>("Impressions")) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                CostPerClick = row.Field<Int32>("Clicks") != 0 ? System.Math.Round(row.Field<Decimal>("Spend") / row.Field<Int32>("Clicks"),2, MidpointRounding.AwayFromZero) : 0
            };
        }

        private Keyword ConvertToOptimizeCamp(DataRow row)
        {
            return new Keyword()
            {
                KeywordName = row.Field<String>("Keyword"),
                CampaignName = row.Field<String>("CampaignName"),
                AdGroupName = row.Field<String>("AdGroupName"),
                Impressions = row.Field<Int32>("Impressions"),
                Clicks = row.Field<Int32>("Clicks"),
                Spend = row.Field<Decimal>("Spend"),
                Orders = row.Field<Int32>("Orders"),
                Sales = row.Field<Decimal>("Sales"),
                ACoS = row.Field<Decimal>("ACoS"), /*!= 0 ? System.Math.Round(((row.Field<Decimal>("Spend") / row.Field<Decimal>("Sales")) * 100), 2, MidpointRounding.AwayFromZero): 0,*/
                CTR = row.Field<Int32>("Impressions") != 0 ? System.Math.Round(((Decimal)row.Field<Int32>("Clicks")/ row.Field<Int32>("Impressions")) * 100, 2,MidpointRounding.AwayFromZero):0,
                CostPerClick = row.Field<Int32>("Clicks") != 0 ? System.Math.Round(row.Field<Decimal>("Spend") / row.Field<Int32>("Clicks"), 2, MidpointRounding.AwayFromZero) : 0
                //CostPerClick = Convert.ToDecimal(row.Field<Int32>("Impressions") / row.Field<Int32>("Clicks") == 0 ? 1: row.Field<Int32>("Clicks"))
            };
        }
        private Log ConvertToOptimizedData(DataRow row, int type)
        {
            return new Log()
            {
                CampID = (type == 0) ? row.Field<Int64>("CiD") : 0,
                CampaignName = (type == 0) ? row.Field<String>("campaign") : "",
                KeywordName = (type == 1) ? row.Field<String>("Keyword") : "",
                MatchType = (type != 0) ? row.Field<String>("MatchType") : "",
                ModifiedField = (type != 0) ? row.Field<String>("ModifiedField") : "",
                ModifiedOn = MppUtility.FullDateString(row.Field<DateTime>("Date")),
                OldValue = (type != 0) ? row.Field<String>("OldValue") : "",
                NewValue = (type != 0) ? row.Field<String>("NewValue") : "",

                //ReportID = (type != 0) ? row.Field<Int32>("ReportID"):0,
                ReasonID = (type != 0) ? row.Field<Int32>("ReasonID") : 0,



                //AdGroupName = (type != 0) ? row.Field<String>("AdGroupName"):"",
                //Reason = (type != 0) ? row.Field<String>("Reasondesc"):""
            };
        }
        private AdGroup ConvertToAdGroup(DataRow row)
        {
            return new AdGroup()
            {
                AdGroupName = row.Field<String>("AdGroupName"),
                Campaign  = row.Field<String>("CampaignName"),
                Impressions = row.Field<Int32>("Impressions"),
                Clicks = row.Field<Int32>("Clicks"),
                Spend = row.Field<Decimal>("Spend"),
                Orders = row.Field<Int32>("Orders"),
                Sales = row.Field<Decimal>("Sales"),
                ACoS = row.Field<Decimal>("Sales") != 0 ? System.Math.Round(((row.Field<Decimal>("Spend") / row.Field<Decimal>("Sales")) * 100), 2, MidpointRounding.AwayFromZero) : 0,
                CTR = row.Field<Int32>("Impressions") != 0 ? System.Math.Round(((Decimal)row.Field<Int32>("Clicks") / row.Field<Int32>("Impressions")) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                CostPerClick = row.Field<Int32>("Clicks") != 0 ? System.Math.Round(row.Field<Decimal>("Spend") / row.Field<Int32>("Clicks"), 2, MidpointRounding.AwayFromZero) : 0
                //CostPerClick = Convert.ToDecimal(row.Field<Int32>("Impressions") / row.Field<Int32>("Clicks") == 0 ? 1: row.Field<Int32>("Clicks"))
            };
        }

        private SpendModel ConvertToSpend(DataRow row)
        {
            return new SpendModel()
            {
                ReportDay = row.Field<DateTime>("ReportDate"),
                Spend = System.Math.Round(row.Field<Decimal>("Spend"),2, MidpointRounding.AwayFromZero),
            };
        }

        private SalesModel ConvertToSale(DataRow row)
        {
            return new SalesModel()
            {
                ReportDay = row.Field<DateTime>("ReportDate"),
                Sales = System.Math.Round(row.Field<Decimal>("Sales"),2, MidpointRounding.AwayFromZero),
            };
        }
        private AcosModel ConvertToAcos(DataRow row)
        {
            return new AcosModel()
            {
                ReportDay = row.Field<DateTime>("ReportDate"),
                Sales = System.Math.Round(row.Field<Decimal>("Sales"), 2, MidpointRounding.AwayFromZero),
                Spend = System.Math.Round(row.Field<Decimal>("Spend"), 2, MidpointRounding.AwayFromZero),
            };
        }
        private ImpressionModel ConvertToImpressions(DataRow row)
        {
            return new ImpressionModel()
            {
                ReportDay = row.Field<DateTime>("ReportDate"),
                Impressions  = row.Field<Int32>("Impressions")
               
            };
        }
        private ClickModel ConvertToClicks(DataRow row)
        {
            return new ClickModel()
            {
                ReportDay = row.Field<DateTime>("ReportDate"),
                Clicks = row.Field<Int32>("Clicks")

            };
        }
        private CTRModel ConvertToCTR(DataRow row)
        {
            return new CTRModel()
            {
                ReportDay = row.Field<DateTime>("ReportDate"),
                Clicks  = row.Field<Int32>("Clicks"),
                Impressions=row.Field<Int32>("Impressions")                
            };
        }

        private CPCModel ConvertToCPC(DataRow row)
        {
            return new CPCModel()
            {
                ReportDay = row.Field<DateTime>("ReportDate"),
                Clicks = row.Field<Int32>("Clicks"),
                Spend = row.Field<decimal>("Spend")
            };
        }

        private GraphInventoryModel ConvertToInventory(DataRow row)
        {
            return new GraphInventoryModel()
            {
                ReportDay = row.Field<DateTime>("ReportDate"),
                Spend = System.Math.Round(row.Field<Decimal>("Spend"), 2, MidpointRounding.AwayFromZero),
                Sales = System.Math.Round(row.Field<Decimal>("Sales"), 2, MidpointRounding.AwayFromZero),
                Impressions=row.Field<Int32>("Impressions"),
                Click=row.Field<Int32>("Click")
            };
        }

        private Alert ConvertToAlertType(DataRow row)
        {
            return new Alert()
            {
                AlertID = row.Field<Int32>("Id"),
                AlertDate = row.Field<DateTime>("AlertDate").ToShortDateString(),
                Description = row.Field<String>("Description")
            };
        }

        private String ReturnCampQuery(Int32 range)
        {
            String query = "";
            if (range == 30)
                query = @"select * from CampaignThirtyDay where MppUserID=@UserID";
            else if (range == 60)
                query = @"select * from CampaignSixtyDay where MppUserID=@UserID";
            else
                query = @"SELECT CampaignName,SUM(Impressions) as Impressions,SUM(Clicks) as Clicks, SUM(Spend) as Spend, SUM(Orders) as Orders, SUM(Sales) as Sales FROM CampaignOneDay where ReportID in 
                              (select ReportID FROM MppReports where MppUserID=@UserID and Days =1) group by CampaignName";
            return query;
        }

        private String ReturnKeyQuery(Int32 range)
        {
            String query = "";
            if (range == 30)
                query = @"select * from KeywordThirtyDay where MppUserID=@UserID and Status ='enabled'";
            else if (range == 60)
                query = @"select * from KeywordSixtyDay where MppUserID=@UserID and Status = 'enabled'";
            else
                query = @"SELECT RecordID,Keyword,CampaignName,AdGroupName,SUM(Impressions) as Impressions,SUM(Clicks) as Clicks, SUM(Spend) as Spend, SUM(Orders) as Orders, SUM(Sales) as Sales FROM KeywordOneDay where ReportID in 
                              (select ReportID FROM MppReports where MppUserID=@UserID and Days =1) and Status = 'enabled' group by RecordID, Keyword, CampaignName, AdGroupName";
            return query;
        }

        private String ReturnAdGroupQuery(Int32 range)
        {
            String query = "";
            if (range == 30)
                query = @"select * from AdGroupThirtyDay where MppUserID=@UserID";
            else if (range == 60)
                query = @"select * from AdGroupSixtyDay where MppUserID=@UserID";
            else
                query = @"SELECT RecordID,AdGroupName,CampaignName,SUM(Impressions) as Impressions,SUM(Clicks) as Clicks, SUM(Spend) as Spend, SUM(Orders) as Orders, SUM(Sales) as Sales FROM AdGroupOneDay where ReportID in 
                              (select ReportID FROM MppReports where MppUserID=@UserID and Days =1) group by RecordID, AdGroupName, CampaignName";
            return query;
        }
        #endregion
    }
}

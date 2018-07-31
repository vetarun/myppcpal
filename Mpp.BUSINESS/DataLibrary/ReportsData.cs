using Mpp.UTILITIES;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Mpp.BUSINESS.DataLibrary
{
    public class ReportsData
    {
        #region Class Variables and Constants

        #endregion

        #region Constructors

        #endregion

        #region Public Methods

        /// <summary>
        /// Get ReportsOUT details
        ///<param name="UserID">UserID</param>
        /// </summary>
        /// <returns></returns>
        public List<Report> GetReportsOutData(Int32 userID)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Select ReportStartDate, ReportEndDate, UploadDate as ReportDate from MppReports where MppUserID=@MppUserId and UploadStatus=2 order by UploadDate desc";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@MppUserId", SqlDbType.Int).Value = userID;
            dt = DataAccess.GetTable(cmd);
            return dt.AsEnumerable().Select(r => ConvertToReport(r)).ToList();
        }

        /// <summary>
        /// Get ReportsIN details
        ///<param name="UserID">UserID</param>
        /// </summary>
        /// <returns></returns>
        public List<Report> GetReportsInData(Int32 userID)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Select ReqReportDate as ReportDate,ReportStartDate, ReportEndDate from MppReports where MppUserID=@MppUserId and days=60 and DwnldStatus=3 order by ReqReportDate desc";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@MppUserId", SqlDbType.Int).Value = userID;
            dt = DataAccess.GetTable(cmd);
            return dt.AsEnumerable().Select(r => ConvertToReport(r)).ToList();
        }

        /// <summary>
        /// Download File
        ///<param name="strtdate">strtdate</param>
        ///<param name="enddate">enddate</param>
        /// </summary>
        /// <returns></returns>


        /// <summary>
        /// Upload File
        ///<param name="strtdate">strtdate</param>
        ///<param name="enddate">enddate</param>
        /// </summary>
        /// <returns></returns>
        public String UploadFileData(String strtdate, String enddate)
        {
            String msg = "";
            try
            {
                Int32 userid = SessionData.UserID;
                String uploadFileDir = ConfigurationManager.AppSettings["UploadFolderPath"];
                string uploadfilepath = MppUtility.GetFilelocation(userid, uploadFileDir, "bulk");
                string fileName = ConfigurationManager.AppSettings["filename"];
                string fileName1 = fileName.PadRight(29) + strtdate + "-" + enddate + ".csv";
                string filePath = Path.Combine(uploadfilepath, fileName1);
                if (File.Exists(filePath) == true)
                {
                    HttpContext context = HttpContext.Current;
                    context.Response.Clear();
                    context.Response.ContentType = "text/csv";
                    context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName1);
                    context.Response.WriteFile(filePath);
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }

        public List<object> GetReportDates(int userId)
        {
         
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sbsp_GetUserReportDates";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
            ds = DataAccess.GetDataSet(cmd);
            return GetDates(ds);
        }


        public DataSet GetReports(int userId,DateTime date,int type)
        {
            DataSet ds = new DataSet();
            try
            {
              
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Sbsp_GetUserReportData";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@date", SqlDbType.SmallDateTime).Value = date.ToShortDateString();
                cmd.Parameters.Add("@type", SqlDbType.Int).Value = type;
                ds = DataAccess.GetDataSet(cmd);
                return ds;
                //return dt.AsEnumerable().Select(r => ConvertToKeyword(r)).ToList();
            }
            catch(Exception ex)
            {
                return ds;
            }
        }


        #endregion

        #region Private Methods
        private Report ConvertToReport(DataRow row)
        {
            return new Report()
            {
                ReportStartDate = row.Field<DateTime>("ReportStartDate"),
                ReportEndDate = row.Field<DateTime>("ReportEndDate"),
                ReportDate = row.Field<DateTime>("ReportDate")
            };
        }
        private List<object> GetDates(DataSet ds)
        {
            List<object> objList = new List<object>();
            if (ds != null && ds.Tables.Count > 0)
            {
                
               
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    List<object> KeyBulk = new List<object>();
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        KeyBulk.Add(new { Date = Convert.ToDateTime(row["KeyBulkDates"]).ToString("MM/dd/yyyy") });
                    }
                    objList.Add(new {  Dates = KeyBulk });
                }
                else
                {
                    objList.Add(new {Dates = "" });
                }
                if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                {
                    List<object> Stbulk = new List<object>();
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        Stbulk.Add(new { Date = Convert.ToDateTime(row["StermBulkDates"]).ToString("MM/dd/yyyy") });
                    }
                    objList.Add(new {Dates = Stbulk });
                }
                else
                {
                    objList.Add(new {Dates = "" });
                }
                if (ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                {
                    List<object> Stbulk = new List<object>();
                    foreach (DataRow row in ds.Tables[2].Rows)
                    {
                        Stbulk.Add(new { Date = Convert.ToDateTime(row["KeyUpload"]).ToString("MM/dd/yyyy") });
                    }
                    objList.Add(new {Dates = Stbulk });
                }
                else
                {
                    objList.Add(new {Dates = "" });
                }
                if (ds.Tables[3] != null && ds.Tables[3].Rows.Count > 0)
                {
                    List<object> Stbulk = new List<object>();
                    foreach (DataRow row in ds.Tables[3].Rows)
                    {
                        Stbulk.Add(new { Date = Convert.ToDateTime(row["StermUpload"]).ToString("MM/dd/yyyy") });
                    }
                    objList.Add(new {Dates = Stbulk });
                }
                else
                {
                    objList.Add(new {Dates = "" });
                }
            }
            return objList;
        }

        private KeywordReport ConvertToKeyword(DataRow row)
        {
            return new KeywordReport()
            {
                KeywordName = row.Field<String>("Keyword"),
                CampaignName = row.Field<String>("CampaignName"),
                AdGroupName = row.Field<String>("AdGroupName"),
                Impressions = row.Field<Int32>("Impressions"),
                Clicks = row.Field<Int32>("Clicks"),
                Spend = row.Field<Decimal>("Spend"),
                Orders = row.Field<Int32>("Orders"),
                Sales = row.Field<Decimal>("Sales"),
                ACoS = row.Field<Decimal>("Sales") != 0 ? System.Math.Round(((row.Field<Decimal>("Spend") / row.Field<Decimal>("Sales")) * 100), 2, MidpointRounding.AwayFromZero) : 0,
                //CTR = row.Field<Int32>("Impressions") != 0 ? System.Math.Round(((Decimal)row.Field<Int32>("Clicks") / row.Field<Int32>("Impressions")) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                //CostPerClick = row.Field<Int32>("Clicks") != 0 ? System.Math.Round(row.Field<Decimal>("Spend") / row.Field<Int32>("Clicks"), 2, MidpointRounding.AwayFromZero) : 0
                ////CostPerClick = Convert.ToDecimal(row.Field<Int32>("Impressions") / row.Field<Int32>("Clicks") == 0 ? 1: row.Field<Int32>("Clicks"))
                CampaignDailyBudget = row.Field<Decimal>("DailyBudget"),
                CampaignTargetingType= row.Field<String>("TargetingType"),
                Max_Bid = row.Field<Decimal>("Bid"),
                MatchType=row.Field<string>("MatchType"),
                CampaignStatus =row.Field<string>("CampaignStatus"),
                KeyStatus=row.Field<string>("Keyword"),
                AdGroupStatus = row.Field<string>("AddStatus")

            };
        }
        #endregion
    }
}

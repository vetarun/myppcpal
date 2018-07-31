using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.BUSINESS.DataLibrary
{
    public class UploadData
    {
        #region Class Variables and Constants

        #endregion

        #region Constructors

        #endregion

        #region Public Methods

        /// <summary>
        /// Get Seller details
        /// </summary>
        /// <returns></returns>
        public DataTable GetSellerUploadData()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT mp.MppUserID, ri.ReportID ,ri.Days, ReportStartDate,ReportEndDate, NegDwnldStatus from MppUser as mp inner join MppReports as ri on mp.MppUserID = ri.MppUserID where mp.PlanStatus = 1 and mp.SellerAccess=1 and ri.DwnldStatus=2 and ri.Days=60 and ri.UploadStatus=0 and ri.ReqReportDate=@Date";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = DateTime.Today;
            DataTable dt = DataAccess.GetTable(cmd);
            return dt;
        }

        /// <summary>
        /// Get Campaign, Formual details
        /// </summary>
        /// <param name="ReportID">ReportID</param>
        /// <returns></returns>
        public DataTable GetCampaignFormulaData(Int32 SellerID, Int32 ReportID)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = @"select ky.RecordID,ky.Sales,ky.Orders,ky.Spend,ky.Clicks,ky.Impressions,ky.ACoS,ky.MaxBid,ky.Status, fm.AcosPause,
                                fm.AcosLower,fm.AcosRaise,fm.AcosNegative,fm.SpendPause,fm.SpendLower,fm.SpendNegative,fm.ClicksPause,fm.ClicksLower,
                                fm.ClicksNegative,fm.CTRPause,fm.CTRLower,fm.CTRNegative,fm.BidRaise,fm.BidLower,fm.MinBid,fm.MaxBid from CampaignSixtyDay as cm 
                                join KeywordSixtyDay as ky on cm.CampaignName = ky.CampaignName join formula as fm on cm.FormulaID=fm.FormulaID where cm.active=1 and cm.MppUserID=@UserID and cm.ReportID=@ReportID";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = SellerID;
            cmd.Parameters.Add("@ReportID", SqlDbType.Int).Value = ReportID;
            dt = DataAccess.GetTable(cmd);
            return dt;
        }

        /// <summary>
        /// Get Negative keywords, Formual details
        /// </summary>
        /// <param name="ReportID">ReportID</param>
        /// <returns></returns>
        public DataTable GetNegKeywordFormulaData(Int32 SellerID, Int32 ReportID)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = @"select ky.KeyID,ky.MatchType,ky.ACoS,ky.CTR,ky.Spend,ky.Sales,ky.Clicks,ky.Orders,fm.AcosNegative,fm.ClicksNegative,fm.SpendNegative,fm.CTRNegative from CampaignSixtyDay as cm join KeywordNegative as ky on cm.CampaignName = ky.CampaignName 
                              join formula as fm on cm.FormulaID=fm.FormulaID where cm.active=1 and ky.ReportID=@ReportID and cm.MppUserID=@UserID and ky.MppUserID = @UserID";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = SellerID;
            cmd.Parameters.Add("@ReportID", SqlDbType.Int).Value = ReportID;
            dt = DataAccess.GetTable(cmd);
            return dt;
        }

        /// <summary>
        /// Update keywords bids
        ///<param name="tbl">tbl</param>
        /// </summary>
        /// <returns></returns>
        public String UpdateKeyBidData(DataTable tbl)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"Sbsp_UpdateKeyBid";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tblKeybid", tbl);
                DataAccess.ExecuteCommand(cmd);
                return res;
            }
            catch (SqlException ex)
            {
                res = ex.Message;
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
            return res;
        }

        /// <summary>
        /// Update Negative keywords bids
        ///<param name="tbl">tbl</param>
        /// </summary>
        /// <returns></returns>
        public String UpdateNegKeyBidData(DataTable tbl)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"Sbsp_UpdateNegKeyBid";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tblKeybid", tbl);
                DataAccess.ExecuteCommand(cmd);
                return res;
            }
            catch (SqlException ex)
            {
                res = ex.Message;
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
            return res;
        }

        /// <summary>
        /// Get 
        ///<param name="ReportID">ReportID</param>
        /// </summary>
        /// <returns></returns>
        public DataSet GetCsvData(Int32 SellerID, Int32 ReportID)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sbsp_GetUploadCSVData";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ReportID", ReportID);
            cmd.Parameters.AddWithValue("@SellerID", SellerID);
            ds = DataAccess.GetDataSet(cmd);
            return ds;
        }

        /// <summary>
        ///Add or Update ReportOut
        ///<param name="tbl">tbl</param>
        /// </summary>
        /// <returns></returns>
        public String UpdateUploadStatus(Int32 reportID, Int32 status)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"UPDATE MppReports set UploadStatus=@Status,UploadDate=@UpldDate,ModifiedOn=@ModifiedOn where ReportID=@ReportID";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@ReportId", SqlDbType.Int).Value = reportID;
                cmd.Parameters.AddWithValue("@Status", SqlDbType.Int).Value = status;
                cmd.Parameters.AddWithValue("@UpldDate", SqlDbType.SmallDateTime).Value = DateTime.Today.AddDays(1);
                cmd.Parameters.AddWithValue("@ModifiedOn", SqlDbType.DateTime).Value = DateTime.Now;
                DataAccess.ExecuteCommand(cmd);
                return res;
            }
            catch (SqlException ex)
            {
                res = ex.Message;
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
            return res;
        }

        #endregion

        #region Private Methods
        #endregion
    }
}


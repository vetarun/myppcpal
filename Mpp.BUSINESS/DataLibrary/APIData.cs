using Mpp.BUSINESS.DataModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mpp.UTILITIES.Statics;

namespace Mpp.BUSINESS.DataLibrary
{
    public class APIData
    {
        /// <summary>
        /// Set Report Dates
        /// </summary>
        /// <returns></returns>
        /// 
       
        public static String SetOldReportDatesData()
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_SetReportDates";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@enddate", SqlDbType.SmallDateTime).Value = DateTime.Now.Date.AddDays(-3);
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogAPI.WriteLog("UpdateProfileAndAccessTokenData - " + ex.Message);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogAPI.WriteLog("UpdateProfileAndAccessTokenData - " + msg);
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

        /// <summary>
        /// Update user profile and adevertising token
        ///<param name="ProfileId">ProfileId</param>
        ///<param name="AccessToken">AccessToken</param>
        ///<param name="RefreshToken">RefreshToken</param>
        ///<param name="TokenType">TokenType</param>
        ///<param name="ExpiresIn">ExpiresIn</param>
        /// </summary>
        /// <returns></returns>
        public static String UpdateAccessTokenData(int UserID, String AccessToken, String RefreshToken, String TokenType, int ExpiresIn)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_UpdateAccessToken";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@mppUserId", SqlDbType.Int).Value = UserID;
                cmd.Parameters.Add("@accessToken", SqlDbType.VarChar).Value = AccessToken;
                cmd.Parameters.Add("@refreshToken", SqlDbType.VarChar).Value = RefreshToken;
                cmd.Parameters.Add("@tokenType", SqlDbType.VarChar, 20).Value = TokenType;
                cmd.Parameters.Add("@countryCode", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("@expiresIn", SqlDbType.Int).Value = ExpiresIn;
                cmd.Parameters.Add("@tokendate", SqlDbType.SmallDateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@date", SqlDbType.SmallDateTime).Value = DateTime.Now.Date.AddDays(-3);
                DataAccess.ExecuteCommand(cmd);
            }

            catch (Exception ex)
            {
                msg = ex.Message;
                LogAPI.WriteLog("UpdateAccessTokenData- MppUserID" + UserID + ": " + msg);
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

        /// <summary>
        /// Update user profile and adevertising token
        ///<param name="ProfileId">ProfileId</param>
        ///<param name="AccessToken">AccessToken</param>
        ///<param name="RefreshToken">RefreshToken</param>
        ///<param name="TokenType">TokenType</param>
        ///<param name="ExpiresIn">ExpiresIn</param>
        /// </summary>
        /// <returns></returns>
        public static String UpdateProfileData(int UserID, string ProfileId, String SellerStringId)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_UpdateSellerProfile";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@mppUserId", SqlDbType.Int).Value = UserID;
                cmd.Parameters.Add("@profileId", SqlDbType.VarChar, 100).Value = ProfileId;
                cmd.Parameters.Add("@sellerStringId", SqlDbType.VarChar, 100).Value = SellerStringId;
                cmd.Parameters.Add("@modifiedby", SqlDbType.VarChar, 30).Value = "User";
                cmd.Parameters.Add("@date", SqlDbType.SmallDateTime).Value = DateTime.Now.Date.AddDays(-3);
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogAPI.WriteLog("UpdateProfileData-Profile ID" + ProfileId + ": " + ex.Message);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogAPI.WriteLog("UpdateProfileData-Profile ID" + ProfileId + ": " + msg);
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

        /// <summary>
        /// Get the sellers to update their products
        ///<param name="Type">Type</param>
        /// </summary>
        /// <returns></returns>
        public static DataTable GetSnapShotData(ReportStatus? Type, int MaxCount)
        {
            String msg = "";
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_SnapShotManagement";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@date", SqlDbType.SmallDateTime).Value = DateTime.Now.Date.AddDays(-3);
                cmd.Parameters.AddWithValue("@type", SqlDbType.Int).Value = (int)Type;
                cmd.Parameters.AddWithValue("@count", SqlDbType.Int).Value = MaxCount;
                dt = DataAccess.GetTable(cmd);
            }

            catch (Exception ex)
            {
                msg = ex.Message;
                LogAPI.WriteLog("GetSnapShotData - " + msg);
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
            return dt;
        }

        /// <summary>
        /// Get the sellers to update their inventory
        ///<param name="Type">Type</param>
        /// </summary>
        /// <returns></returns>
        public static DataTable GetReportData(ReportStatus? Type, int MaxCount)
        {

            String msg = "";
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_ReportManagement";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@date", SqlDbType.SmallDateTime).Value = DateTime.Now.Date.AddDays(-3);
                cmd.Parameters.AddWithValue("@type", SqlDbType.Int).Value = (int)Type;
                cmd.Parameters.AddWithValue("@count", SqlDbType.Int).Value = MaxCount;
                dt = DataAccess.GetTable(cmd);
            }

            catch (Exception ex)
            {
                msg = ex.Message;
                LogAPI.WriteLog("GetReportData - " + msg);
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
            return dt;
        }

        /// <summary>
        /// Get users for plan sku count
        /// </summary>
        /// <returns></returns>
        public static DataTable GetUserProfileForSkuData(int UserID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT ProfileId, AccessToken, RefreshToken, TokenType, AccessTokenUpdatedOn, TokenExpiresIn from MppUser where MppUserID = @UserID  and ProfileAccess=1 ";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
            DataTable dt = DataAccess.GetTable(cmd);
            return dt;
        }
        public static DataTable GetUserProfileForUpdate(int UserID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT ProfileId, AccessToken, RefreshToken, TokenType, AccessTokenUpdatedOn, TokenExpiresIn from MppUser where MppUserID = @UserID";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
            DataTable dt = DataAccess.GetTable(cmd);
            return dt;
        }


        public static DataTable GetUserProfile(int UserID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT ProfileId, AccessToken, RefreshToken, TokenType, AccessTokenUpdatedOn, TokenExpiresIn from MppUser where MppUserID = @UserID";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
            DataTable dt = DataAccess.GetTable(cmd);
            return dt;
        }

        /// <summary>
        /// Get users to optimize keywords data
        /// </summary>
        /// <returns></returns>
        public static DataTable GetUpldUsersForKeywordOptimize()
        {
            SqlCommand cmd = new SqlCommand();
            //cmd.CommandText = "SELECT top 5 m.MppUserID, r.ReportID from MppUser as m inner join Reports as r on m.MppUserID = r.MppUserID join Keywords as k on k.MppUserID = m.MppUserID where m.ProfileAccess != 0 and m.PlanStatus != 0 and r.UpdtBidStatus = 0 and cast(dateadd(day,30,ISNULL(ManuallyChangedOn,DATEADD(YEAR,-10,GETDATE()))) as date) < cast(getdate() as date) and r.IsLocallyProcessed = 0 and r.ReportDate = @date";
            cmd.CommandText = "SELECT top 5000 m.MppUserID, r.ReportID from MppUser as m inner join Reports as r on m.MppUserID = r.MppUserID where m.ProfileAccess != 0 and m.PlanStatus != 0 and r.UpdtBidStatus = 0 and r.IsLocallyProcessed = 0 and r.ReportDate = @date"; //--Top 5 replace to 5000 by Tarun
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@Date", SqlDbType.SmallDateTime).Value = DateTime.Today.AddDays(-3);
            DataTable dt = DataAccess.GetTable(cmd);
            return dt;
        }

        /// <summary>
        /// Get Users to optimize search term data
        /// </summary>
        /// <returns></returns>
        public static DataTable GetUpldUsersForSearchTermOptimize()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT top 5 m.MppUserID, r.ReportID from MppUser as m inner join Reports as r on m.MppUserID = r.MppUserID where m.ProfileAccess !=0 and m.PlanStatus!=0 and r.UpdtNegStatus = 0 and IsLocallyProcessedSearchTerm = 0 and r.ReportDate=@date";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@Date", SqlDbType.SmallDateTime).Value = DateTime.Today.AddDays(-3);
            DataTable dt = DataAccess.GetTable(cmd);
            return dt;
        }

        /// <summary>
        /// Get Active Campaigns formula 
        ///<param name="UserId">UserId</param>
        /// </summary>
        /// <returns></returns>
        public static DataSet GetOptimizeKeywordsAndFormula(int UserId)
        {
            String msg = "";
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_GetOptimizeKeywordsAndFormula";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", SqlDbType.Int).Value = UserId;
                cmd.Parameters.AddWithValue("@enddate", SqlDbType.SmallDateTime).Value = DateTime.Now.Date.AddDays(-3);
                ds = DataAccess.GetDataSetForBulk(cmd);
            }

            catch (Exception ex)
            {
                msg = ex.Message;
                LogAPI.WriteLog("GetOptimizeKeywordsAndFormula - " + msg, UserId.ToString());
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
            return ds;
        }

        /// <summary>
        /// Get Active Campaigns formula 
        ///<param name="UserId">UserId</param>
        /// </summary>
        /// <returns></returns>
        public static DataSet GetOptimizeSearchTermsAndFormula(int UserId)
        {
            String msg = "";
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_GetOptimizeSearchTermsAndFormula";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", SqlDbType.Int).Value = UserId;
                cmd.Parameters.AddWithValue("@enddate", SqlDbType.SmallDateTime).Value = DateTime.Now.Date.AddDays(-3);
                ds = DataAccess.GetDataSetForBulk(cmd);
            }

            catch (Exception ex)
            {
                msg = ex.Message;
                LogAPI.WriteLog("GetOptimizeSearchTermsAndFormula - " + msg, UserId.ToString());
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
            return ds;
        }

        /// <summary>
        /// Get optimized data to update on seller accounts
        ///<param name="Type">Type</param>
        /// </summary>
        /// <returns></returns>
        /// 
        public static DataSet UpdateChanges(Int64 KeyID, Int64 CID, Int64 AID, Int32 RID, Int32 RSNID, Int32 userID)
        {

            String msg = "";
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_GetUpdatedData";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = userID;
                cmd.Parameters.Add("@ReportID", SqlDbType.Int).Value = RID;
                cmd.Parameters.Add("@ReasonID", SqlDbType.Int).Value = RSNID;
                cmd.Parameters.Add("@CID", SqlDbType.BigInt).Value = CID;
                cmd.Parameters.Add("@AID", SqlDbType.BigInt).Value = AID;
                cmd.Parameters.Add("@KeyID", SqlDbType.BigInt).Value = KeyID;     
                ds = DataAccess.GetDataSet(cmd);

            }

            catch (SqlException ex)
            {
                LogFile.WriteLog(ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex.Message);
                return null;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
            return ds;
        }
        public static DataSet GetUpdatedData (Int64 KeyID, Int64 CID, Int64 AID, Int32 RID, Int32 RSNID, DateTime ModifiedOn, Int32 UserID)
        {
            String msg = "";
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "[Sbsp_GetUpdatedData]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                cmd.Parameters.Add("@ModifiedOn", SqlDbType.DateTime).Value = ModifiedOn;
                cmd.Parameters.Add("@ReportID", SqlDbType.Int).Value = RID;
                cmd.Parameters.Add("@ReasonID", SqlDbType.Int).Value = RSNID;
                cmd.Parameters.Add("@KeyID", SqlDbType.BigInt).Value = KeyID;
                cmd.Parameters.Add("@CID", SqlDbType.BigInt).Value = CID;
                cmd.Parameters.Add("@AID", SqlDbType.BigInt).Value = AID;
                ds = DataAccess.GetDataSet(cmd);
            }

            catch (Exception ex)
            {
                msg = ex.Message;
                LogAPI.WriteLog("GetOptimizeData - " + msg);
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
            return ds;
        }
        public static DataTable GetOptimizeData(RecordType Type)
        {
            String msg = "";
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_GetOptimizedKeysToUpdateOnAmz";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@type", SqlDbType.Int).Value = (int)Type;
                cmd.Parameters.AddWithValue("@date", SqlDbType.SmallDateTime).Value = DateTime.Today;
                dt = DataAccess.GetTable(cmd);
            }

            catch (Exception ex)
            {
                msg = ex.Message;
                LogAPI.WriteLog("GetOptimizeData - " + msg);
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
            return dt;
        }

        /// <summary>
        /// Update Campaigns
        ///<param name="camptbl">camptbl</param>
        ///<param name="userid">userid</param>
        ///<param name="reportid">reportid</param>
        /// </summary>
        /// <returns></returns>
        public static String UpdateCampaignData(DataTable camptbl, Int32 userid, Int32 reportid, out String emailtoNotify)
        {
            String res = "";
            emailtoNotify = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"Sbsp_UpdateCampaigns";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tblCampaigns", camptbl);
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@ReportID", SqlDbType.Int).Value = reportid;
                cmd.Parameters.Add("@RecordType", SqlDbType.Int).Value = (int)RecordType.Campaign;
                cmd.Parameters.Add("@Date", SqlDbType.SmallDateTime).Value = DateTime.Today.AddDays(-3);

                SqlParameter UserNotify = new SqlParameter("@Email", SqlDbType.VarChar, 100);
                UserNotify.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(UserNotify);

                DataAccess.ExecuteScalarCommand(cmd);
                emailtoNotify = Convert.ToString(UserNotify.Value);
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateCampaignData-" + reportid + ": " + ex.Message, userid.ToString());
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateCampaignData-" + reportid + ": " + ex.Message, userid.ToString());
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
        /// Update AdGroups
        ///<param name="adgtbl">adgtbl</param>
        ///<param name="userid">userid</param>
        ///<param name="reportid">reportid</param>
        /// </summary>
        /// <returns></returns>
        public static String UpdateAdGroupData(DataTable adgtbl, Int32 userid, Int32 reportid)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"Sbsp_UpdateAdGroups";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tblAdgroups", adgtbl);
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@ReportID", SqlDbType.Int).Value = reportid;
                cmd.Parameters.Add("@RecordType", SqlDbType.Int).Value = (int)RecordType.AdGroup;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateAdGroupData-" + reportid + ": " + ex.Message, userid.ToString());
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateAdGroupData-" + reportid + ": " + ex.Message, userid.ToString());
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
        /// Update Keywords
        ///<param name="keywordtbl">keywordtbl</param>
        ///<param name="userid">userid</param>
        ///<param name="reportid">reportid</param>
        /// </summary>
        /// <returns></returns>
        public static String UpdateKeywordData(DataTable keywordtbl, Int32 userid, Int32 reportid)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"Sbsp_UpdateKeywords";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tblKeywords", keywordtbl);
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@ReportID", SqlDbType.Int).Value = reportid;
                cmd.Parameters.Add("@RecordType", SqlDbType.Int).Value = (int)RecordType.Keyword;
                DataAccess.ExecuteScalarCommandForBulk(cmd);
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateKeywordData-" + reportid + ": " + ex.Message, userid.ToString());
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateKeywordData-" + reportid + ": " + ex.Message, userid.ToString());
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
        /// Update Ads
        ///<param name="adstbl">adstbl</param>
        ///<param name="userid">userid</param>
        ///<param name="reportid">reportid</param>
        /// </summary>
        /// <returns></returns>
        public static String UpdateAdsData(DataTable adtbl, Int32 userid)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"Sbsp_UpdateAds";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tblAds", adtbl);
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@Date", SqlDbType.SmallDateTime).Value = DateTime.Now.Date;
                DataAccess.ExecuteScalarCommandForBulk(cmd);
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateAdsData- " + ex.Message, userid.ToString());
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateAdsData- " + ex.Message, userid.ToString());
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
        /// Update api attempts
        ///<param name="reportid">reportid</param>
        ///<param name="reporttype">reporttype</param>
        /// </summary>
        /// <returns></returns>
        public static String UpdateAPIAttemptStatusData(Int32 reportid, int reporttype)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"Sbsp_UpdateFailedAttemptStatus";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@ReportId", SqlDbType.Int).Value = reportid;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = reporttype;
                cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = DateTime.Now;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateAPIStatusData-" + reportid + ": " + ex.Message);
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateAPIStatusData-" + reportid + ": " + ex.Message);
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
        /// Update products from snap shot report
        ///<param name="snapid">snapid</param>
        ///<param name="reportid">reportid</param>
        ///<param name="reporttype">reporttype</param>
        /// </summary>
        /// <returns></returns>
        public static String UpdateSnapStatusData(String snapid, Int32 reportid, int reporttype, ResponseStatus responsestatus)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"Update ReportType set Snap_ReportId=@Snap_ReportId,SnapAttempt=0,ApiAttempt=0,SnapStatus=@Status,SnapReqDate=@Date,ModifiedOn=@Date
                                    where ReportId=@ReportId and RecordType=@Type";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@ReportId", SqlDbType.Int).Value = reportid;
                cmd.Parameters.Add("@Snap_ReportId", SqlDbType.VarChar).Value = snapid;
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = (int)responsestatus;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = reporttype;
                cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = DateTime.Now;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateSnapStatusData-" + reportid + ": " + ex.Message);
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateSnapStatusData-" + reportid + ": " + ex.Message);
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
        /// Update Inventory Status
        ///<param name="amzreportid">amzreportid</param>
        ///<param name="reportid">reportid</param>
        ///<param name="reporttype">reporttype</param>
        ///<param name="status">status</param>
        ///<param name="responsestatus">responsestatus</param>
        /// </summary>
        /// <returns></returns>
        public static String UpdateInventoryStatusData(String amzreportid, Int32 reportid, int reporttype, ReportStatus statustype, ResponseStatus responsestatus)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_UpdateInventoryStatus";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@ReportId", SqlDbType.Int).Value = reportid;
                cmd.Parameters.Add("@Amz_ReportId", SqlDbType.VarChar).Value = amzreportid;
                cmd.Parameters.Add("@StatusType", SqlDbType.Int).Value = (int)statustype;
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = (int)responsestatus;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = reporttype;
                cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = DateTime.Now;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateInventoryStatusData-" + reportid + ": " + ex.Message);
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateInventoryStatusData-" + reportid + ": " + ex.Message);
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
        /// Update Report Status
        ///<param name="reportid">reportid</param>
        ///<param name="reporttype">reporttype</param>
        ///<param name="status">status</param>
        ///<param name="responsestatus">responsestatus</param>
        /// </summary>
        /// <returns></returns>
        public static String UpdateReportStatusData(Int32 IsSnapType,Int32 reportid, int reporttype, ReportStatus statustype, ResponseStatus responsestatus)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "UpdateReportDataStatus";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@ReportId", SqlDbType.Int).Value = reportid;
                cmd.Parameters.Add("@StatusType", SqlDbType.Int).Value = (int)statustype;
                cmd.Parameters.Add("@IsSnap", SqlDbType.Int).Value = (int)IsSnapType;
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = (int)responsestatus;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = reporttype;
                cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = DateTime.Now;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateReportStatusData-" + reportid + ": " + ex.Message);
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateReportStatusData-" + reportid + ": " + ex.Message);
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
        /// Update Products Inventory
        ///<param name="Inventorytbl">Inventorytbl</param>
        ///<param name="userid">userid</param>
        ///<param name="reportid">reportid</param>
        ///<param name="recordtype">recordtype</param>
        ///<param name="status">status</param>
        /// </summary>
        /// <returns></returns>
        public static String UpdateProductInventoryData(DataTable Inventorytbl, Int32 userid, Int32 reportid, int recordtype, ReportStatus status, out String emailtoNotify)
        {
            String res = "";
            emailtoNotify = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"Sbsp_UpdateProductInventory";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tblProducts", Inventorytbl);
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@ReportID", SqlDbType.Int).Value = reportid;
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = status;
                cmd.Parameters.Add("@RecordType", SqlDbType.Int).Value = recordtype;

                SqlParameter UserNotify = new SqlParameter("@Email", SqlDbType.VarChar, 100);
                UserNotify.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(UserNotify);

                DataAccess.ExecuteScalarCommandForBulk(cmd);
                emailtoNotify = Convert.ToString(UserNotify.Value);
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateProductInventoryData-" + reportid + ": " + ex.Message, userid.ToString());
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateProductInventoryData-" + reportid + ": " + ex.Message, userid.ToString());
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
        /// Update search term Inventory(sales,clicks,spend)
        ///<param name="Inventorytbl">Inventorytbl</param>
        ///<param name="userid">userid</param>
        ///<param name="reportid">reportid</param>
        ///<param name="status">status</param>
        /// </summary>
        /// <returns></returns>
        public static String UpdateSearchTermInventoryData(DataTable Inventorytbl, Int32 userid, Int32 reportid, ReportStatus status)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"Sbsp_UpdateSearchtermInventory";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tblSearchterm", Inventorytbl);
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@ReportId", SqlDbType.Int).Value = reportid;
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = status;
                cmd.Parameters.Add("@RecordType", SqlDbType.Int).Value = (int)RecordType.SearchTerm;
                DataAccess.ExecuteScalarCommandForBulk(cmd);
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateSearchTermInventoryData-" + reportid + ": " + ex.Message, userid.ToString());
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateSearchTermInventoryData-" + reportid + ": " + ex.Message, userid.ToString());
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
        /// Update keywords bids or Create negative keywords
        /// <param name="ReportID">ReportID</param>
        ///<param name="tbl">tbl</param>
        ///<param name="Type">Type</param>
        /// </summary>
        /// <returns></returns>
        public static String UpdateOptimizeLogData(Int32 ReportID, DataTable tbl, RecordType Type)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"Sbsp_CreateOptimizeLogReport";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tblOptimizelog", tbl);
                cmd.Parameters.AddWithValue("@ReportID", ReportID);
                cmd.Parameters.AddWithValue("@Type", (int)Type);
                cmd.Parameters.Add("@Date", SqlDbType.SmallDateTime).Value = DateTime.Today;
                DataAccess.ExecuteScalarCommandForBulk(cmd);
                return res;
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateOptimizeLogData-" + ReportID + ": " + ex.Message);
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateOptimizeLogData-" + ReportID + ": " + ex.Message);
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
        /// Update user optimization processed or not
        /// <param name="ReportID">ReportID</param>
        /// </summary>
        /// <returns></returns>
        public static String UpdateOptimizeLocalReportProcess(Int32 ReportID, Int32 Status)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"UPDATE Reports set IsLocallyProcessed = @Status, ModifiedOn = @Date where ReportID = @ReportID";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ReportID", ReportID);
                cmd.Parameters.AddWithValue("@Status", Status);
                cmd.Parameters.Add("@Date", SqlDbType.SmallDateTime).Value = DateTime.Today;
                DataAccess.ExecuteScalarCommandForBulk(cmd);
                return res;
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateOptimizeLocalReportProcess-" + ReportID + ": " + ex.Message);
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateOptimizeLocalReportProcess-" + ReportID + ": " + ex.Message);
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
        /// Update user optimization processed or not
        /// <param name="ReportID">ReportID</param>
        /// </summary>
        /// <returns></returns>
        public static String UpdateOptimizeLocalSearchReportProcess(Int32 ReportID, Int32 Status)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"UPDATE Reports set IsLocallyProcessedSearchTerm = @Status, ModifiedOn = @Date where ReportID = @ReportID";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ReportID", ReportID);
                cmd.Parameters.AddWithValue("@Status", Status);
                cmd.Parameters.Add("@Date", SqlDbType.SmallDateTime).Value = DateTime.Today;
                DataAccess.ExecuteScalarCommandForBulk(cmd);
                return res;
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateOptimizeLocalSearchReportProcess-" + ReportID + ": " + ex.Message);
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateOptimizeLocalSearchReportProcess-" + ReportID + ": " + ex.Message);
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
        /// Update UserSkuData
        ///<param name="reportid">reportid</param>
        /// </summary>
        /// <returns></returns>
        public static String UpdateUserSkuData(Int32 userid, int count,Int64 campId,int status)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"Sbsp_UpdateSkuCount";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@Count", SqlDbType.Int).Value = count;
                cmd.Parameters.Add("@campId", SqlDbType.BigInt).Value = campId;
                cmd.Parameters.Add("@status", SqlDbType.TinyInt).Value = status;

                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateUserSkuData-" + ex.Message, userid.ToString());
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateUserSkuData-" + ex.Message, userid.ToString());
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
        /// Update the notification of statup email
        /// </summary>
        /// <returns></returns>
        public static String UpdateFirstEmailNotify(Int32 UserID, Int16 status)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "UPDATE MppUser set DataImportAlert=@Status,StartDate=@Date,ModifiedOn=@ModifiedOn where MppUserID=@UserID";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                cmd.Parameters.Add("@Status", SqlDbType.Bit).Value = status;
                cmd.Parameters.Add("@Date", SqlDbType.SmallDateTime).Value = DateTime.Today;
                cmd.Parameters.Add("@ModifiedOn", SqlDbType.DateTime).Value = DateTime.Now;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogAPI.WriteLog("UpdateFirstEmailNotify - " + ex.Message, UserID.ToString());
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogAPI.WriteLog("UpdateFirstEmailNotify - " + ex.Message, UserID.ToString());
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

        public static DataTable GetActiveCamapignsByUsers()
        {
            String msg = "";
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"SELECT distinct top 1 t.MppUserID,
                                         STUFF((SELECT distinct ',' + CAST(t1.RecordID AS VARCHAR(200))
                                         FROM Campaigns t1
                                         WHERE t.MppUserID = t1.MppUserID AND t1.Active = 1 AND t1.TempSKUCheck = 0
                                         FOR XML PATH(''), TYPE
                                         ).value('.', 'NVARCHAR(MAX)') 
                                         ,1,1,'') Records
                                        FROM MppUser t WHERE t.ProfileAccess=1 and t.Active=1 and EXISTS(SELECT * from Campaigns where t.MppUserID = MppUserID AND Active=1 AND TempSKUCheck = 0);";
                cmd.CommandType = CommandType.Text;
                dt = DataAccess.GetTable(cmd);
            }

            catch (Exception ex)
            {
                msg = ex.Message;
                LogAPI.WriteLog("GetUserForOneTimeSkuLoad - " + msg);
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
            return dt;
        }

        /// <summary>
        /// get best customer search campaigns for creation 
        /// </summary>
        /// <returns></returns>
        public static DataSet GetBestSrchTermCampaignsToCreate()
        {
            String msg = "";
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_GetBestSrchTermCampaignsToCreate";
                cmd.CommandType = CommandType.StoredProcedure;
                ds = DataAccess.GetDataSetForBulk(cmd);
            }

            catch (Exception ex)
            {
                msg = ex.Message;
                LogAPI.WriteLog("GetBestSrchTermCampaignsToCreate - " + msg);
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
            return ds;
        }

        public static string UpdateBestSrchTermCampaigns(DataTable dt)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"Sbsp_UpdateBestSrchTerm";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UpdateNewCampDt", dt);
               
                DataAccess.ExecuteScalarCommand(cmd);
              
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateBestSrchTermCampaigns-"+ ex.Message);
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogAPI.WriteLog("UpdateBestSrchTermCampaigns-"+ ex.Message);
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

    }
}

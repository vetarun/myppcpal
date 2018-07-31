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
    public class AdminData
    {
        #region Class Variables and Constants
        #endregion

        #region Constructors
        #endregion

        #region Public Methods

        /// <summary>
        /// Get All Sellers
        /// </summary>
        /// <returns></returns>
        /// revenue data function 

        public List<AdminSeller> GetSellerData(int type)
        {
            try
            {
                DataTable dt = null;
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Sbsp_GetDashboardData";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@type", type);
                dt = DataAccess.GetTable(cmd);
                var res = dt.AsEnumerable().Select(r => ConvertToAdminSeller(r));
                return res.ToList();
            }

            catch (Exception ex)
            {
                LogFile.WriteLog("GetSellerData : " + ex.Message);
                return null;
            }


        }
        public List<YearlyRevenueData> GetYearlyRevenueData(int type)
        {
            try
            {

                DataTable dt = null;
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Sbsp_GetAdminRevenue";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@type", type);
                dt = DataAccess.GetTable(cmd);

                var yearlyPayDetail = dt.AsEnumerable().Select(r => ConvertToYearlyRevenueData(r));
                return yearlyPayDetail.ToList();
            }

            catch (Exception ex)
            {
                LogFile.WriteLog("GetYearlyRevenueData : " + ex.Message);
                return null;
            }
        }
        public List<RevenueData> GetRevenueData(int type)
        {
            try
            {

                DataTable dt = null;
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Sbsp_GetAdminRevenue";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@type", type);
                dt = DataAccess.GetTable(cmd);

                var payDetail = dt.AsEnumerable().Select(r => ConvertToRevenueData(r));
                return payDetail.ToList();
            }

            catch (Exception ex)
            {
                LogFile.WriteLog("GetRevenueData : " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get Seller Dashboard
        /// </summary>
        /// <returns></returns>
        public MppSales GetSellerDashboardData()
        {
            try
            {
                DataSet ds = null;
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Sbsp_GetAdminDashboard";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Date", SqlDbType.SmallDateTime).Value = DateTime.Today;
                ds = DataAccess.GetDataSet(cmd);
                return new MppSales()
                {
                    CurrentMonthlySales = Convert.ToDecimal(ds.Tables[0].Rows[0]["CurrentMonthlySales"]),
                    TotalYearlySales = Convert.ToDecimal(ds.Tables[0].Rows[0]["TotalYearlySales"]),
                    LastMonthSales = Convert.ToDecimal(ds.Tables[0].Rows[0]["LastMonthSales"]),
                    Inactive = Convert.ToInt32(ds.Tables[0].Rows[0]["Inactive"]),
                    PendingAccess = Convert.ToInt32(ds.Tables[0].Rows[0]["PendingAccess"]),
                    TrailEnd = Convert.ToInt32(ds.Tables[0].Rows[0]["TrailEnd"]),
                    Unsubscribed = Convert.ToInt32(ds.Tables[0].Rows[0]["Unsubscribed"]),
                    TotalActivePaid = Convert.ToInt32(ds.Tables[0].Rows[0]["ActivePaid"]),
                    TotalActiveTrail = Convert.ToInt32(ds.Tables[0].Rows[0]["ActiveTrail"]),
                    TotalNewlyPaid = Convert.ToInt32(ds.Tables[0].Rows[0]["NewlyPaid"])
                };
            }
            catch (Exception ex)
            {
                LogFile.WriteLog("GetSellerDashboardData : " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Admin User Registration
        ///<param name="firstName">firstName</param>
        /// </summary>
        /// <returns>ID</returns>
        public String AddUpdateAdminUser(String firstName, String lastName, String email, String password, String code,int type)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_InsertAdminUser";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Email", SqlDbType.VarChar, 500).Value = email;
                cmd.Parameters.Add("@Password", SqlDbType.Binary, 16).Value = MppUtility.EncryptPassword(password);
                cmd.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = firstName;
                cmd.Parameters.Add("@LastName", SqlDbType.VarChar).Value = lastName;
                cmd.Parameters.Add("@Code", SqlDbType.VarChar).Value = code;
                cmd.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@type", SqlDbType.Int).Value = type;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Insert AdminUser : " + msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Insert AdminUser : " + msg);
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
        public static String DeleteArchiveData()
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_DeleteArchiveData";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@ArchiveDate", SqlDbType.SmallDateTime).Value = DateTime.Now.Date;
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
        /// Update Seller Status 
        ///<param name="sellerID">userID</param>
        ///<param name="Status">Status</param>
        /// </summary>
        /// <returns></returns>
        public String UpdateSeller(Int32 sellerID, bool Status, int PlanStatus, bool Sellerstatus)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_UpdateSellerDetails";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@userID", SqlDbType.Int).Value = sellerID;
                cmd.Parameters.Add("@Status", SqlDbType.Bit).Value = Status;
                cmd.Parameters.Add("@PlanStatus", SqlDbType.Int).Value = PlanStatus;
                cmd.Parameters.Add("@Selleraccess", SqlDbType.Int).Value = Sellerstatus;
                cmd.Parameters.Add("@ModifiedOn", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@modifiedBy", SqlDbType.VarChar, 50).Value = "Admin";
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update SellerDetails - " + sellerID + ": " + msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update SellerDetails - " + sellerID + ": " + msg);
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
        /// Delete Seller  
        ///<param name="sellerID">userID</param>
        /// </summary>
        /// <returns></returns>
        public String DeleteSeller(Int32 sellerID)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "update  MppUser set IsArchive=1 , IsArchiveOn = getdate() where MppUserID=@userID";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@userID", SqlDbType.Int).Value = sellerID;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Delete SellerId - " + sellerID + ": " + msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Delete SellerId - " + sellerID + ": " + msg);
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
        /// Get Admin Users
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Administration> GetAdminUsersData(Int32 adminID)
        {
            DataTable dt = null;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Select MppAdminID,Email,FirstName,LastName,Active,UserType from MppAdmin where Email !=@Email and MppAdminID !=@AdminID";
            cmd.Parameters.Add("@Email", SqlDbType.VarChar, 50).Value = "admin@myppcpal.com";
            cmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = adminID;
            cmd.CommandType = CommandType.Text;
            dt = DataAccess.GetTable(cmd);
            var res = dt.AsEnumerable().Select(r => ConvertToUser(r));
            return res;
        }

        /// <summary>
        /// Delete User 
        ///<param name="userID">userID</param>
        /// </summary>
        /// <returns></returns>
        public String DeleteAdminUser(Int32 userID)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Delete from MppAdmin where MppAdminID=@userID";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@userID", SqlDbType.Int, 20).Value = userID;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Delete AdminId - " + userID + ": " + msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Delete AdminId - " + userID + ": " + msg);
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
        /// Update User Status 
        ///<param name="userID">userID</param>
        ///<param name="Status">Status</param>
        /// </summary>
        /// <returns></returns>
        public String UpdateUserStatus(Int32 userID, bool Status)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Update MppAdmin set Active=@Status where MppAdminID=@userID";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
                cmd.Parameters.Add("@Status", SqlDbType.Bit).Value = Status;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update AdminStatus - " + userID + ": " + msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update AdminStatus - " + userID + ": " + msg);
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
        /// Insert or Update Coupon
        ///<param name="firstName">firstName</param>
        /// </summary>
        /// <returns>ID</returns>
        public String AddUpdateCouponData(String Code, int? Duration, Decimal? Amount, int? Percent_off, int? Max, DateTime? Redeemby,int userID)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_AddUpdateCoupon";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@CouponCode", SqlDbType.VarChar, 30).Value = Code;
                cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = Amount;
                cmd.Parameters.Add("@Percent", SqlDbType.Int).Value = Percent_off;
                cmd.Parameters.Add("@Max", SqlDbType.Int).Value = Max;
                cmd.Parameters.Add("@Redeemby", SqlDbType.SmallDateTime).Value = Redeemby;
                cmd.Parameters.Add("@Duration", SqlDbType.Int).Value = Duration;
                cmd.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = userID;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Add Or Update CouponData - " + Code + ": " + msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Add Or Update CouponData - " + Code + ": " + msg);
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
        /// Get Coupns
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AffiliationCode> GetAffiliationData(string srchTerm)
        {
            DataTable dt = null;
            SqlCommand cmd = new SqlCommand();
            srchTerm = "'"+srchTerm + "%'";
            cmd.CommandText = @"select af.AffiliateID,af.AffiliateCode,ISNULL(af.Amount,0) as Amount,af.Duration,
                                ISNULL(af.Percentile_off,0) as Percent_off,
                                ISNULL(af.Maxredem,0) as Maxredeem,af.Redeemby, count(bl.MppUserId) as Subscribers,
                                ISNULL(SUM(bl.Amount_off),0) as Sales,
                                ISNULL(SUM(bl.Amount),0) as PreOffSale ,usr.FirstName+' '+usr.LastName as CreatedBy,
                                ISNULL(afas.Id,0) as AssignId,
								SUM(ISNULL(bl.AffiliateEarning,0.0)) AS Commision
                                from AffiliationCode  as af 
                                left outer join Billing as bl on af.AffiliateID = bl.AffiliateID
                                join MppAdmin usr on usr.MppAdminID=af.CreatedBy
								left join AffiliateAssignedCodes afas on afas.CouponId=af.AffiliateID and afas.IsActive=1
                                where af.AffiliateCode like "+srchTerm+" group by af.AffiliateID,af.AffiliateCode,af.Amount,af.Duration,af.Percentile_off,af.Maxredem,af.Redeemby,usr.FirstName,usr.LastName,afas.Id";
            cmd.CommandType = CommandType.Text;
            dt = DataAccess.GetTable(cmd);
            var res = dt.AsEnumerable().Select(r => ConvertToAffiliation(r)).ToList();
            return res;
        }

        /// <summary>
        /// Get Coupns
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AffiliationCode> GetAffiliationData(int userID,string srchTerm)
        {
            DataTable dt = null;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sbsp_GetAffiliateCodes";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userID;
            cmd.Parameters.Add("@srchTerm", SqlDbType.VarChar,100).Value = string.IsNullOrWhiteSpace(srchTerm)?"":srchTerm;
            dt = DataAccess.GetTable(cmd);
            var res = dt.AsEnumerable().Select(r => ConvertToAffiliation(r)).ToList();
            return res;
        }

        /// <summary>
        /// Delete Coupon
        ///<param name="couponID">couponID</param>
        /// </summary>
        /// <returns></returns>
        public String DeleteCouponData(string couponID)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
               
                cmd.CommandText = "Sbsp_DeleteCoupon";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@coupon", SqlDbType.VarChar, 200).Value = couponID;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Delete CouponData - " + couponID + ": " + msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Delete CouponData - " + couponID + ": " + msg);
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
        /// Update Coupon Status 
        ///<param name="codeID">codeID</param>
        ///<param name="Status">Status</param>
        /// </summary>
        /// <returns></returns>
        public String UpdateCouponStatus(Int32 codeID, bool Status)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Update MppCoupons set Active=@Status where MppCouponID=@codeID";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@codeID", SqlDbType.Int).Value = codeID;
                cmd.Parameters.Add("@Status", SqlDbType.Bit).Value = Status;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update CouponStatus - " + codeID + ": " + msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update CouponStatus - " + codeID + ": " + msg);
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
        /// Get 2FA Codes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TwoFactorCodes> Get2FAData()
        {
            DataTable dt = null;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = @"select top 5  Id,LoginCode as LoginCode,LoginDate,CreatedOn from AmazonLoginAccess order by Id desc";
            cmd.CommandType = CommandType.Text;
            dt = DataAccess.GetTable(cmd);
            var res = dt.AsEnumerable().Select(r => ConvertToTwoFactorCodes(r)).ToList();
            return res;
        }

        /// <summary>
        /// Get 2FA Codes
        /// </summary>
        /// <returns></returns>
        public String Update2FACodeData(Int32 codeID, String code)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Update AmazonLoginAccess set LoginCode=@Code where Id=@CodeID";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@CodeID", SqlDbType.Int).Value = codeID;
                cmd.Parameters.Add("@Code", SqlDbType.VarChar).Value = code;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update 2FACode - " + codeID + ": " + msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update 2FACode - " + codeID + ": " + msg);
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
        /// Get Affiliate Sales
        /// </summary>
        /// <returns></returns>
        public DataSet GetAffiliateSaleData(int Id)
        {
            DataSet ds = null;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sbsp_GetAffiliateReportDetails";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@AffiliateID", SqlDbType.Int).Value = Id;
            ds = DataAccess.GetDataSet(cmd);
            return ds;
        }

        /// <summary>
        /// Get Active users
        /// </summary>
        /// <returns></returns>
        public DataTable GetActiveUsersData()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT FirstName,LastName,Email from MppUser where Active=1";
            cmd.CommandType = CommandType.Text;
            DataTable dt = DataAccess.GetTable(cmd);
            return dt;
        }
  

        /// <summary>
        /// get all clients or users 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public List<object> GetAllClients(string term)
        {
            var list = new List<object>();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sbsp_GetMPPClients";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@term", SqlDbType.VarChar, 50).Value = term;
            DataTable dt = DataAccess.GetTable(cmd);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new { Id = row["Id"], Name = row["Name"] });
            }
            return list;
        }

        public List<object> GetCampaigns(string term, int userId)
        {
            var list = new List<object>();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sbsp_getCampaigns";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@term", SqlDbType.VarChar, 50).Value = term;
            cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
            DataTable dt = DataAccess.GetTable(cmd);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new { Id = row["RecordID"], Name = row["CampaignName"] });
            }
            return list;
        }
        /// <summary>
        /// get report log data to show admin user
        /// </summary>
        /// <returns></returns>
        public DataTable GetReportLogs(PagingOptions options, out int total)
        {
           var date1=  Convert.ToDateTime(options.Date);
           var date2= Convert.ToDateTime(options.Date2);
               date2=date2.AddDays(1);

            var list = new List<object>();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sbsp_GetReportLog";
            cmd.Parameters.Add("@orderBy", SqlDbType.VarChar, 100).Value = options.ColumnName;
            cmd.Parameters.Add("@dir", SqlDbType.Bit).Value = options.Direction;
            cmd.Parameters.Add("@pageNumber", SqlDbType.Int).Value = options.Start;
            cmd.Parameters.Add("@pageSize", SqlDbType.Int).Value = options.Length;
            cmd.Parameters.Add("@client", SqlDbType.Int).Value = options.Client;
            cmd.Parameters.Add("@date", SqlDbType.Date).Value =  date1;
            cmd.Parameters.Add("@date2", SqlDbType.Date).Value = date2;

            cmd.Parameters.Add("@total", SqlDbType.Int);


            cmd.Parameters["@total"].Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            DataTable dt = DataAccess.GetTable(cmd);
            total = (int)cmd.Parameters["@total"].Value;
            return dt;
        }

        /// <summary>
        /// get report log data to show admin user
        /// </summary>
        /// <returns></returns>
        public DataTable GetEmailLOg(PagingOptions options, out int total)
        {
            var list = new List<object>();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sbsp_getEmailLog";
            cmd.Parameters.Add("@orderBy", SqlDbType.VarChar, 100).Value = options.ColumnName;
            cmd.Parameters.Add("@dir", SqlDbType.Bit).Value = options.Direction;
            cmd.Parameters.Add("@pageNumber", SqlDbType.Int).Value = options.Start;
            cmd.Parameters.Add("@pageSize", SqlDbType.Int).Value = options.Length;
            cmd.Parameters.Add("@client", SqlDbType.Int).Value = options.Client;
            cmd.Parameters.Add("@date", SqlDbType.VarChar, 50).Value = options.Date;

            cmd.Parameters.Add("@total", SqlDbType.Int);


            cmd.Parameters["@total"].Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            DataTable dt = DataAccess.GetTable(cmd);
            total = (int)cmd.Parameters["@total"].Value;
            return dt;
        }

        /// <summary>
        /// affilate user list based on 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public List<object> GetAffilateUsers(string srchTerm,int code)
        {
            DataTable dt = null;
            SqlCommand cmd = new SqlCommand();
            var list = new List<object>();
            var query = "";
            try
            {

                if (string.IsNullOrWhiteSpace(srchTerm))
                    query = @"SELECT MppAdminID Id,FirstName+' ( '+Email+' )' as Name FROM MppAdmin  where UserType=2 and Active=1 and IsAccountLocked=0 " +
                        "and not exists(select * from AffiliateAssignedCodes where UserId=MppAdminID and CouponId="+code+")" +
                        "and not exists(select * from AffiliationCode where CreatedBy= MppAdminID and AffiliateID=" + code + ")";
                else
                    query = @"SELECT MppAdminID Id,FirstName+' ( '+Email+' )' as Name FROM MppAdmin
                              where UserType=2 and Active=1 and IsAccountLocked=0 and (Email like '" + srchTerm + "%' OR FirstName like '" + srchTerm + "%') " +
                              "and not exists(select * from AffiliateAssignedCodes where UserId= MppAdminID and CouponId=" + code + ")" +
                              "and not exists(select * from AffiliationCode where CreatedBy= MppAdminID and AffiliateID=" + code + ")";
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
               
                dt = DataAccess.GetTable(cmd);
                if(dt!=null && dt.Rows.Count>0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(new { Id = Convert.ToInt32(row["Id"]), Name = row["Name"] });
                    }
                }
                return list;
            }
            catch(Exception ex)
            {
                return list;
            }
            finally
            {
                cmd.Dispose();
            }
        }

        /// <summary>
        /// Add coupon code with affiliate assigned by admin user
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        /// 
        public bool AddAffiliateCode(int code,int userId)
        {
           
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = " INSERT INTO AffiliateAssignedCodes(UserId,CouponId,IsActive) VALUES("+userId+","+code+ ",1)";
                cmd.CommandType = CommandType.Text;
                DataAccess.ExecuteCommand(cmd);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
        }

        /// <summary>
        /// get assigned affiliates by coupon id
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        /// 
        public List<object> GetAssignedAffliates(int code)
        {
            DataTable dt = null;
            var list = new List<object>();

            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"SELECT MppAdminID,FirstName+' '+LastName Name,Email FROM MppAdmin join
                                     AffiliateAssignedCodes af on af.UserId=MppAdminID and af.CouponId="+code+"";
                cmd.CommandType = CommandType.Text;
                dt=DataAccess.GetTable(cmd);
                if(dt!=null && dt.Rows.Count>0)
                {
                    foreach(DataRow dr in dt.Rows)
                    {
                        list.Add(new {Id=Convert.ToInt32(dr["MppAdminID"]),Name=dr["Name"],Email= dr["Email"] });
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex.Message);
                return list;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                    dt.Dispose();
                }
            }
        }
        public List<object> GetReportDates(int userId,int type)
        {

            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sbsp_GetUserReportDates";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
            ds = DataAccess.GetDataSet(cmd);
            return GetDates(ds, type);
        }
        private List<object> GetDates(DataSet ds,int type)
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
                    if (type == 2)
                        KeyBulk = KeyBulk.Take(10).ToList();
                    objList.Add(new { Dates = KeyBulk });

                }
                else
                {
                    objList.Add(new { Dates = "" });
                }
                if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                {
                    List<object> Stbulk = new List<object>();
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        Stbulk.Add(new { Date = Convert.ToDateTime(row["StermBulkDates"]).ToString("MM/dd/yyyy") });
                    }
                    if (type == 2)
                        Stbulk = Stbulk.Take(10).ToList();

                    objList.Add(new { Dates = Stbulk });
                }
                else
                {
                    objList.Add(new { Dates = "" });
                }
                if (ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                {
                    List<object> Stbulk = new List<object>();
                    foreach (DataRow row in ds.Tables[2].Rows)
                    {
                        Stbulk.Add(new { Date = Convert.ToDateTime(row["KeyUpload"]).ToString("MM/dd/yyyy") });
                    }

                    if (type == 2)
                        Stbulk = Stbulk.Take(10).ToList();
                    objList.Add(new { Dates = Stbulk });
                }
                else
                {
                    objList.Add(new { Dates = "" });
                }
                if (ds.Tables[3] != null && ds.Tables[3].Rows.Count > 0)
                {
                    List<object> Stbulk = new List<object>();
                    foreach (DataRow row in ds.Tables[3].Rows)
                    {
                        Stbulk.Add(new { Date = Convert.ToDateTime(row["StermUpload"]).ToString("MM/dd/yyyy") });
                    }
                    if (type == 2)
                        Stbulk = Stbulk.Take(10).ToList();
                    objList.Add(new { Dates = Stbulk });
                }
                else
                {
                    objList.Add(new { Dates = "" });
                }
            }
            return objList;
        }

        #endregion

        #region Private Methods
        private AdminSeller ConvertToAdminSeller(DataRow row)
        {
            return new AdminSeller()
            {
                SellerID = row.Field<Int32>("MppUserID"),
                Stp_CustID = row.Field<String>("stp_custId"),
                Email = row.Field<String>("Email"),
                FirstName = row.Field<String>("FirstName"),
                LastName = row.Field<String>("LastName"),
                Active = row.Field<Boolean>("Active"),
                StartDate = row.Field<DateTime>("StartDate"),
                ProfileId = row.Field<String>("ProfileId"),
                SellerAccess = (row.Field<int>("ProfileAccess") == 1) ? true : false,
                Plan = row.Field<Int32>("PlanID") > 7 ? "8" : row.Field<Int32>("PlanID").ToString(),
                PlanStatus = row.Field<Int32>("PlanStatus")
            };
        }
        private RevenueData ConvertToRevenueData(DataRow row)
        {
            return new RevenueData()
            {
                //SellerName = row.Field<String>("SellerName"),
                Payment = row.Field<Decimal>("Amount_off"),
                FirstName = row.Field<String>("FirstName"),
                LastName = row.Field<String>("LastName"),
                Plan = row.Field<Int32>("PlanID"),
                PaymentDate = row.Field<DateTime>("CreatedOn"),

            };
        }
        private YearlyRevenueData ConvertToYearlyRevenueData(DataRow row)
        {
            return new YearlyRevenueData()
            {
                PaymentMonth = row.Field<String>("PaymentMonth"),
                PaymentAmount = row.Field<Decimal>("PaymentAmount"),
                MonthNumber = row.Field<Int32>("MonthNumber")
            };
        }

        private Administration ConvertToUser(DataRow row)
        {
            return new Administration()
            {
                UserID = row.Field<Int32>("MppAdminID"),
                Email = row.Field<String>("Email"),
                FirstName = row.Field<String>("FirstName"),
                LastName = row.Field<String>("LastName"),
                Active = row.Field<Boolean>("Active"),
                UserType=row.Field<int>("UserType")
            };
        }

        private AffiliationCode ConvertToAffiliation(DataRow row)
        {
            var times_redeemed = 0;
            var Code = row.Field<String>("AffiliateCode");
            var coupon = StripeHelper.GetCoupon(Code);
            var sales = row.Field<Decimal>("Sales");
            if (coupon != null)
                times_redeemed = coupon.TimesRedeemed;
            var couponamount = row.Field<Decimal>("Amount");
            var actualPrice = row.Field<Decimal>("PreOffSale");
            var offInPercent = row.Field<int>("Percent_off");
            var customers = row.Field<int>("Subscribers");
            var commision = Math.Round(row.Field<Decimal>("Commision"), 2);
            var assignId = row.Field<int>("AssignId");
            return new AffiliationCode()
            {
                ID = row.Field<int>("AffiliateID"),
                Code = Code,
                Percent = offInPercent,
                Duration = row.Field<int>("Duration"),
                Sales = sales,
                Subscribers = customers,
                Amount = couponamount,
                Max = row.Field<int>("Maxredeem"),
                Redeemby = row.Field<DateTime?>("Redeemby"),
                PreDiscountSale = actualPrice,
                AffiliateCommission = (commision <= 0.00M)?0.00M: commision,
                CreatedBy = row.Field<string>("CreatedBy"),
                IsAssigned = assignId <= 0 ? false : true,
                RedeemedCount = times_redeemed
            };
        }

        public decimal CalculateCommision(decimal couponamount, decimal actualPrice,int offInPercent,decimal actualSale)
        {
            var percentage = 0.00M;
            var commision = 0.00M;
            if (couponamount > 0M && actualPrice != 0)
            {
                percentage = (actualSale / actualPrice);
                percentage = 1.00M - percentage;
            }
            else if (offInPercent > 0)
            {
                percentage = offInPercent / 100M;
            }
            var commissionRate = (0.30M - percentage);
            commision = commissionRate * actualPrice;
            return (commision<=0?0:commision);   
        }
        private TwoFactorCodes ConvertToTwoFactorCodes(DataRow row)
        {
            return new TwoFactorCodes()
            {
                Id = row.Field<int>("Id"),
                Code = row.Field<int?>("LoginCode"),
                Date = row.Field<DateTime?>("LoginDate"),
                Time = ConvertToTime(row.Field<DateTime?>("CreatedOn")),
                Required = CodeRequired(row.Field<DateTime>("CreatedOn"), row.Field<int?>("LoginCode"))
            };
        }

        private bool CodeRequired(DateTime dt, int? code)
        {
            bool Valid = false;
            double Timeleft = 0;
            Timeleft = DateTime.Now.Subtract(dt).TotalMilliseconds;
            if (Timeleft < 180000 && code == null)
                Valid = true;
            return Valid;
        }
        private String ConvertToTime(DateTime? dt)
        {
            String Time = "";
            if (dt != null)
            {
                Time = dt.Value.ToString("hh:mm tt");
            }
            return Time;
        }
        #endregion
    }
}

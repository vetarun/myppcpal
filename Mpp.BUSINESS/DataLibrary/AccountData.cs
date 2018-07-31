using Mpp.UTILITIES;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mpp.UTILITIES.BO;
using System.Configuration;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Mpp.BUSINESS.DataLibrary
{
    /// <summary>
    /// Class to handle all operations related to User Registration process
    /// </summary>
    public class AccountData
    {
        #region Class Variables and Constants
        #endregion

        #region Constructors
        #endregion

        #region Public Methods

        /// <summary>
        /// User Registration
        ///<param name="firstName">firstName</param>
        /// </summary>
        /// <returns>ID</returns>
        public String InsertUpdateUser(String firstName, String lastName, String email, String password, String activecode)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"Sbsp_InsertUser";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Email", SqlDbType.VarChar, 150).Value = email;
                cmd.Parameters.Add("@SbPassword", SqlDbType.Binary, 16).Value = MppUtility.EncryptPassword(password);
                cmd.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = firstName;
                cmd.Parameters.Add("@LastName", SqlDbType.VarChar).Value = lastName;
                cmd.Parameters.Add("@StartDate", SqlDbType.SmallDateTime).Value = DateTime.Now.Date;
                cmd.Parameters.Add("@TrailEndDate", SqlDbType.SmallDateTime).Value = DateTime.Now.Date.AddDays(30);
                cmd.Parameters.Add("@ActivationCode", SqlDbType.VarChar).Value = activecode;
                cmd.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value = DateTime.Now;

                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogFile.WriteLog(res);
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogFile.WriteLog(res);
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
        /// User Activation code 
        ///<param name="userID">userID</param>
        ///<param name="ActivationCode">ActivationCode</param>
        /// </summary>
        /// <returns>Affected rows</returns>
        public string InsertActivationCode(Int32 userID, String ActivationCode, Enum Type)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_ActivationCode";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                cmd.Parameters.Add("@ActivationCode", SqlDbType.VarChar).Value = ActivationCode;
                cmd.Parameters.Add("@Type", SqlDbType.Bit).Value = Type;
                cmd.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value = DateTime.Now;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogFile.WriteLog(res);
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogFile.WriteLog(res);
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
        /// User Account activation
        ///<param name="Code">Code</param>
        /// </summary>
        /// <returns></returns>
        public DataRow AccountConfirmation(String Code)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_UserActivation";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@ActivationCode", SqlDbType.VarChar).Value = Code;
                var dt=DataAccess.GetTable(cmd);
                return dt.Rows[0];
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogFile.WriteLog(res);
                return null;
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogFile.WriteLog(res);
                return null;
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
        /// User Account activation
        ///<param name="Code">Code</param>
        /// </summary>
        /// <returns></returns>
        public String ValidatePasswordCode(String Code, Int32 Type)
        {
            String res;
            String query;
            if (Type == 1)
                query = "Select Email from MppAdmin where Code = @ActivationCode";
            else
                query = "Select Email from MppUser where Code = @ActivationCode";
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@ActivationCode", SqlDbType.VarChar).Value = Code;
            res = Convert.ToString(DataAccess.GetDataValue(cmd));
            return res;
        }

        /// <summary>
        /// Email Address Exists or not
        ///<param name="Email">Email</param>
        /// </summary>
        /// <returns></returns>
        public Int32 CheckEmailExists(String Email)
        {
            Int32 Count = 0;
            SqlCommand cmd = new SqlCommand();
            try
            {
                String query = @"Select count(*) from MppUser where Email=@Email";
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@Email", SqlDbType.VarChar, 50).Value = Email;
                Count = Convert.ToInt32(DataAccess.ExecuteScalarCommand(cmd));
            }
            catch (SqlException ex)
            {
                LogFile.WriteLog(ex.Message);
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex.Message);
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
            return Count;
        }

        /// <summary>
        /// Gets user data 
        ///<param name="Email">Email</param>
        /// </summary>
        /// <returns></returns>
        public DataSet ValidateEmailAndGetUserinfo(String Email)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_GetUserDetails";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Email", SqlDbType.VarChar, 50).Value = Email;
                ds = DataAccess.GetDataSet(cmd);
            }
            catch (SqlException ex)
            {
                LogFile.WriteLog(ex.Message);
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex.Message);
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
        /// User login vaildation
        ///<param name="UserID">UserID</param>
        ///<param name="Password">Password</param>
        /// </summary>
        /// <returns></returns>
        public String CheckUserLogin(Int32 UserID, String Password)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_VerifyUserLogin";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserID", SqlDbType.VarChar, 50).Value = UserID;
                cmd.Parameters.Add("@Password", SqlDbType.Binary, 16).Value = MppUtility.EncryptPassword(Password);
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogFile.WriteLog(ex.Message);
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogFile.WriteLog(ex.Message);
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
        /// Gets Admin user data 
        ///<param name="Email">Email</param>
        /// </summary>
        /// <returns></returns>
        public DataTable ValidateEmailAndGetAdminUserinfo(String Email)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            try
            {
                String query = @"Select MppAdminID,FirstName,LastName,UserType from MppAdmin where Email=@Email and Active=1";
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@Email", SqlDbType.VarChar, 50).Value = Email;
                dt = DataAccess.GetTable(cmd);
            }
            catch (SqlException ex)
            {
                LogFile.WriteLog(ex.Message);
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex.Message);
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
        /// User Admin login vaildation
        ///<param name="UserID">AdminID</param>
        ///<param name="Password">Password</param>
        /// </summary>
        /// <returns></returns>
        public String CheckAdminUserLogin(Int32 AdminID, String Password)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_VerifyAdminUserLogin";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@AdminID", SqlDbType.VarChar, 50).Value = AdminID;
                cmd.Parameters.Add("@Password", SqlDbType.Binary, 16).Value = MppUtility.EncryptPassword(Password);
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogFile.WriteLog(res);
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogFile.WriteLog(res);
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
        /// User Password
        ///<param name="email">email</param>
        /// </summary>
        /// <returns>ID</returns>
        public String ChangePasswordCode(String email, String code)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"Sbsp_ChangePasswordCode";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Email", SqlDbType.VarChar, 50).Value = email;
                cmd.Parameters.Add("@Code", SqlDbType.VarChar).Value = code;
                cmd.Parameters.Add("@ModifiedOn", SqlDbType.DateTime).Value = DateTime.Now;

                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogFile.WriteLog(res);
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogFile.WriteLog(res);
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
        /// Update User Password 
        ///<param name="userID">userID</param>
        ///<param name="Pwd">Pwd</param>
        /// </summary>
        /// <returns>Affected rows</returns>
        public String UpdatePassword(String userID, String Pwd, Int32 Type)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_ResetPassword";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@userID", SqlDbType.VarChar).Value = userID;
                cmd.Parameters.Add("@Password", SqlDbType.Binary, 16).Value = MppUtility.EncryptPassword(Pwd);
                cmd.Parameters.Add("@Code", SqlDbType.VarChar).Value = null;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = Type;
                cmd.Parameters.Add("@ModifiedOn", SqlDbType.DateTime).Value = DateTime.Now;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog(msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog(msg);
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
        /// Update User Agreement
        ///<param name="userID">userID</param>
        ///<param name="IsAgreed">IsAgreed</param>
        /// </summary>
        /// <returns>Affected rows</returns>
        public String UpdateUserAgreement(Int32 userID, Int16 Accepted)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Update MppUser set IsAgreementConfirm=@Accepted,ModifiedOn=@ModifiedOn where MppUserID=@userID";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@userID", SqlDbType.VarChar).Value = userID;
                cmd.Parameters.Add("@Accepted", SqlDbType.Int).Value = Accepted;
                cmd.Parameters.Add("@ModifiedOn", SqlDbType.DateTime).Value = DateTime.Now;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog(msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog(msg);
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

        public String UpdateActivationEmailAlert(string email)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "DECLARE @activation int=0;SELECT @activation=ISNULL(ActivationEmailAlert,0) FROM MppUser where Email=@email;Update MppUser set ActivationEmailAlert=@activation+1,ModifiedOn=GETDATE() where Email=@email;";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@email", SqlDbType.VarChar,200).Value = email;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog(msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog(msg);
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

        //public String UpdateActivationEmailAlert(Int32 id)
        //{
        //    String msg = "";
        //    SqlCommand cmd = new SqlCommand();
        //    try
        //    {
        //        cmd.CommandText = "DECLARE @activation int=0;SELECT @activation=ISNULL(ActivationEmailAlert,0) FROM MppUser where MppUserID=@id;Update MppUser set ActivationEmailAlert=@activation+1,ModifiedOn=GETDATE() where MppUserID=@id;";
        //        cmd.CommandType = CommandType.Text;
        //        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id; ;
        //        DataAccess.ExecuteCommand(cmd);
        //    }
        //    catch (SqlException ex)
        //    {
        //        msg = ex.Message;
        //    }
        //    catch (Exception ex)
        //    {
        //        msg = ex.Message;
        //    }
        //    finally
        //    {
        //        if (cmd != null)
        //        {
        //            cmd.Dispose();
        //        }
        //    }
        //    return msg;
        //}

        /// <summary>
        /// method to update customer details to our database for future use
        /// </summary>
        /// <param name="custId"></param>
        /// <param name="cardId"></param>
        /// <param name="userid"></param>
        public static String UpdateStripeData(UpdateUserInfo obj)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
                cmd.CommandText = "Sbsp_AddUserStripeIds";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", SqlDbType.Int).Value = obj.UserId;
                cmd.Parameters.Add("@cust_id", SqlDbType.VarChar, 500).Value = obj.CustId;
                cmd.Parameters.Add("@card_id", SqlDbType.VarChar, 500).Value = obj.CardId;
                cmd.Parameters.Add("@planId", SqlDbType.Int).Value = obj.PlanId;
                cmd.Parameters.AddWithValue("@amount", SqlDbType.Decimal).Value = Convert.ToDecimal(obj.Amount);
                cmd.Parameters.Add("@StartDate", SqlDbType.SmallDateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(obj.PlanStartDate), timeZone);
                cmd.Parameters.Add("@EndDate", SqlDbType.SmallDateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(obj.PlanEndDate).AddDays(-1), timeZone);
                cmd.Parameters.Add("@TrialEndDate", SqlDbType.SmallDateTime).Value = obj.TrialEndDate == "" ? Convert.ToDateTime("1970-01-01 00:00:00") : TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(obj.TrialEndDate).AddDays(-1), timeZone);
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update StripeData : " + msg, SessionData.SellerName);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update StripeData : " + msg, SessionData.SellerName);
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
        /// method to update customer details to our database for future use
        /// </summary>
        /// <param name="custId"></param>
        /// <param name="cardId"></param>
        /// <param name="userid"></param>
        public static String UpdateStripeCardData(UpdateUserInfo obj)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "UPDATE MppUser SET PlanStatus=1, stp_custId = @cust_id, stp_cardId = @card_id WHERE MppUserID=@userid";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@userid", SqlDbType.Int).Value = obj.UserId;
                cmd.Parameters.Add("@cust_id", SqlDbType.VarChar, 500).Value = obj.CustId;
                cmd.Parameters.Add("@card_id", SqlDbType.VarChar, 500).Value = obj.CardId;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update StripeCard Data : " + msg, SessionData.SellerName);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update StripeCard Data : " + msg, SessionData.SellerName);
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
        /// method to update customer and card ids to our database for future use
        /// </summary>
        /// <param name="custId"></param>
        /// <param name="cardId"></param>
        /// <param name="userid"></param>
        public static String UpdateUserAffiliateCode(String custmid, String code)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_UpdateUserPromo";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@custid", SqlDbType.VarChar, 500).Value = custmid;
                cmd.Parameters.Add("@code", SqlDbType.VarChar, 200).Value = code;
                cmd.Parameters.Add("@date", SqlDbType.SmallDateTime).Value = DateTime.Now;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update PromoCode : " + msg, SessionData.SellerName);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update PromoCode : " + msg, SessionData.SellerName);
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

        public static int CreateCustomPlan(int Id, int Price)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_CreateCustomPlan";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@PlanId", SqlDbType.Int).Value = Id;
                cmd.Parameters.Add("@Price", SqlDbType.Int).Value = Price;
                var rtnValue = DataAccess.ExecuteCommand(cmd);
                return rtnValue;
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Create CustomPlan : " + msg, SessionData.SellerName);
                return 0;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Create CustomPlan : " + msg, SessionData.SellerName);
                return 0;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
        }

        public static int GetPlanId(int Price)
        {
            SqlCommand cmd = new SqlCommand();
            String query = @"SELECT Top 1 PlanId from CustomPlans where Price = @Price";
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@Price", SqlDbType.Int).Value = Price;
            int returnId = Convert.ToInt32(DataAccess.ExecuteScalarCommand(cmd));
            return returnId;
        }

        public static int UserSkuCount(int UserId)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_GetUserSku";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = UserId;
                var rtnValue = Convert.ToInt32(DataAccess.ExecuteScalarCommand(cmd));
                return rtnValue;

            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog(msg, SessionData.SellerName);
                return 0;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog(msg, SessionData.SellerName);
                return 0;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
        }

        public static int UserPlan(int userid)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                String query = @"Select PlanID from MppUser where MppUserID=@userid";
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@userid", SqlDbType.VarChar, 50).Value = userid;
                return Convert.ToInt32(DataAccess.GetDataValue(cmd));

            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog(msg, SessionData.SellerName);
                return 0;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog(msg, SessionData.SellerName);
                return 0;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
        }

        public static DataTable GetSubscribers()
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            try
            {
                String query = @"Select top 1 mpp.MppUserID, stp_custId, stp_cardId,TrailEndDate from MppUser mpp join Billing bl on mpp.MppUserID = bl.MppUserId
                                where bl.PlanEndDate < @NextPlanDate and mpp.PlanStatus = 1 order by bl.ID desc";
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@NextPlanDate", SqlDbType.SmallDateTime).Value = DateTime.Now;
                dt = DataAccess.GetTable(cmd);
            }
            catch (SqlException ex)
            {
                LogFile.WriteLog(ex.Message);
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex.Message);
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
        public static String UpdatePaymentStatus(string custid, double amount, double amount_off, string coupon_id, double discount, DateTime startdate, DateTime enddate, Int16 status)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Ssp_UpdatePaymentStatus";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@custid", SqlDbType.VarChar, 500).Value = custid;
                cmd.Parameters.Add("@status", SqlDbType.Bit).Value = status;
                cmd.Parameters.Add("@amount", SqlDbType.Decimal).Value = amount;
                cmd.Parameters.Add("@amount_off", SqlDbType.Decimal).Value = amount_off;
                cmd.Parameters.Add("@couponid", SqlDbType.VarChar).Value = coupon_id;
                cmd.Parameters.Add("@discount", SqlDbType.Decimal).Value = discount;
                cmd.Parameters.Add("@startdate", SqlDbType.SmallDateTime).Value = startdate;
                cmd.Parameters.Add("@enddate", SqlDbType.SmallDateTime).Value = enddate.AddDays(-1);
                cmd.Parameters.Add("@date", SqlDbType.SmallDateTime).Value = DateTime.Today;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update PaymentStatus : " + msg, SessionData.SellerName);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update PaymentStatus : " + msg, SessionData.SellerName);
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

        //public static void BackupDatabase() 
        //{
        //    try
        //    {

        //        Backup sqlBackup = new Backup();
        //        System.Data.Common.DbConnectionStringBuilder builder = new System.Data.Common.DbConnectionStringBuilder();

        //        builder.ConnectionString = ConfigurationManager.AppSettings["PPCConnStr"];

        //        string server = builder["Server"] as string;
        //        string database = builder["Initial Catalog"] as string;
        //        string UserID = builder["User Id"] as string;
        //        string password = builder["password"] as string;

        //        sqlBackup.Action = BackupActionType.Database;
        //        sqlBackup.BackupSetDescription = "BackUp of:" + database + "on" + DateTime.Now.ToShortDateString();
        //        sqlBackup.BackupSetName = "FullBackUp";
        //        sqlBackup.Database = database;
        //        string backupFolder = @"C:\MppBackups";
        //        bool exists = System.IO.Directory.Exists(backupFolder);
        //        if (!exists)
        //        {
        //            DirectoryInfo di = System.IO.Directory.CreateDirectory(backupFolder);
        //            Console.WriteLine("The Folder is created Sucessfully");
        //        }
        //        else
        //        {
        //            Console.WriteLine("The Folder already exists");
        //        }
        //        DirectoryInfo dInfo = new DirectoryInfo(backupFolder);
        //        DirectorySecurity dSecurity = dInfo.GetAccessControl();
        //        dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
        //        dInfo.SetAccessControl(dSecurity);

        //        var backupFileName = String.Format("{0}-{1}.bak", database, DateTime.Now.ToString("yyyy-MM-dd"));
        //        BackupDeviceItem deviceItem = new BackupDeviceItem(backupFileName, DeviceType.File);

        //        ServerConnection connection = new ServerConnection(server, UserID, password);

        //        Server sqlServer = new Server(connection);
        //        sqlServer.ConnectionContext.StatementTimeout = 60 * 60;
        //        Database db = sqlServer.Databases[database];

        //        sqlBackup.Initialize = true;
        //        sqlBackup.Checksum = true;
        //        sqlBackup.ContinueAfterError = true;


        //        sqlBackup.Devices.Add(deviceItem);
        //        sqlBackup.Incremental = false;

        //        sqlBackup.ExpirationDate = DateTime.Now.AddDays(3);

        //        sqlBackup.LogTruncation = BackupTruncateLogType.Truncate;
        //        sqlBackup.FormatMedia = false;
        //        sqlBackup.SqlBackup(sqlServer);
        //        sqlBackup.Devices.Remove(deviceItem);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

       // delete unusual users
        public List<ArchiveData> GetArchiveData()
        {
            try
            {
                DataTable dt = null;
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Sbsp_ArchiveUserData";
                cmd.CommandType = CommandType.StoredProcedure;
                dt = DataAccess.GetTable(cmd);
                if (dt.Rows.Count > 0)
                {
                    var ArchiveData = dt.AsEnumerable().Select(r => ConvertToArchiveData(r));
                    return ArchiveData.ToList();
                }
                return null;

            }
            catch (Exception ex)
            {
                LogFile.WriteLog("GetArchiveUserData : " + ex.Message);
                return null;
            }
        }

        public String Update48HourAlert(Guid code)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Update MppUserActivation Set Is48HourAlert=1  WHERE ActivationCode=@userId";
                cmd.Parameters.Add("@userId", SqlDbType.UniqueIdentifier).Value = code;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogFile.WriteLog("MppUserActivation - " + code + ": " + res);
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogFile.WriteLog("MppUserActivation - " + code + ": " + res);
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

        private ArchiveData ConvertToArchiveData(DataRow row)
        {
            return new ArchiveData()
            {

                Email = row.Field<String>("Email"),
                FirstName = row.Field<String>("FirstName"),
                LastName = row.Field<String>("LastName"),
                ActivationCode = row.Field<Guid>("ActivationCode")

            };
        }
        #endregion
    }
}

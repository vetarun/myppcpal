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
    public class SellerData
    {
        #region Class Variables and Constants

        #endregion

        #region Constructors

        #endregion

        #region Public Methods

        /// <summary>
        /// Get Seller details
        ///<param name="UserID">UserID</param>
        /// </summary>
        /// <returns></returns>
        public DataTable GetSellerAccessData(Int32 userID)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = @"SELECT SellerName,SellerAccess,BulkAccess,NegAccess from MppUser where MppUserID=@MppUserId";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@MppUserId", SqlDbType.Int).Value = userID;
            dt = DataAccess.GetTable(cmd);
            return dt;
        }

        /// <summary>
        /// Update user with Seller profile details
        ///<param name="UserID">UserID</param>
        ///<param name="SellerName">SellerName</param>
        /// </summary>
        /// <returns></returns>
        public String UpdateSellerAccount(Int32 UserID, String SellerName)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_UpdateSellerName";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@MppUserid", SqlDbType.VarChar).Value = UserID;
                cmd.Parameters.Add("@SellerName", SqlDbType.VarChar, 50).Value = SellerName;
                cmd.Parameters.Add("@Modifiedon", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@modifiedBy", SqlDbType.VarChar, 50).Value = "User";
                res =  Convert.ToString(DataAccess.ExecuteScalarCommand(cmd));
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogFile.WriteLog("Update Seller Name - " + UserID + ": " + res);
            }
            catch (Exception ex)
            {
                LogFile.WriteLog("Update Seller Name - " + UserID + ": " + res);
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

        public String UpdatePaymentAlert(int status,string custId)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"UPDATE Billing set IsAlertSent=@status WHERE MppUserId=(SELECT MppUserID from MppUser where stp_custId=@custId) AND GETDATE() BETWEEN PlanStartDate and PlanEndDate";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@custId", custId);
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
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
        /// Get All Seller email details
        /// </summary>
        /// <returns></returns>
        public DataTable GetUserStatus()
        {
            DataTable dt;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sbsp_GetUserStatus";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Date", SqlDbType.SmallDateTime).Value = DateTime.Now;
            dt = DataAccess.GetTable(cmd);
            return dt;
        }

        /// <summary>
        /// Get All trial expiry user emails
        /// </summary>
        /// <returns></returns>
        public DataSet GetUserEmails()
        {
            DataSet ds;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sbsp_GetUserEmails";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Date", SqlDbType.SmallDateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@Trialdate", SqlDbType.DateTime).Value = DateTime.Now.AddDays(7);
            ds = DataAccess.GetDataSet(cmd);
            return ds;
        }

        /// <summary>
        /// Get All Seller user access email details
        /// </summary>
        /// <returns></returns>
        public void UpdateUserAccessLimit(DataTable tbl)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sbsp_UpdateAccessLimit";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@tbllimit", tbl);
            cmd.Parameters.Add("@ModifiedOn", SqlDbType.DateTime).Value = DateTime.Now;
            DataAccess.ExecuteCommand(cmd);
        }

        /// <summary>
        /// Get All Seller 7 day trial email status
        /// </summary>
        /// <returns></returns>
        public void UpdateSevenTrialEmailStatus(DataTable tbl)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sbsp_UpdateSevenTrialEmailStatus";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@tbltrial", tbl);
            cmd.Parameters.Add("@date", SqlDbType.DateTime).Value = DateTime.Now;
            DataAccess.ExecuteCommand(cmd);
        }

        /// <summary>
        /// Get All Seller trial end email status
        /// </summary>
        /// <returns></returns>
        public void UpdateTrialEndEmailStatus(DataTable tbl)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sbsp_UpdateTrialEndEmailStatus";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@tbltrial", tbl);
            cmd.Parameters.Add("@date", SqlDbType.DateTime).Value = DateTime.Now;
            DataAccess.ExecuteCommand(cmd);
        }

        #endregion

        #region Private Methods

        #endregion
    }
}

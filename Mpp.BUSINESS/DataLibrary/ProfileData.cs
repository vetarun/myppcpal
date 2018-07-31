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
    public class ProfileData
    {
        #region Class Variables and Constants
        #endregion

        #region Constructors
        #endregion

        #region Public Methods
        /// <summary>
        /// User Activation code 
        ///<param name="userID">userID</param>
        ///<param name="ActivationCode">ActivationCode</param>
        /// </summary>
        /// <returns>Affected rows</returns>
        public String UpdateUserProfile(Int32 userID, String Email)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_UpdateUserProfile";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = Email;
                cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = DateTime.Now;
                msg = Convert.ToString(DataAccess.GetDataValue(cmd));
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update User Profile - " + userID + ": " + msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update User Profile - " + userID + ": " + msg);
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
        /// User Activation code 
        ///<param name="userID">userID</param>
        ///<param name="ActivationCode">ActivationCode</param>
        /// </summary>
        /// <returns>Affected rows</returns>
        public String UpdatePassword(Int32 userID, String oldPwd, String newPwd)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_UpdatePassword";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
                cmd.Parameters.Add("@oldPassword", SqlDbType.Binary, 16).Value = MppUtility.EncryptPassword(oldPwd);
                cmd.Parameters.Add("@newPassword", SqlDbType.Binary, 16).Value = MppUtility.EncryptPassword(newPwd);
                cmd.Parameters.Add("@date", SqlDbType.DateTime).Value = DateTime.Now;
                msg = Convert.ToString(DataAccess.GetDataValue(cmd));
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update Password - " + userID + ": " + msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update Password - " + userID + ": " + msg);
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
    }
}

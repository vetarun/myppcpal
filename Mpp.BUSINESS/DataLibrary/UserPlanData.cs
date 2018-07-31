using Mpp.UTILITIES;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Mpp.BUSINESS.DataLibrary
{
    public class UserPlanData
    {
        #region Class Variables and Constants
        #endregion

        #region Constructors
        #endregion

        #region Public Methods

        public DataSet GetPlanData(Int32 UserID)
        {
            DataSet ds;
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_GetUserPlan";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                ds = DataAccess.GetDataSet(cmd);
            }
            catch (Exception ex)
            {
                throw ex;
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
        /// Update Plan
        ///<param name="UserID">UserID</param>
        /// </summary>
        /// <returns></returns>
        public String UpdatePlanData(int UserID, int Planstatus)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                String query = @"UPDATE MppUser set PlanStatus = @Planstatus, ModifiedOn = @ModifiedOn,PlanUpdatedOn=GETDATE() where MppUserID = @UserId";
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserID;
                cmd.Parameters.Add("@Planstatus", SqlDbType.Int).Value = Planstatus;
                cmd.Parameters.Add("@ModifiedOn", SqlDbType.DateTime).Value = DateTime.Now;           
                DataAccess.ExecuteCommand(cmd);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update PlanData - " + UserID + ": " + msg);
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

        public String UpdatePlanData(string custId, int Planstatus)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                String query = @"UPDATE MppUser set PlanStatus = @Planstatus, ModifiedOn = @ModifiedOn where stp_custId = @custId";
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@custId", SqlDbType.VarChar,500).Value = custId;
                cmd.Parameters.Add("@Planstatus", SqlDbType.Int).Value = Planstatus;
                cmd.Parameters.Add("@ModifiedOn", SqlDbType.DateTime).Value = DateTime.Now;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update PlanData - " + custId + ": " + msg);
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

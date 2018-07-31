using Mpp.UTILITIES.BO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.BUSINESS.DataLibrary
{
    public class ServicesData
    {
        /// <summary>
        /// Get All Sellers
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAllSellerIds()
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "select stp_custId from MppUser where PlanStatus != 0 and (stp_custId IS NOT NULL or stp_custId != '') and (stp_cardId IS NOT NULL or stp_cardId != '')";
                cmd.CommandType = CommandType.Text;
                dt = DataAccess.GetTable(cmd);
            }
            catch (SqlException ex)
            {
                LogFile.WriteLog("GetAllSellerIds - " + ex.Message);
            }

            catch (Exception ex)
            {
                LogFile.WriteLog("GetAllSellerIds - " + ex.Message);
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
        public static DataTable GetSubscribers()
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            try
            {
                String query = @"Select mpp.MppUserID, stp_custId, stp_cardId, TrailEndDate from MppUser mpp join Billing bl on mpp.MppUserID = bl.MppUserId
                                where mpp.PlanStatus = 1 and PlanEndDate <=@NextPlanDate and bl.ID IN (select MAX(ID) from Billing group by MppUserID)";
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@NextPlanDate", SqlDbType.DateTime).Value = DateTime.Now;
                dt = DataAccess.GetTable(cmd);
            }
            catch (SqlException ex)
            {
                LogFile.WriteLog("GetSubscribers - " + ex.Message);
            }

            catch (Exception ex)
            {
                LogFile.WriteLog("GetSubscribers - " + ex.Message);
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
        /// method to update customer and card ids to our database for future use
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
                cmd.Parameters.Add("@planId", SqlDbType.VarChar, 500).Value = obj.PlanId;
                cmd.Parameters.AddWithValue("@amount", SqlDbType.Decimal).Value = Convert.ToDecimal(obj.Amount);
                cmd.Parameters.Add("@StartDate", SqlDbType.SmallDateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(obj.PlanStartDate), timeZone);
                cmd.Parameters.Add("@EndDate", SqlDbType.SmallDateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(obj.PlanEndDate).AddDays(-1), timeZone);
                cmd.Parameters.Add("@TrialEndDate", SqlDbType.SmallDateTime).Value = obj.TrialEndDate == "" ? Convert.ToDateTime("1970-01-01 00:00:00") : TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(obj.TrialEndDate).AddDays(-1), timeZone);
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("UpdateStripeData - " + obj.UserId + ": " + msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("UpdateStripeData - " + obj.UserId + ": " + msg);
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
        /// Update Plan
        ///<param name="UserID">UserID</param>
        /// </summary>
        /// <returns></returns>
        public static String UpdatePlanData(int UserID, int Planstatus)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                String query = @"UPDATE MppUser set stp_cardId= @CardId, PlanStatus = @Planstatus, ModifiedOn = @ModifiedOn where MppUserID = @UserId";
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserID;
                cmd.Parameters.Add("CardId", SqlDbType.VarChar).Value = null;
                cmd.Parameters.Add("@Planstatus", SqlDbType.Int).Value = Planstatus;
                cmd.Parameters.Add("@ModifiedOn", SqlDbType.DateTime).Value = DateTime.Now;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("UpdatePlanData - " + UserID + ": " + msg);
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

        public static String UpdatePaymentStatus(string custid, double amount, double amount_off, string coupon_id, double discount, string startdate, string enddate, bool status)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
                cmd.CommandText = "Ssp_UpdatePaymentStatus";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@custid", SqlDbType.VarChar, 500).Value = custid;
                cmd.Parameters.Add("@status", SqlDbType.Bit).Value = status;
                cmd.Parameters.Add("@amount", SqlDbType.Decimal).Value = amount;
                cmd.Parameters.Add("@amount_off", SqlDbType.Decimal).Value = amount_off;
                cmd.Parameters.Add("@couponid", SqlDbType.VarChar).Value = coupon_id;
                cmd.Parameters.Add("@discount", SqlDbType.Decimal).Value = discount;
                cmd.Parameters.Add("@startdate", SqlDbType.SmallDateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(startdate), timeZone);
                cmd.Parameters.Add("@enddate", SqlDbType.SmallDateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(enddate).AddDays(-1), timeZone);
                cmd.Parameters.Add("@date", SqlDbType.DateTime).Value = DateTime.Now;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("UpdatePaymentStatus - " + custid + ": " + msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("UpdatePaymentStatus - " + custid + ": " + msg);
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
                LogFile.WriteLog("CreateCustomPlan - " + Id + ": " + msg);
                return 0;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("CreateCustomPlan - " + Id + ": " + msg);
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

        //public static int UserSkuCount(int userID)
        //{
        //    String msg = "";
        //    SqlCommand cmd = new SqlCommand();
        //    try
        //    {
        //        cmd.CommandText = "Sbsp_GetUserSku";
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.Add("@custId", SqlDbType.VarChar, 500).Value = custId;
        //        var rtnValue = Convert.ToInt32(DataAccess.ExecuteScalarCommand(cmd));
        //        return rtnValue;

        //    }
        //    catch (SqlException ex)
        //    {
        //        msg = ex.Message;
        //        LogFile.WriteLog("UserSkuCount - " + custId + ": " + msg);
        //        return 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        msg = ex.Message;
        //        LogFile.WriteLog("UserSkuCount - " + custId + ": " + msg);
        //        return 0;
        //    }
        //    finally
        //    {
        //        if (cmd != null)
        //        {
        //            cmd.Dispose();
        //        }
        //    }
        //}
    }
}

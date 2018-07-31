using Mpp.UTILITIES;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mpp.UTILITIES.Enums.StatusEnums;
using static Mpp.UTILITIES.Statics;

namespace Mpp.BUSINESS.DataLibrary
{
    public class OptimizeData
    {
        #region Class Variables and Constants

        #endregion

        #region Constructors

        #endregion

        #region Public Methods

        /// <summary>
        /// Get Campaigns to Optimize
        ///<param name="UserID">UserID</param>
        /// </summary>
        /// <returns></returns>
        public List<Optimize> GetMyCampaignsData(Int32 UserID)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"SELECT c.Name AS CampaignName,c.RecordID,c.FormulaID,f.FormulaName,c.Active,ISNULL(s.ID,0) BestSrchTerm
                                    FROM Campaigns AS c INNER JOIN Formula AS f ON c.FormulaID = f.FormulaID 
                                    LEFT JOIN BestSearchTermRequest s ON s.CampaignId=c.RecordID and s.MppUserId=@UserID
                                    WHERE c.MppUserID = @UserID ORDER BY c.Name";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                cmd.Parameters.Add("@date", SqlDbType.SmallDateTime).Value = DateTime.Today.AddDays(-3);
                dt = DataAccess.GetTable(cmd);
                var cams = dt.AsEnumerable().Select(r => ConvertToOptimize(r));
                return cams.ToList();
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
        }

        /// <summary>
        /// Updates formula
        ///<param name="CampID">CampID</param>
        /// </summary>
        /// <returns></returns>
        public String UpdateFormula(Int32 UserID, Int64 CampID, Formula fm)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_UpdateFormulaForCamp";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@MppUserId", SqlDbType.Int).Value = UserID;
                cmd.Parameters.Add("@CampaignID", SqlDbType.BigInt).Value = CampID;
                cmd.Parameters.Add("@FormulaID", SqlDbType.Int).Value = fm.FormulaID;
                cmd.Parameters.Add("@FormulaName", SqlDbType.VarChar).Value = fm.FormulaName;
                cmd.Parameters.Add("@AcosPause", SqlDbType.VarChar).Value = fm.AcosPause;
                cmd.Parameters.Add("@AcosLower", SqlDbType.VarChar).Value = fm.AcosLower;
                cmd.Parameters.Add("@AcosRaise", SqlDbType.VarChar).Value = fm.AcosRaise;
                cmd.Parameters.Add("@AcosNegative", SqlDbType.VarChar).Value = fm.AcosNegative;
                cmd.Parameters.Add("@SpendPause", SqlDbType.VarChar).Value = fm.SpendPause;
                cmd.Parameters.Add("@SpendLower", SqlDbType.VarChar).Value = fm.SpendLower;
                cmd.Parameters.Add("@SpendNegative", SqlDbType.VarChar).Value = fm.SpendNegative;
                cmd.Parameters.Add("@ClicksPause", SqlDbType.VarChar).Value = fm.ClicksPause;
                cmd.Parameters.Add("@ClicksLower", SqlDbType.VarChar).Value = fm.ClicksLower;
                cmd.Parameters.Add("@ClicksNegative", SqlDbType.VarChar).Value = fm.ClicksNegative;
                cmd.Parameters.Add("@CTRPause", SqlDbType.VarChar).Value = fm.CTRPause;
                cmd.Parameters.Add("@CTRLower", SqlDbType.VarChar).Value = fm.CTRLower;
                cmd.Parameters.Add("@CTRNegative", SqlDbType.VarChar).Value = fm.CTRNegative;
                cmd.Parameters.Add("@BidRaise", SqlDbType.VarChar).Value = fm.BidRaise;
                cmd.Parameters.Add("@BidLower", SqlDbType.VarChar).Value = fm.BidLower;
                cmd.Parameters.Add("@MinBid", SqlDbType.VarChar).Value = fm.MinBid;
                cmd.Parameters.Add("@MaxBid", SqlDbType.VarChar).Value = fm.MaxBid;
                cmd.Parameters.Add("@ModifiedOn", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@isBestSrch", SqlDbType.Bit).Value = fm.IsBestSrchCheked;
                cmd.Parameters.Add("@bestSrchAcos", SqlDbType.VarChar).Value = fm.BestSearchACos;
                cmd.Parameters.Add("@bestSrchImpression", SqlDbType.VarChar).Value = fm.BestSearchImpressons;
                DataAccess.ExecuteCommand(cmd);
            }


            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Update FormulaData - " + UserID + ": " + msg);
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
        /// Get formula
        ///<param name="CampID">campID</param>
        /// </summary>
        /// <returns></returns>
        /// 
        public string RevertUpdates(Int64 KeyID, Int64 CID, Int64 AID, Int32 RID, Int32 RSNID, String ModifiedOn, Int32 userID)
        {
            DateTime date = Convert.ToDateTime(ModifiedOn);
            String res = "";
          
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_RevertKeyUpdates";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                cmd.Parameters.Add("@ModifiedOn", SqlDbType.DateTime).Value = ModifiedOn;
                cmd.Parameters.Add("@ReportID", SqlDbType.Int).Value = RID;
                cmd.Parameters.Add("@ReasonID", SqlDbType.Int).Value = RSNID;
                cmd.Parameters.Add("@KeyID", SqlDbType.BigInt).Value = KeyID;
                cmd.Parameters.Add("@CampID", SqlDbType.BigInt).Value = CID;
                cmd.Parameters.Add("@AdGroupID", SqlDbType.BigInt).Value = AID;
                DataAccess.ExecuteCommand(cmd);
                return res;
               
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
        }
        
        public Formula GetFormulaData(Int64 campID, Int32 userID)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_GetFormulaForCamp";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = userID;
                cmd.Parameters.Add("@RecordID", SqlDbType.BigInt).Value = campID;
                dt = DataAccess.GetTable(cmd);
                var res = ConvertToFormula(dt.Rows[0]);
                return res;
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
        }
        //hide warning 
        public String HideWarning(Int32 userID)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                
                cmd.CommandText = "Sbsp_hideWarning";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                cmd.Parameters.Add("@PopUpID", SqlDbType.Int).Value = 1;
               
               
                DataAccess.ExecuteCommand(cmd);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Activate Campaign - " + userID + ": " + msg);
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
        /// Update campaign formula
        ///<param name="CampID">campID</param>
        ///<param name="pasteID">pasteID</param>
        /// </summary>
        /// <returns></returns>
        public String PasteCampFormulaData(Int32 userID, Int64 copyID, Int64 pasteID)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_ApplyCampFormula";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                cmd.Parameters.Add("@CopyCampID", SqlDbType.BigInt).Value = copyID;
                cmd.Parameters.Add("@PasteCampID", SqlDbType.BigInt).Value = pasteID;
                cmd.Parameters.Add("@ModifiedOn", SqlDbType.DateTime).Value = DateTime.Now;
                DataAccess.ExecuteCommand(cmd);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Paste FormulaData - " + userID + ": " + msg);
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
            return msg;
        }

        /// <summary>
        /// Campaign formula activation
        ///<param name="dt">dt</param>
        ///<param name="UserID">UserID</param>
        /// </summary>
        /// <returns></returns>
        public String PasteAllCampaignFormulaData(Int64 cID, Int32 UserID)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"Sbsp_ApplyFormulaOnAllCamp";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@copyid", cID);
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                DataAccess.ExecuteCommand(cmd);
                return res;
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogFile.WriteLog("PasteAll FormulaData - " + UserID + ": " + res);
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogFile.WriteLog("PasteAll FormulaData - " + UserID + ": " + res);
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
        /// Run campaign formula
        ///<param name="CampID">campID</param>
        ///<param name="status">status</param>
        /// </summary>
        /// <returns></returns>
        public String RunCampaignData(Int32 userID, Int64 CampID, Int32 status)
        {
            String msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {              
                String query = "Sbsp_UpdateMyCampaignStatus";
                cmd.CommandText = query;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                cmd.Parameters.Add("@CampId", SqlDbType.BigInt).Value = CampID;
                cmd.Parameters.Add("@Status", SqlDbType.VarChar, 50).Value = status;
                cmd.Parameters.Add("@ModifiedOn", SqlDbType.DateTime).Value = DateTime.Now;
                DataAccess.ExecuteScalarCommand(cmd);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("Activate Campaign - " + userID + ": " + msg);
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
        /// Campaign formula activation
        ///<param name="dt">dt</param>
        ///<param name="UserID">UserID</param>
        /// </summary>
        /// <returns></returns>
        public String RunCampaignData(DataTable tbl, Int32 UserID)
        {
            String res = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = @"Sbsp_UpdateAllCampaignsStatus";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tblCamp", tbl);
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                DataAccess.ExecuteCommand(cmd);
                return res;
            }
            catch (SqlException ex)
            {
                res = ex.Message;
                LogFile.WriteLog("Activate Bulk Campaigns - " + UserID + ": " + res);
            }
            catch (Exception ex)
            {
                res = ex.Message;
                LogFile.WriteLog("Activate Bulk Campaigns - " + UserID + ": " + res);
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
        /// Campaigns Log
        ///<param name="recordID">recordID</param>
        ///<param name="userID">userID</param>
        /// </summary>
        /// <returns></returns>
        public List<Log> GetCampaignLogData(Int64 CampID, Int32 userID, out string firstCampaign)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            var res = new List<Log>();

            try
            {
                var fstCampaign = "";
                cmd.CommandText = @"SELECT cdtl.CampaignLogID, cdtl.ModifiedField, CONVERT(varchar(50), cdtl.OldValue) as OldValue, CONVERT(varchar(50), cdtl.NewValue) as NewValue,
                                  cl.ModifiedOn FROM CampaignLog cl, CampaignLogDtl cdtl
                                  WHERE cl.CampaignLogID = cdtl.CampaignLogID and cl.RecordID=(CASE WHEN @RecordID=0 THEN ( SELECT TOP 1 RecordID FROM CampaignSixtyDay WHERE MppUserID=@UserID ORDER BY CreatedOn asc)ELSE @RecordID END ) and cl.MppUserID=@UserID order by cl.ModifiedOn desc;
                                      SELECT TOP 1 Name FROM Campaigns WHERE MppUserID=@UserID ORDER BY CreatedOn asc";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@RecordID", SqlDbType.BigInt).Value = CampID;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                var ds = DataAccess.GetDataSet(cmd);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    res = ds.Tables[0].AsEnumerable().Select(r => ConvertToCampLog(r)).ToList();
                    if (ds.Tables[1].Rows.Count > 0)
                        fstCampaign = Convert.ToString(ds.Tables[1].Rows[0][0]);
                }
                firstCampaign = fstCampaign;
                return res;
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex.Message);
                firstCampaign = null;
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
        /// Keywords Log
        ///<param name="recordID">recordID</param>
        /// </summary>
        /// <returns></returns>
        public List<Log> GetKeywordLogData(long RecordId, Int32 userID)
        {
            DataSet ds = new DataSet();
            DataTable dt, dt1, dt2;
           
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "Sbsp_GetOptimizeKeyLog";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@CampId", SqlDbType.BigInt).Value = RecordId;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                ds = DataAccess.GetDataSet(cmd);
                dt = ds.Tables[0];
                dt1 = ds.Tables[1];
                dt2 = ds.Tables[2];
               
                
                var log1 = new List<Log>();
                var log2 = new List<Log>();
                var log3 = new List<Log>();

            
                if (dt.Rows.Count > 0)
                    log1 = dt.AsEnumerable().Select(r => ConvertToKeyLog(r, 1)).ToList();
                if (dt1.Rows.Count > 0)
                    log2 = (dt1.AsEnumerable().Select(r => ConvertToKeyLog(r, 2)).ToList());
                if (dt2.Rows.Count > 0)
                    log3 = (dt2.AsEnumerable().Select(r => ConvertToKeyLog(r, 3)).ToList());
               
                var logs = new List<Log>(log1.Count + log2.Count + log3.Count);
                logs.AddRange(log1);
                logs.AddRange(log2);
                logs.AddRange(log3);
               

                return logs;
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
        }

        /// <summary>
        /// Keywords Log
        ///<param name="recordID">recordID</param>
        /// </summary>
        /// <returns></returns>
        public List<object> GetSellerAccessLogData(Int32 userID)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            List<object> obj = new List<object>();
            try
            {
                cmd.CommandText = "Sbsp_GetUserAccessLog";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userID;
                dt = DataAccess.GetTable(cmd);
                foreach (DataRow dr in dt.Rows)
                {
                    var arr = new string[] { "SellerAccess", "BulkAccess", "NegAccess" };
                    var oldVal = dr["OldValue"];
                    var newVal = dr["NewValue"];
                    if (arr.Contains(dr["ModifiedField"]))
                    {
                        if (Convert.ToString(oldVal) != "-")
                            oldVal = Enum.GetName(typeof(AccessStatus), Convert.ToInt32(oldVal));
                        if (Convert.ToString(newVal) != "-")
                            newVal = Enum.GetName(typeof(AccessStatus), Convert.ToInt32(newVal));

                    }
                    obj.Add(
                        new
                        {
                            ModifiedOn = Convert.ToDateTime(dr["ModifiedOn"]).ToString("MMM dd, yyyy"),
                            ModifiedBy = dr["ModifiedBy"],
                            ModifiedField = dr["ModifiedField"],
                            OldValue = oldVal,
                            NewValue = newVal
                        }
                        );
                }
                return obj;
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
        }
        #endregion

        #region Private Methods
        private Formula ConvertToFormula(DataRow row)
        {
            var bestAcos = row.Field<Decimal>("BestACoSCutOff");
            var bestImp = row.Field<Decimal>("BestImpressionCutOff");
            return new Formula()
            {
                FormulaID = row.Field<Int32>("FormulaID"),
                FormulaName = row.Field<String>("FormulaName"),
                AcosPause = row.Field<Decimal>("AcosPause"),
                AcosLower = row.Field<Decimal>("AcosLower"),
                AcosRaise = row.Field<Decimal>("AcosRaise"),
                AcosNegative = row.Field<Decimal>("AcosNegative"),
                SpendPause = row.Field<Decimal>("SpendPause"),
                SpendLower = row.Field<Decimal>("SpendLower"),
                SpendNegative = row.Field<Decimal>("SpendNegative"),
                ClicksPause = row.Field<Decimal>("ClicksPause"),
                ClicksLower = row.Field<Decimal>("ClicksLower"),
                ClicksNegative = row.Field<Decimal>("ClicksNegative"),
                CTRPause = row.Field<Decimal>("CTRPause"),
                CTRLower = row.Field<Decimal>("CTRLower"),
                CTRNegative = row.Field<Decimal>("CTRNegative"),
                BidRaise = row.Field<Decimal>("BidRaise"),
                BidLower = row.Field<Nullable<Decimal>>("BidLower"),
                MinBid = row.Field<Decimal>("MinBid"),
                MaxBid = row.Field<Decimal>("MaxBid"),
                BestSearchACos= bestAcos>0?bestAcos: row.Field<Decimal>("AcosRaise"),
                BestSearchImpressons = bestImp>0?bestImp:row.Field<Decimal>("CTRPause"),
                IsBestSrchCheked = row.Field<int>("BSTId")>0
            };
        }
        private Optimize ConvertToOptimize(DataRow row)
        {
            return new Optimize()
            {
                RecordID = row.Field<Int64>("RecordID"),
                Name = row.Field<String>("CampaignName"),
                FormulaName = row.Field<String>("FormulaName"),
                Status = row.Field<Boolean>("Active"),
                FormulaID = row.Field<Int32>("FormulaID"),
                IsBestSrchTerm= row.Field<Int32>("BestSrchTerm")>0
            };
        }
        public Log ConvertToCampLog(DataRow row)
        {
            return new Log()
            {
                LogID = row.Field<Int32>("CampaignLogID"),
                ModifiedOn = MppUtility.FullDateString(row.Field<DateTime>("ModifiedOn")),
                ModifiedField = row.Field<String>("ModifiedField"),
                OldValue = row.Field<String>("OldValue"),
                NewValue = row.Field<String>("NewValue"),
                Time = MppUtility.TimeToString(row.Field<DateTime>("ModifiedOn")),
            };
        }
        public Log ConvertToKeyLog(DataRow row, int type)
        {
            return new Log()
            {
                RevertStatus= (type != 1)? row.Field<Int32>("RevertStatus"): 0,
                PopUpID = row.Field<Int32>("PopUpID"),
                ModifiedOn = MppUtility.FullDateString(row.Field<DateTime>("ModifiedOn")),
                ModifiedOn1 = row.Field<DateTime>("ModifiedOn").ToString("yyyy-MM-dd"),
                MatchType = (type != 1) ? row.Field<String>("MatchType"): "",
                ModifiedField = row.Field<String>("ModifiedField"),
                OldValue = row.Field<String>("OldValue"),
                NewValue = row.Field<String>("NewValue"),
                KeywordID = (type != 1) ? row.Field<Int64>("KeywordID"): 0,
                ReportID= (type != 1) ? row.Field<Int32>("ReportID"): 0,
                ReasonID = (type != 1) ? row.Field<Int32>("ReasonID"): 0,
                CampID = (type != 1) ? row.Field<Int64>("CampaignID"): 0,
                AdGroupID= (type != 1) ? row.Field<Int64>("AdgroupID"): 0,
                KeywordName = row.Field<String>("KeyName"),
                AdGroupName = row.Field<String>("AdGroupName"),
                Reason = row.Field<String>("Reasondesc")
            };
        }
        #endregion
    }
}

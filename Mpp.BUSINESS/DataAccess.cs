using Mpp.UTILITIES;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.BUSINESS
{
    public class DataAccess
    {
        #region Class Variables and Constants

        private static String ConnStr = String.Empty;

        #endregion

        #region Constructors
        static DataAccess()
        {
            ConnStr = MppUtility.ReadConfig("PPCConnStr");
        }
        #endregion

        #region Public Methods

        public static DataTable GetTable(String query, Boolean IsStoredProc)
        {
            DataTable tbl = null;
            SqlCommand cmd = null;
            try
            {
                cmd = GetDbCommand();
                cmd.CommandText = query;
                if (cmd.CommandTimeout == 30)
                {
                    cmd.CommandTimeout = 90;
                }
                if (IsStoredProc)
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                }
                tbl = new DataTable();
                SqlDataReader dr = cmd.ExecuteReader();
                tbl.Load(dr);
                dr.Close();
                dr.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Connection.Close();
                    cmd.Dispose();
                }
            }
            return tbl;

        }

        public static DataTable GetTable(SqlCommand cmd)
        {
            DataTable tbl = null;
            SqlConnection conn = null;
            try
            {
                conn = GetDbConnection();
                cmd.Connection = conn;
                tbl = new DataTable();
                if (cmd.CommandTimeout == 30)
                {
                    cmd.CommandTimeout = 90;
                }
                SqlDataReader dr = cmd.ExecuteReader();
                tbl.Load(dr);

                dr.Close();
                dr.Dispose();
        }
            catch (Exception ex)
            {
                //LogFile.WriteLog(ex.ToString());
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            return tbl;
        }

        public static DataSet GetDataSetForBulk(SqlCommand cmd)
        {
            DataSet ds = null;
            SqlConnection conn = null;
            try
            {
                conn = GetDbConnection();
                cmd.Connection = conn;
                if (cmd.CommandTimeout == 30)
                {
                    cmd.CommandTimeout = 90;
                }

                ds = new DataSet();
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(ds);
            }
            catch (Exception ex)
            {
                //LogFile.WriteLog(ex.ToString());
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            return ds;
        }

        public static Object GetDataValue(String query)
        {
            Object retValue = null;
            SqlCommand cmd = null;
            try
            {
                cmd = GetDbCommand();
                cmd.CommandText = query;
                if (cmd.CommandTimeout == 30)
                {
                    cmd.CommandTimeout = 90;
                }
                retValue = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Connection.Close();
                    cmd.Dispose();
                }
            }
            return retValue;
        }

        public static Object GetDataValue(SqlCommand cmd)
        {
            Object retValue = null;
            SqlConnection conn = null;
            try
            {
                conn = GetDbConnection();
                cmd.Connection = conn;
                if (cmd.CommandTimeout == 30)
                {
                    cmd.CommandTimeout = 90;
                }
                retValue = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                //LogFile.WriteLog(ex.ToString());
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            return retValue;
        }

        public static Object ExecuteScalarCommand(SqlCommand cmd)
        {
            Object retValue = null;
            SqlConnection conn = null;
            try
            {
                conn = GetDbConnection();
                cmd.Connection = conn;
                if (cmd.CommandTimeout == 30)
                {
                    cmd.CommandTimeout = 90;
                }
                foreach (SqlParameter Parameter in cmd.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                retValue = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            return retValue;
        }

        public static Object ExecuteScalarCommandForBulk(SqlCommand cmd)
        {
            Object retValue = null;
            SqlConnection conn = null;
            try
            {
                conn = GetDbConnection();
                cmd.Connection = conn;
                if (cmd.CommandTimeout == 30)
                {
                    cmd.CommandTimeout = 90;
                }
                if(cmd.CommandTimeout == 90)
                {
                    cmd.CommandTimeout = 150;
                }
                foreach (SqlParameter Parameter in cmd.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                retValue = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            return retValue;
        }

        public static Int32 ExecuteCommand(SqlCommand cmd)
        {
            Int32 retValue = 0;
            SqlConnection conn = null;
            try
            {
                conn = GetDbConnection();
                cmd.Connection = conn;
                if (cmd.CommandTimeout == 30)
                {
                    cmd.CommandTimeout = 90;
                }
                foreach (SqlParameter Parameter in cmd.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                retValue = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            return retValue;
        }

        public static Int32 ExecuteCommandForBulk(SqlCommand cmd)
        {
            Int32 retValue = 0;
            SqlConnection conn = null;
            try
            {
                conn = GetDbConnection();
                cmd.Connection = conn;
                if (cmd.CommandTimeout == 30)
                {
                    cmd.CommandTimeout = 90;
                }

                if (cmd.CommandTimeout == 90)
                {
                    cmd.CommandTimeout = 150;
                }

                foreach (SqlParameter Parameter in cmd.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                retValue = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            return retValue;
        }

        public static DataSet GetDataSet(SqlCommand cmd)
        {
            DataSet ds = null;
            SqlConnection conn = null;
            try
            {
                conn = GetDbConnection();
                cmd.Connection = conn;
                if (cmd.CommandTimeout == 30)
                {
                    cmd.CommandTimeout = 90;
                }
                ds = new DataSet();
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(ds);
            }
            catch (Exception ex)
            {
                //LogFile.WriteLog(ex.ToString());
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            return ds;
        }

        public static void ExecuteBulkCommand(DataTable dt, String Dtablname)
        {
            SqlConnection conn = null;
            try
            {
                conn = GetDbConnection();
                SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn);
                sqlBulkCopy.BatchSize = dt.Rows.Count;
                sqlBulkCopy.DestinationTableName = Dtablname;
                sqlBulkCopy.WriteToServer(dt);
            }
            catch (Exception ex)
            {
                //LogFile.WriteLog(ex.ToString());
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
        #endregion

        #region Private Methods

        private static SqlCommand GetDbCommand()
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;

            conn = new SqlConnection(ConnStr);
            conn.Open();
            cmd = new SqlCommand();
            cmd.Connection = conn;
            return cmd;
        }

        private static SqlConnection GetDbConnection()
        {
            SqlConnection conn = null;
            conn = new SqlConnection(ConnStr);
            conn.Open();
            return conn;
        }

        #endregion
    }
}

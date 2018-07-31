using Mpp.BUSINESS.DataLibrary;
using Mpp.UTILITIES;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace Mpp.WEB
{
    /// <summary>
    /// Summary description for UsersCSV
    /// </summary>
    public class UsersCSV : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            var response = System.Web.HttpContext.Current.Response;
            if (HttpContext.Current.Session["AdminUserID"] != null)
            {
                var adata = new AdminData();
                var dtbl = adata.GetActiveUsersData();
                StringBuilder sb = new StringBuilder();

                if (dtbl.Rows.Count > 0)
                {
                    //headers  
                    foreach (DataColumn dc in dtbl.Columns)
                    {
                        sb.Append(MppUtility.FormatCSV(dc.ColumnName.ToString()) + ",");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append(Environment.NewLine);
                    foreach (DataRow dr in dtbl.Rows)
                    {
                        foreach (DataColumn dc in dtbl.Columns)
                        {
                            sb.Append(MppUtility.FormatCSV(dr[dc.ColumnName].ToString()) + ",");
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append(Environment.NewLine);
                    }

                    response.ClearContent();
                    response.Clear();
                    response.ContentType = "text/csv";
                    response.AddHeader("content-disposition", "attachment; filename= MyPPCPal_ActiveUsers.csv");
                    response.BufferOutput = true;
                    response.Cache.SetCacheability(HttpCacheability.NoCache);
                    response.Write(sb.ToString());
                    response.Flush();
                    response.End();
                }
                else
                {
                    response.Write("Active users were not found");
                }
            }else
            {
                response.Redirect("admin/account/login");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
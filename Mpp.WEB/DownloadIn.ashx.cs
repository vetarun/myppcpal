using Mpp.BUSINESS.DataLibrary;
using Mpp.UTILITIES;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Mpp.WEB
{
    /// <summary>
    /// Summary description for DownloadIn
    /// </summary>L
    public class DownloadIn : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            Int32 userid = 0;


            System.Web.HttpRequest request = System.Web.HttpContext.Current.Request;
            int ClientuserId = Convert.ToInt32(request.QueryString["userId"]);
            if (ClientuserId > 0)
            {
                userid = ClientuserId;
            }
            else
            {
                userid = SessionData.UserID;
            }
            DateTime date = Convert.ToDateTime(request.QueryString["date"]);
            int type = Convert.ToInt32(request.QueryString["type"]);
            String fileName = String.Empty;

            if (type == 1)
                fileName = "MyPPCPal_KeywordReport_" + date + ".csv";
            //else if(type==2)
            //    fileName = "MyPPCPal_OptimizedKeywordReport_" + date + ".csv";
            else if (type == 3)
                fileName = "MyPPCPal_SearchTermReport_" + date + ".csv";
            //else if (type == 4)
            //    fileName = "MyPPCPal_OptimizedSearchTermReport_" + date + ".csv";
            else if (type == 5)
                fileName = "MyPPCPal_KeywordOptimized_" + date + ".csv";
            else if (type == 6)
                fileName = "MyPPCPal_SearchTermOptimized_" + date + ".csv";

            var response = System.Web.HttpContext.Current.Response;
            if (userid > 0)
            {
                var adata = new ReportsData();
                var dataset = adata.GetReports(userid, date, type);


                if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    if (type == 1 || type == 5)
                    {
                        KeywordReportGenerator(dataset, out sb);
                    }
                    else
                    {
                        KeywordSearchTermReportGenerator(dataset, out sb);
                    }

                    sb.Remove(sb.Length - 1, 1);
                    sb.Append(Environment.NewLine);
                    response.ClearContent();
                    response.Clear();
                    response.ContentType = "text/csv";
                    response.AddHeader("content-disposition", "attachment; filename=" + fileName + "");
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
            }
            else
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

        private void KeywordReportGenerator(DataSet dataset, out StringBuilder sb)
        {
            sb = new StringBuilder();
            var dtbl = dataset.Tables[0];
            var dtTotal = dataset.Tables[1];
            //headers  
            foreach (DataColumn dc in dtbl.Columns)
            {
                sb.Append(MppUtility.FormatCSV(dc.ColumnName.ToString().Replace('_', ' ')) + ",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(Environment.NewLine);

           // var flag = false;
            var i = 1;
           
            foreach (DataRow dr in dtbl.Rows)
            {

                var nextKey = "";
                var CurrentKey = dr["Keyword"].ToString().ToLower().Replace(" ","_");
                if (i < dtbl.Rows.Count)
                    nextKey = dtbl.Rows[i]["Keyword"].ToString().ToLower().Replace(" ", "_");
                if (CurrentKey == nextKey)
                {
                   
                    foreach (DataColumn dc in dtbl.Columns)
                    {
                        sb.Append(MppUtility.FormatCSV(dr[dc.ColumnName].ToString()) + ",");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append(Environment.NewLine);
                   // flag = true;
                    i++;
                    continue;

                }
                else if (nextKey != "")
                {
                    try
                    {
                        foreach (DataColumn dc in dtbl.Columns)
                        {
                            sb.Append(MppUtility.FormatCSV(dr[dc.ColumnName].ToString()) + ",");
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append(Environment.NewLine);
                      //  if (flag)
                       // {

                            var CurrentRow = (from row in dtTotal.AsEnumerable()
                                              where row.Field<string>("Keyword").ToLower().Replace(" ", "_") == CurrentKey
                                              select row).FirstOrDefault();
                            foreach (DataColumn dc in dtbl.Columns)
                            {
                                var columnName = dc.ColumnName == "Campaign_Name" ? "Total" : null;
                                if (dtTotal.Columns.IndexOf(dc.ColumnName) < 0)
                                {
                                    sb.Append(MppUtility.FormatCSV(columnName) + ",");
                                }
                                else
                                {
                                    sb.Append(MppUtility.FormatCSV(CurrentRow[dc.ColumnName].ToString()) + ",");
                                }
                            }
                            sb.Remove(sb.Length - 1, 1);
                            sb.Append(Environment.NewLine);
                           // flag = false;
                       // }
                        i++;
                        
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                    foreach (DataColumn dc in dtbl.Columns)
                    {
                        sb.Append(MppUtility.FormatCSV(dr[dc.ColumnName].ToString()) + ",");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append(Environment.NewLine);

                    //if (flag)
                 //   {
                        var CurrentRow = (from row in dtTotal.AsEnumerable()
                                          where row.Field<string>("Keyword").ToLower().Replace(" ", "_") == CurrentKey
                                          select row).FirstOrDefault();
                        foreach (DataColumn dc in dtbl.Columns)
                        {
                            var columnName = dc.ColumnName == "Campaign_Name" ? "Total" : null;
                            if (dtTotal.Columns.IndexOf(dc.ColumnName) < 0)
                            {
                                sb.Append(MppUtility.FormatCSV(columnName) + ",");
                            }
                            else
                            {
                                sb.Append(MppUtility.FormatCSV(CurrentRow[dc.ColumnName].ToString()) + ",");
                            }
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append(Environment.NewLine);
                    //}
                    //flag = false;
                   

                }


            }
        }

        private void KeywordSearchTermReportGenerator(DataSet dataset, out StringBuilder sb)
        {
            sb = new StringBuilder();
            var dtbl = dataset.Tables[0];
            var dtTotal = dataset.Tables[1];
            //headers  
            foreach (DataColumn dc in dtbl.Columns)
            {
                sb.Append(MppUtility.FormatCSV(dc.ColumnName.ToString().Replace('_', ' ')) + ",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(Environment.NewLine);

           // var flag = false;
            var i = 1;
            
            foreach (DataRow dr in dtbl.Rows)
            {

                var nextKey = "";
                var nextQuery = "";
                var CurrentKey = dr["Keyword"].ToString().ToLower().Replace(" ", "_");
                var currentQuery = dr["Search_Term"].ToString().ToLower().Replace(" ", "_");
                if (i < dtbl.Rows.Count)
                {
                    nextKey = dtbl.Rows[i]["Keyword"].ToString().ToLower().Replace(" ", "_");
                    nextQuery = dtbl.Rows[i]["Search_Term"].ToString().ToLower().Replace(" ", "_");
                }
                if (CurrentKey == nextKey && currentQuery==nextQuery)
                {
                    
                    foreach (DataColumn dc in dtbl.Columns)
                    {
                        sb.Append(MppUtility.FormatCSV(dr[dc.ColumnName].ToString()) + ",");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append(Environment.NewLine);
                  //  flag = true;
                    i++;
                    continue;

                }
                else if (nextKey != "" && nextQuery!="")
                {
                    try
                    {
                        foreach (DataColumn dc in dtbl.Columns)
                        {
                            sb.Append(MppUtility.FormatCSV(dr[dc.ColumnName].ToString()) + ",");
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append(Environment.NewLine);
                       // if (flag)
                       // {

                            var CurrentRow = (from row in dtTotal.AsEnumerable()
                                              where row.Field<string>("Keyword").ToLower().Replace(" ", "_") == CurrentKey &&
                                                   row.Field<string>("Search_Term").ToLower().Replace(" ", "_") == currentQuery
                                              select row).FirstOrDefault();
                            foreach (DataColumn dc in dtbl.Columns)
                            {
                                var columnName = dc.ColumnName == "Campaign_Name" ? "Total" : null;
                                if (dtTotal.Columns.IndexOf(dc.ColumnName) < 0)
                                {
                                    sb.Append(MppUtility.FormatCSV(columnName) + ",");
                                }
                                else
                                {
                                    sb.Append(MppUtility.FormatCSV(CurrentRow[dc.ColumnName].ToString()) + ",");
                                }
                            }
                            sb.Remove(sb.Length - 1, 1);
                            sb.Append(Environment.NewLine);
                           
                           // flag = false;
                            
                     //   }
                        i++;

                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                    foreach (DataColumn dc in dtbl.Columns)
                    {
                        sb.Append(MppUtility.FormatCSV(dr[dc.ColumnName].ToString()) + ",");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append(Environment.NewLine);

                    //if (flag)
                   // {
                        var CurrentRow = (from row in dtTotal.AsEnumerable()
                                          where row.Field<string>("Keyword").ToLower().Replace(" ", "_") == CurrentKey &&
                                                row.Field<string>("Search_Term").ToLower().Replace(" ", "_") == currentQuery
                                          select row).FirstOrDefault();
                        foreach (DataColumn dc in dtbl.Columns)
                        {
                            var columnName = dc.ColumnName == "Campaign_Name" ? "Total" : null;
                            if (dtTotal.Columns.IndexOf(dc.ColumnName) < 0)
                            {
                                sb.Append(MppUtility.FormatCSV(columnName) + ",");
                            }
                            else
                            {
                                sb.Append(MppUtility.FormatCSV(CurrentRow[dc.ColumnName].ToString()) + ",");
                            }
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append(Environment.NewLine);
                  //  }
                   // flag = false;
                   

                }


            }
        }
    }
}
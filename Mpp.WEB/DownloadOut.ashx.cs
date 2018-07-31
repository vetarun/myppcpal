using Mpp.UTILITIES;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace Mpp.WEB
{
    /// <summary>
    /// Summary description for DownloadOut
    /// </summary>
    public class DownloadOut : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            Int32 userid = SessionData.UserID;
            String uploadFileDir = ConfigurationManager.AppSettings["UploadFolderPath"];
            System.Web.HttpRequest request = System.Web.HttpContext.Current.Request;
            string startDate = request.QueryString["startDate"];
            string endDate = request.QueryString["endDate"];
            string uploadfilepath = MppUtility.GetFilelocation(userid, uploadFileDir, "bulk");
            string fileName = ConfigurationManager.AppSettings["filename"];
            string fileName1 = fileName.PadRight(29) + MppUtility.DateFormat(Convert.ToDateTime(startDate), 2) + "-" + MppUtility.DateFormat(Convert.ToDateTime(endDate), 2) + ".csv";
            string filePath = Path.Combine(uploadfilepath, fileName1);
            bool test = System.IO.File.Exists(filePath);
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            if (System.IO.File.Exists(filePath))
            {
                response.ClearContent();
                response.Clear();
                byte[] Content = System.IO.File.ReadAllBytes(filePath);
                response.ContentType = "text/csv";
                response.AddHeader("content-disposition", "attachment; filename=" + fileName1 + ".csv");
                response.BufferOutput = true;
                response.Cache.SetCacheability(HttpCacheability.NoCache);
                response.OutputStream.Write(Content, 0, Content.Length);
                response.Flush();
                response.End();
            }
            else
            {
                response.Redirect("ShowStatus.html");
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
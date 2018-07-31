using Mpp.UTILITIES;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.BUSINESS
{
    public class LogAPI
    {
        #region 
        private static long MAX_FILE_SIZE = 5000;  //maximum size of log file, 5KB
        #endregion

        #region Public Methods
        /// <summary>
        /// write a message in client specific log file
        /// </summary>
        /// <param name="msg">message to be written into log file</param>
        /// <param name="clientName">if not blank, msg will be written in log file named as PrimePay_clientName</param>
        public static void WriteLog(String msg, String clientName)
        {
            if (clientName == null)
                clientName = String.Empty;
            WriteLog(false, msg, false, clientName);
        }

        /// <summary>
        /// write a message in log file
        /// </summary>
        /// <param name="msg">message to be written into log file</param>
        public static void WriteLog(String msg)
        {
            WriteLog(false, msg, false, String.Empty);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// actually write the message in log file
        /// </summary>
        /// <param name="bBeginNewLine">start a new line or not</param>
        /// <param name="msg">msg to be written into log file</param>
        /// <param name="bEndNewLine">the message ends with a new line or not</param>
        /// <param name="clientName">if not blank, msg will be written in log file named as PrimePay_clientName</param>
        private static void WriteLog(bool bBeginNewLine, string msg, bool bEndNewLine, string clientName)
        {
            string auditErrorString;
            FileStream objFileStream;
            StreamWriter stmWriter;
            string ErrMsg;

            try
            {
                string auditLogFilePath = getLogDir();

                string auditFileName = getLogFile(clientName);

                if (Directory.Exists(auditLogFilePath) == false)
                    Directory.CreateDirectory(auditLogFilePath);

                if (File.Exists(auditFileName) == false)
                {
                    objFileStream = new FileStream(auditFileName, FileMode.Create, FileAccess.Write);
                }
                else
                {
                    objFileStream = new FileStream(auditFileName, FileMode.Append, FileAccess.Write);
                }

                stmWriter = new StreamWriter(objFileStream);
                auditErrorString = System.DateTime.Now.ToString() + "|" + msg;

                if (bBeginNewLine)
                    stmWriter.WriteLine(stmWriter.NewLine);

                if (msg.Trim().Length > 0)
                    stmWriter.WriteLine(auditErrorString);

                if (bEndNewLine)
                    stmWriter.WriteLine(stmWriter.NewLine);

                stmWriter.Flush();
                objFileStream.Flush();
                objFileStream.Close();
                ValidateFileSize(clientName);
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
            }
        }

        /// <summary>
        /// validates the file size, if file is greater than the maximum limit then old file is backed up with name as current date and a new log file is created
        /// </summary>
        /// <param name="clientName">client name for a client specific log file</param>
        private static void ValidateFileSize(String clientName)
        {
            //Take backup of log file if size is greater than max limit, and recreate the log file
            string auditLogDir = getLogDir();
            string logFileName = getLogFile(clientName);

            FileStream objFileStream;
            FileInfo info = new FileInfo(logFileName);
            long fileSize = info.Length / 1024;
            if (fileSize > MAX_FILE_SIZE)
            {
                DateTime today = DateTime.Now;
                string newFileName = string.Format("{0}\\Mpp_API_{1}-{2}-{3}-{4}-{5}.log", auditLogDir, today.Month, today.Day, today.Year, today.Hour, today.Minute);
                File.Copy(logFileName, newFileName);
                File.Delete(logFileName);
                objFileStream = new FileStream(logFileName, FileMode.Create, FileAccess.Write);

                StreamWriter stmWriter = new StreamWriter(objFileStream);
                stmWriter.WriteLine(stmWriter.NewLine);
                stmWriter.Flush();
                objFileStream.Flush();
                objFileStream.Close();
            }
        }

        /// <summary>
        /// get the name of log file
        /// </summary>
        /// <param name="clientName">if not blank, msg will be written in log file named as PrimePay_clientName</param>
        /// <returns></returns>
        private static String getLogFile(String clientName)
        {
            String logFile = String.Empty;
            if (clientName == String.Empty)
                logFile = "Mpp_API.log";
            else
                logFile = "Mpp_API_" + clientName + ".log";
            String logFileDir = getLogDir();
            String logFileName = logFileDir + logFile;
            return logFileName;
        }

        /// <summary>
        /// Get the path of folder to save log file
        /// </summary>
        /// <returns>returns the path of log directory</returns>
        private static String getLogDir()
        {
            String logFileDir = MppUtility.ReadConfig("LogFolderPath");
            if (logFileDir.EndsWith("\\") == false)
            {
                logFileDir += "\\";
            }
            return logFileDir;
        }
        #endregion
    }
}

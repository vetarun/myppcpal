using Mpp.UTILITIES;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.BUSINESS
{
    public class SchedulerLog
    {
        #region Public Methods
        
        /// <summary>
        /// write a message in log file
        /// </summary>
        /// <param name="msg">message to be written into log file</param>
        public static void WriteLog(String msg)
        {
            DeleteOldLogFile();
            WriteLog(false, msg, false);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// actually write the message in log file
        /// </summary>
        /// <param name="bBeginNewLine">start a new line or not</param>
        /// <param name="msg">msg to be written into log file</param>
        /// <param name="bEndNewLine">the message ends with a new line or not</param>
        private static void WriteLog(bool bBeginNewLine, string msg, bool bEndNewLine)
        {
            string auditErrorString;
            FileStream objFileStream;
            StreamWriter stmWriter;
            string ErrMsg;

            try
            {
                string auditLogFilePath = getLogDir();

                string auditFileName = getLogFile(DateTime.Now, 0);

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
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
            }
        }

        /// <summary>
        /// get the name of log file
        /// </summary>
        /// <returns></returns>
        private static String getLogFile(DateTime today, int type)
        {
            String logFile;
            if (type == 1)
                logFile = string.Format("MppJob_{0}-{1}-{2}", today.Month, today.Day, today.Year);
            else
                logFile = string.Format("MppJob_{0}-{1}-{2}.log", today.Month, today.Day, today.Year);
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
            String logFileDir = MppUtility.ReadConfig("JobLogFolderPath");
            if (logFileDir.EndsWith("\\") == false)
            {
                logFileDir += "\\";
            }
            return logFileDir;
        }

        //delete old files
        private static void DeleteOldLogFile()
        {
            DateTime date = DateTime.Now.AddDays(-3);
            String filePath = getLogFile(date, 1);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
        #endregion
    }
}

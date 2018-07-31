using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.UTILITIES
{
    public class MppUtility
    {
        //Read Config data 
        public static String ReadConfig(String key)
        {
            String result = "";
            result = System.Configuration.ConfigurationManager.AppSettings[key];
            return result;
        }

        //Password Encryption
        public static Byte[] EncryptPassword(String strInput)
        {
            Byte[] hashBytes = null;
            UTF8Encoding encoding = new UTF8Encoding();
            MD5CryptoServiceProvider encrypter = new MD5CryptoServiceProvider();
            hashBytes = encrypter.ComputeHash(encoding.GetBytes(strInput));
            return hashBytes;
        }

        // Convert date to DDD,MMM,dd,YYYY string format
        public static String FullDateString(DateTime Date)
        {
            return Date.DayOfWeek.ToString().Substring(0, 3) + ", " + Date.ToString("MMM") + " " + Date.ToString("dd") + ", " + Date.ToString("yyyy");
        }

        // Convert date to MMM day,YYYY string format
        public static String FullRequestDate(DateTime Date)
        {
            return Date.ToString("MMMM") + " " + Date.Day + ", " + Date.ToString("yyyy");
        }

        //Convert time to hh,mm tt string format
        public static String TimeToString(DateTime dateTime)
        {
            String result = "";
            //AM/PM
            result = dateTime.ToString("hh:mm tt");
            return result;

        }

        //Get download folder path 
        public static String GetFilelocation(Int32 userID, String filedir, String type)
        {
            String FileDirectory = filedir;
            Int64 Seller_uid = userID;
            if (FileDirectory.EndsWith("\\") == false)
            {
                FileDirectory += "\\";
            }
            if(type == "bulk")
                FileDirectory = FileDirectory + Seller_uid + "\\";
            else
                FileDirectory = FileDirectory + Seller_uid + "\\" + "Search Term Reports"+ "\\";

            if (!Directory.Exists(FileDirectory))
                Directory.CreateDirectory(FileDirectory);
            return FileDirectory;
        }

        //Convert report date to Amazon supported format
        public static String DateFormat(DateTime dt, Int16 Format)
        {
            String nwdate = "";
            if (Format == 1) 
            {
                nwdate = dt.Month.ToString("00") + "/" + dt.Day.ToString("00") + "/" + dt.Year; // Amazon format - Bulk Reports(1,7,30,60)
            }
            else if(Format == 2)
            {
                nwdate = dt.Month.ToString("00") + "-" + dt.Day.ToString("00") + "-" + dt.Year; // Local Directory format(Bulk)
            }
            else if(Format == 3)
            {
                nwdate = dt.Month + "/" + dt.Day + "/" + dt.Year.ToString("00");  // Amazon format - Search term(negative keywords)
            }
            else if (Format == 4)
            {
                nwdate = dt.Year + "-" + dt.Month.ToString("00") + dt.Day.ToString("00"); // Local Directory format(Search term)
            }
            else if (Format == 5)
            {
                nwdate = dt.Month.ToString("00") + "_" + dt.Day.ToString("00") + "_" + dt.Year; // New Local Directory format(Bulk)
            }
            return nwdate;
        }

        //Check file exists or not before go and get reports
        public static String CheckFileExist(Int32 UserID, DateTime startdate, DateTime enddate, out String fileName)
        {
            String res = String.Empty;
            String downloaddir = ConfigurationManager.AppSettings["DownloadFolderPath"];
            string downloadfilepath = MppUtility.GetFilelocation(UserID, downloaddir, "bulk");
            fileName = ConfigurationManager.AppSettings["filename"];
            string fileName1 = fileName.PadRight(29) + MppUtility.DateFormat(startdate, 2).PadRight(11) + "-" + MppUtility.DateFormat(enddate, 2).PadLeft(11) + ".csv";
            string fileName2 = fileName.PadRight(29) + MppUtility.DateFormat(startdate, 5).PadRight(11) + "-" + MppUtility.DateFormat(enddate, 5).PadLeft(11) + ".csv";
            string filePath1 = Path.Combine(downloadfilepath, fileName1);
            string filePath2 = Path.Combine(downloadfilepath, fileName2);//New format
            if (File.Exists(filePath1))
            {
                fileName = fileName1;
                res = filePath1;
            }
            else if (File.Exists(filePath2))
            {
                fileName = fileName2;
                res = filePath2;
            }
            return res;
        }

        //Check file exists or not before go and get reports
        public static bool CheckNegFileExist(Int32 UserID, DateTime ReportDate, Int64 BatchNo)
        {
            bool res = false;
            String downloaddir = ConfigurationManager.AppSettings["DownloadFolderPath"];
            string downloadfilepath = MppUtility.GetFilelocation(UserID, downloaddir, "search");
            string fileName = ConfigurationManager.AppSettings["negfilename"];
            string fileName1 = fileName + "-" + MppUtility.DateFormat(ReportDate, 4) + "-" + BatchNo + ".txt";            
            string filePath = Path.Combine(downloadfilepath, fileName1);
            if (File.Exists(filePath))
            {
                res = true;
            }
            return res;
        }

        //Delete seller files
        //public static void DeleteSellerDirectory(Int32 UserId)
        //{
        //    String downloaddir = ConfigurationManager.AppSettings["DownloadFolderPath"];
        //    String uploaddir = ConfigurationManager.AppSettings["UploadFolderPath"];
        //    if (downloaddir.EndsWith("\\") == false)
        //        downloaddir += "\\";
        //    if (uploaddir.EndsWith("\\") == false)
        //        uploaddir += "\\";
        //    downloaddir = downloaddir + UserId;
        //    uploaddir = uploaddir + UserId;

        //    if (Directory.Exists(downloaddir))
        //        Directory.Delete(downloaddir, true);
        //    if (Directory.Exists(uploaddir))
        //        Directory.Delete(uploaddir, true);
        //}

        //Get Next Tuesday
        public static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static string FormatCSV(string input)
        {
            try
            {
                if (input == null || input == "\n" || input == "\r" || input == "\r\n")
                    return string.Empty;

                bool containsQuote = false;
                bool containsComma = false;

                int len = input.Length;
                for (int i = 0; i < len && (containsComma == false || containsQuote == false); i++)
                {
                    char ch = input[i];
                    if (ch == '"')
                        containsQuote = true;
                    else if (ch == ',')
                        containsComma = true;
                }

                if (containsQuote && containsComma)
                    input = input.Replace("\"", "\"\"");

                if (containsComma)
                    return "\"" + input + "\"";
                else
                    return input;
            }
            catch
            {
                throw;
            }
        }

        public static String DateFormat(DateTime dt)
        {
            return dt.Year + dt.Month.ToString("00") + dt.Day.ToString("00");
        }

        public static void StringToDateFormat(String str, out DateTime dt)
        {
            if (!DateTime.TryParseExact(str,
                "yyyyMMdd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dt))
                {
                   dt = DateTime.MinValue;
                };
        }

        public static TimeSpan SetTime(int hours, int minutes, int seconds)
        {
            return new TimeSpan(hours, minutes, seconds);
        }
    }
}

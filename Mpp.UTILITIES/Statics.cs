using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.UTILITIES
{
    public class Statics
    {
        public enum StripePlans
        {
            Trial = 1,
            [Description("SOLOPRENEUR")]
            SOLOPRENEUR = 2,
            [Description("STARTUP")]
            STARTUP = 3,
            [Description("BUSINESS TIME")]
            BUSINESS_TIME = 4,
            [Description("BIG BUSINESS")]
            BIG_BUSINESS = 5,
            [Description("ENTERPRISE")]
            ENTERPRISE = 6,
            [Description("NO PLAN")]
            NO_PLAN = 7
        }

        public enum MonthName
        {
            Jan = 1, Feb = 2, Mar = 3, Apr = 4, May = 5, Jun = 6, Jul = 7, Aug = 8, Sep = 9, Oct = 10, Nov = 11, Dec = 12
        }

        public enum AccounType
        {
            MppUser = 0,
            Admin = 1 
        }

        public enum RecordType
        {
            Campaign = 1, AdGroup = 2, Keyword = 3,SearchTerm = 4
        }

        public enum ReportStatus
        {
            NOTREQUIRED = -1, NOTSET_SIGNUP = 0, SET_SIGNUP = 1, NOTSET = 2,SET = 3,REFRESH_NOTSET = 4, REFRESH_SET = 5, FAILED_NOTSET = 6
        }

        public enum ResponseStatus
        {
            ERROR_SQL = -1, ERROR_API = -2, NOTFOUND = 0, SUCCESS = 1, NORECORDS = 2
        }

        public enum UserReportType
        {
            KeywordBulk=1,KeyOptimized=2,SearchTermBulk=3,SearchTermOptimized=4
        }
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes =
            (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}

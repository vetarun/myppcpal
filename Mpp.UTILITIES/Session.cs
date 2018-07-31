using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Mpp.UTILITIES
{
    public static class SessionData
    {
        public static Int32 UserID
        {
            get
            {
                return Convert.ToInt32(HttpContext.Current.Session["UserID"]);
            }
            set
            {
                HttpContext.Current.Session["UserID"] = value;
            }
        }
        public static String FirstName
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["FirstName"]);
            }
            set
            {
                HttpContext.Current.Session["FirstName"] = value;
            }
        }
        public static String LastName
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["LastName"]);
            }
            set
            {
                HttpContext.Current.Session["LastName"] = value;
            }
        }
        public static String SellerName
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["SellerName"]);
            }
            set
            {
                HttpContext.Current.Session["SellerName"] = value;
            }
        }
        public static String StartDate
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["StartDate"]);
            }
            set
            {
                HttpContext.Current.Session["StartDate"] = value;
            }
        }
        
        public static Int32 PlanStatus
        {
            get
            {
                return Convert.ToInt32(HttpContext.Current.Session["PlanStatus"]);
            }
            set
            {
                HttpContext.Current.Session["PlanStatus"] = value;
            }
        }

        public static Int32 PlanID
        {
            get
            {
                return Convert.ToInt32(HttpContext.Current.Session["PlanID"]);
            }
            set
            {
                HttpContext.Current.Session["PlanID"] = value;
            }
        }

        public static Int16 ProfileAccess
        {
            get
            {
                return Convert.ToInt16(HttpContext.Current.Session["ProfileAccess"]);
            }
            set
            {
                HttpContext.Current.Session["ProfileAccess"] = value;
            }
        }

        public static Int16 FormulaAccess
        {
            get
            {
                return Convert.ToInt16(HttpContext.Current.Session["FormulaAccess"]);
            }
            set
            {
                HttpContext.Current.Session["FormulaAccess"] = value;
            }

        }

        public static String Email
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["Email"]);
            }
            set
            {
                HttpContext.Current.Session["Email"] = value;
            }
        }
        public static string StripeCustId
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["StripeCustId"]);
            }
            set
            {
                HttpContext.Current.Session["StripeCustId"] = value;
            }
        }
        public static string StripeCardId
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["StripeCardId"]);
            }
            set
            {
                HttpContext.Current.Session["StripeCardId"] = value;
            }
        }

        public static DateTime TrialEndOn
        {
            get
            {
                return Convert.ToDateTime(HttpContext.Current.Session["TrialEndOn"]);
            }
            set
            {
                HttpContext.Current.Session["TrialEndOn"] = value;
            }
        }

        public static Int16 IsAgreementAccept
        {
            get
            {
                return Convert.ToInt16(HttpContext.Current.Session["IsAgreementAccept"]);
            }
            set
            {
                HttpContext.Current.Session["IsAgreementAccept"] = value;
            }
        }

        public static Int32 AlertCount
        {
            get
            {
                return Convert.ToInt32(HttpContext.Current.Session["AlertCount"]);
            }
            set
            {
                HttpContext.Current.Session["AlertCount"] = value;
            }
        }

    }
}

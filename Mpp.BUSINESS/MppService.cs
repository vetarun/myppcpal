using Mpp.BUSINESS.DataLibrary;
using Mpp.UTILITIES;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Mpp.BUSINESS
{
    public class MppService
    {
        private readonly EmailAlert alert = new EmailAlert();
        //Process once a day services
        public void Process_DailyService()
        {
            DataTable tbl = null;
            tbl = ServicesData.GetAllSellerIds();
            foreach (DataRow row in tbl.Rows)
            {
                String custmid = Convert.ToString(row["stp_custId"]);
                StripeServices.UpdateBillingPeriod(custmid);//when webhooks don't work 
            }
            APIData.SetOldReportDatesData();
        }
        public void Process_DeleteDataService()
        {
           // process once a day to check is there any user to delete 
            AdminData.DeleteArchiveData();
        }

        //All Services
        public void Process_AllServices()
        {
            DataTable dtbl = null, dtbl1 = null, dtbl2 = null;
            DataSet ds = null;
            var sdata = new SellerData();
            dtbl = sdata.GetUserStatus();
            if (dtbl.Rows.Count > 0)
            {
                foreach (DataRow rw in dtbl.Rows)
                {
                    bool subscription = false;
                    int UserId = Convert.ToInt32(rw["MppUserID"]);
                    String CustmID = Convert.ToString(rw["stp_custId"]);
                    if (!String.IsNullOrWhiteSpace(CustmID))
                        subscription = StripeHelper.UnSubscribe(CustmID);    //unsubscribe the users either whose status is 2 or active with no card
                    if (subscription)
                        ServicesData.UpdatePlanData(UserId, 0);
                }
            }

            ds = sdata.GetUserEmails();
            dtbl1 = ds.Tables[0];
            dtbl2 = ds.Tables[1];

            if (dtbl1.Rows.Count > 0)
            {
                DataTable tbl = new DataTable();
                tbl.Columns.AddRange(new DataColumn[2] {
                        new DataColumn("MppUserId", typeof(Int32)),
                        new DataColumn("Limit", typeof(Int32))
                    });

                //Using Parallel Multi-Threading to send multiple bulk email.
                Parallel.ForEach(dtbl1.AsEnumerable(), row =>
                {
                    String msg = "";

                    Int32 Limit = Convert.ToInt32(row["accesslimit"]);
                    Int32 UserID = Convert.ToInt32(row["MppUserID"]);
                    msg = alert.SendNewUserAccessAlert(row["Email"].ToString(), row["FirstName"].ToString(), row["LastName"].ToString());
                    if (msg == "")
                    {
                        tbl.Rows.Add();
                        tbl.Rows[tbl.Rows.Count - 1][0] = UserID;
                        switch (Limit)
                        {
                            case 1:
                                Limit = 2;
                                break;
                            case 2:
                                Limit = 3;
                                break;
                            default:
                                Limit = 0;
                                break;
                        }
                        tbl.Rows[tbl.Rows.Count - 1][1] = Limit;
                    }
                });
                if (tbl.Rows.Count > 0)
                    sdata.UpdateUserAccessLimit(tbl);
            }

            if (dtbl2.Rows.Count > 0)
            {
                DataTable tbl1 = new DataTable();
                DataTable tbl2 = new DataTable();
                tbl1.Columns.AddRange(new DataColumn[2] {
                        new DataColumn("UserID", typeof(Int32)),
                        new DataColumn("Status", typeof(Int32))
                    });
                tbl2.Columns.AddRange(new DataColumn[2] {
                        new DataColumn("UserID", typeof(Int32)),
                        new DataColumn("Status", typeof(Int32))
                    });

                //Using Parallel Multi-Threading to send multiple bulk email.
                Parallel.ForEach(dtbl2.AsEnumerable(), row =>
                {
                    String msg = "";
                    Int32 UserID = Convert.ToInt32(row["MppUserID"]);
                    String FirstName = Convert.ToString(row["FirstName"]);
                    String LastName = Convert.ToString(row["LastName"]);
                    String Email = Convert.ToString(row["Email"]);
                    DateTime TrialEndDate = Convert.ToDateTime(row["TrailEndDate"]);
                    msg = alert.SendTrialExpiresAlert(Email, TrialEndDate, FirstName, LastName);
                    if (msg == "")
                    {
                        if (TrialEndDate < DateTime.Now)
                        {
                            tbl2.Rows.Add();
                            tbl2.Rows[tbl2.Rows.Count - 1][0] = UserID;
                            tbl2.Rows[tbl2.Rows.Count - 1][1] = 1;
                        }
                        else
                        {
                            tbl1.Rows.Add();
                            tbl1.Rows[tbl1.Rows.Count - 1][0] = UserID;
                            tbl1.Rows[tbl1.Rows.Count - 1][1] = 1;
                        }
                    }
                });
                if (tbl1.Rows.Count > 0)
                    sdata.UpdateSevenTrialEmailStatus(tbl1);
                if (tbl2.Rows.Count > 0)
                    sdata.UpdateTrialEndEmailStatus(tbl2);
            }

            StripeServices.RenewPlan();
        }

    }
}

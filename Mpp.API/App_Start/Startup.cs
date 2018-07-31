using Hangfire;
using Microsoft.Owin;
using Mpp.BUSINESS;
using Mpp.UTILITIES;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[assembly: OwinStartup(typeof(Mpp.API.App_Start.Startup))]
namespace Mpp.API.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            string ConnStr = MppUtility.ReadConfig("PPCConnStr");
            GlobalConfiguration.Configuration.UseSqlServerStorage(ConnStr);

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            //JOB: Set Reports (Snapshot, Inventory), run every 3 minutes
            //RecurringJob.AddOrUpdate("Missing Out Punch Alerts", () => ReportsAPI.ProcessSetProductsSnapShot(), "20/30 * * * *", TimeZoneInfo.Local);
        }
    }
}
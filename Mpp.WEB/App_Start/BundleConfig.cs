using System.Web;
using System.Web.Optimization;

namespace Mpp.WEB
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/Mpp.js",
                        "~/Scripts/Custom.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryajax").Include(
                       "~/Scripts/jquery.unobtrusive-ajax.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/dashboard")
                        .Include("~/Scripts/Angular/Dashboard/App.js")
                        .IncludeDirectory("~/Scripts/Angular/Dashboard/", "*.js", searchSubdirectories: true));

            bundles.Add(new ScriptBundle("~/bundles/mycampaigns")
                        .Include("~/Scripts/Angular/MyCampaigns/App.js")
                        .IncludeDirectory("~/Scripts/Angular/MyCampaigns/", "*.js", searchSubdirectories: true));

            bundles.Add(new ScriptBundle("~/bundles/reports")
                        .Include("~/Scripts/Angular/Reports/app.js")
                        .IncludeDirectory("~/Scripts/Angular/Reports/", "*.js", searchSubdirectories: true));

            bundles.Add(new ScriptBundle("~/bundles/admin/dashboard")
                        .Include("~/Scripts/Angular/Admin/app.js",
                                 "~/Scripts/Angular/Admin/DashboardController.js",
                                 "~/Scripts/Angular/Admin/directive.js",
                                 "~/Scripts/Angular/Admin/Service.js",
                                 "~/Scripts/Angular/Admin/factory.js",
                                 "~/Scripts/Angular/Admin/filter.js"));

            bundles.Add(new ScriptBundle("~/bundles/admin/account")
                        .Include("~/Scripts/Angular/Admin/app.js",
                                 "~/Scripts/Angular/Admin/AccountController.js",
                                 "~/Scripts/Angular/Admin/Service.js",
                                 "~/Scripts/Angular/Admin/filter.js"));

            bundles.Add(new ScriptBundle("~/bundles/admin/affiliation")
                        .Include("~/Scripts/Angular/Admin/app.js",
                                 "~/Scripts/Angular/Admin/AffiliationController.js",
                                 "~/Scripts/Angular/Admin/directive.js",
                                 "~/Scripts/Angular/Admin/Service.js",
                                 "~/Scripts/Angular/Admin/filter.js"));

            bundles.Add(new ScriptBundle("~/bundles/admin/preferences")
                        .Include("~/Scripts/Angular/Admin/app.js",
                                 "~/Scripts/Angular/Admin/SystemlogController.js",
                                 "~/Scripts/Angular/Admin/directive.js",
                                 "~/Scripts/Angular/Admin/Service.js",
                                 "~/Scripts/Angular/Admin/filter.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/font-awesome.css",
                      "~/Content/bootstrap.css",
                      "~/Content/app.css",
                      "~/Content/loader.css",
                      "~/Content/AutoLogoutSession.css"));
        }
    }
}

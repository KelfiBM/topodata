using System.Web;
using System.Web.Optimization;

namespace Topodata2
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate.unobtrusive*",
                "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/site.css"));
            bundles.Add(new ScriptBundle("~/bundles/SharedLayout/Scripts").Include(
                "~/resources/plugins/jquery/jquery.min.js",
                "~/resources/plugins/jquery/jquery-migrate.min.js",
                "~/resources/plugins/bootstrap/js/bootstrap.min.js",
                "~/resources/plugins/back-to-top.js",
                "~/resources/plugins/smoothscroll.js",
                "~/resources/js/plugins/validation.js",
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js",
                "~/resources/js/custom.js",
                "~/resources/js/app.js"));
        }
    }
}

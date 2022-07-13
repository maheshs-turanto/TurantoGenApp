using System.Web;
using System.Web.Optimization;
namespace GeneratorBase.MVC
{
/// <summary>A bundle configuration.</summary>
public class BundleConfig
{
    /// <summary>For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862.</summary>
    ///
    /// <param name="bundles">The bundles.</param>
    public static void RegisterBundles(BundleCollection bundles)
    {
        bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.unobtrusive*"));
        bundles.Add(new ScriptBundle("~/bundles/layouttop").Include(
                        "~/Content/vendor/jquery/dist/jquery.js",
                        "~/Content/vendor/popper.js/dist/umd/popper.js",
                        "~/Scripts/Entity.js",
                        "~/Scripts/Common1/jstz.main.js",
                        "~/Scripts/jquery-ui.js"
                    ));
        bundles.Add(new ScriptBundle("~/bundles/layoutbottom").Include(
                        "~/Content/dist/js/bootstrap-colorpicker.js",
                        "~/Content/src/js/docs.js"
                    ));
        // Use the development version of Modernizr to develop with and learn from. Then, when you're
        // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
        bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*",
                        "~/Scripts/respond.js"));
        bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Content/vendor/bootstrap/dist/js/bootstrap.js", new CssRewriteUrlTransform()));
        bundles.Add(new ScriptBundle("~/bundles/common1").Include(
                        "~/Scripts/Common1/responsive1.js",
                        "~/Scripts/Common1/analytics.js",
                        "~/Scripts/Common1/html5shiv.js",
                        "~/Scripts/Common1/custom-forms.js",
                        "~/Content/vendor/chart.js/dist/Chart.js",
                        "~/Scripts/Common1/jquery.cookie.js"));
        bundles.Add(new ScriptBundle("~/bundles/common2").Include(
                        "~/Scripts/Common2/responsive2.js",
                        "~/Scripts/Common2/jquery.are-you-sure.js",
                        "~/Scripts/Common2/moment-2.4.0.js",
                        "~/Scripts/Common2/bootstrap-datetimepicker.js",
                        "~/Scripts/Common2/dashboard.js",
                        "~/Scripts/Common2/bootstrap-multiselect.js",
                        "~/Scripts/Common2/businessrule.js"));
        bundles.Add(new ScriptBundle("~/bundles/common3").Include(
                        "~/Scripts/Common3/jquery.maskedinput.js",
                        "~/Scripts/Common3/currencyformat.js",
                        "~/Scripts/Common3/jquery.ba-throttle-debounce.min.js",
                        "~/Content/vendor/chosen-js/chosen.jquery.js"));
        bundles.Add(new ScriptBundle("~/bundles/Theme").Include(
                        "~/Content/vendor/modernizr/modernizr.custom.js",
                        "~/Content/vendor/js-storage/js.storage.js",
                        "~/Content/vendor/screenfull/dist/screenfull.js",
                        "~/Content/vendor/i18next/i18next.js",
                        "~/Content/vendor/i18next-xhr-backend/i18nextXHRBackend.js",
                        "~/Content/vendor/bootstrap/dist/js/bootstrap.js",
                        "~/Content/vendor/jquery-slimscroll/jquery.slimscroll.js",
                        "~/Content/vendor/jquery-sparkline/jquery.sparkline.js",
                        "~/Content/vendor/flot/jquery.flot.js",
                        "~/Content/vendor/jquery.flot.tooltip/js/jquery.flot.tooltip.js",
                        "~/Content/vendor/flot/jquery.flot.resize.js",
                        "~/Content/vendor/flot/jquery.flot.pie.js",
                        "~/Content/vendor/flot/jquery.flot.time.js",
                        "~/Content/vendor/flot/jquery.flot.categories.js",
                        "~/Content/vendor/jquery.flot.spline/jquery.flot.spline.js",
                        "~/Content/vendor/easy-pie-chart/dist/jquery.easypiechart.js",
                        "~/Content/vendor/moment/min/moment-with-locales.js",
                        "~/Content/vendor/pace/pace.min.js",
                        "~/Scripts/themeBase.js"
                    ));
        bundles.Add(new ScriptBundle("~/bundles/select2js").Include("~/Scripts/select2.full.js"));
        bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap-datetimepicker.min.css", new CssRewriteUrlTransform())
                    .Include("~/Content/bootstrap-multiselect.css", new CssRewriteUrlTransform())
                    .Include("~/Content/vendor/chosen-js/chosen.css", new CssRewriteUrlTransform())
                    .Include("~/Content/print.css", new CssRewriteUrlTransform())
                   );
        bundles.Add(new StyleBundle("~/Content/layoutcss")
                    .Include("~/Content/font-awesome/css/font-awesome.css", new CssRewriteUrlTransform())
                    .Include("~/Content/icon.css", new CssRewriteUrlTransform())
                    .Include("~/Content/main.css", new CssRewriteUrlTransform())
                   );
        bundles.Add(new StyleBundle("~/Content/bootstrapcss").Include(
                        "~/Content/vendor/fortawesome/fontawesome-free/css/brands.css", new CssRewriteUrlTransform())
                    .Include(
                        "~/Content/vendor/fortawesome/fontawesome-free/css/regular.css", new CssRewriteUrlTransform())
                    .Include(
                        "~/Content/vendor/fortawesome/fontawesome-free/css/solid.css", new CssRewriteUrlTransform())
                    .Include(
                        "~/Content/vendor/fortawesome/fontawesome-free/css/fontawesome.css", new CssRewriteUrlTransform())
                    .Include(
                        "~/Content/vendor/fortawesome/fontawesome-free/css/simple-line-icons.css", new CssRewriteUrlTransform())
                   );
        bundles.Add(new ScriptBundle("~/bundles/sbTheme").Include(
                        "~/Scripts/sb-admin-2.min.js"
                    ));
        bundles.Add(new StyleBundle("~/Content/Newcss").Include(
                        "~/Content/bootstrap.min.css",
                        "~/Content/ThemeBase.css",
                        "~/Content/loading.css",
                        "~/Content/animate.css",
                        "~/Content/style.css",
                        "~/Content/plugins/footable/footable.core.css",
                        "~/Content/vendor/animate.css/animate.css",
                        "~/Content/vendor/whirl/dist/whirl.css"));
        //font-awesome
        bundles.Add(new StyleBundle("~/Content/fontawesome").Include(
                        "~/Content/font-awesome/css/*.css"));
        bundles.Add(new StyleBundle("~/Content/csstheme").Include(
                        "~/Content/themes/base/*.css"));
        bundles.Add(new StyleBundle("~/Content/select2css").Include("~/Content/select2.min.css"));
        bundles.Add(new ScriptBundle("~/bundles/WebApi").Include(
                        "~/ScriptWebApi/*.js"));
        bundles.Add(new ScriptBundle("~/bundles/fullcalendarjs").Include(
                        "~/Scripts/moment.min.js",
                        "~/Scripts/fullcalendar.js"
                    ));
#if DEBUG
        BundleTable.EnableOptimizations = false;
#else
        BundleTable.EnableOptimizations = false;
#endif
    }
}
}

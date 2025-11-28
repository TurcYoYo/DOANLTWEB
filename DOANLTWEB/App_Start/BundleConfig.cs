using System.Web;
using System.Web.Optimization;

public class BundleConfig
{
    public static void RegisterBundles(BundleCollection bundles)
    {
        // Tắt optimization trong development
#if DEBUG
        BundleTable.EnableOptimizations = false;
#else
        BundleTable.EnableOptimizations = true;
#endif

        // Tạo bundle không minify
        var jqueryBundle = new Bundle("~/bundles/jquery");
        jqueryBundle.Include("~/Scripts/jquery-{version}.js");
        bundles.Add(jqueryBundle);

        var bootstrapBundle = new Bundle("~/bundles/bootstrap");
        bootstrapBundle.Include("~/Scripts/bootstrap.js");
        bundles.Add(bootstrapBundle);

        var cssBundle = new Bundle("~/Content/css");
        cssBundle.Include("~/Content/bootstrap.css", "~/Content/site.css");
        bundles.Add(cssBundle);
    }
}
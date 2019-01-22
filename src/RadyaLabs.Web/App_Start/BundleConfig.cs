using System.Collections.Generic;
using System.Web.Optimization;

namespace RadyaLabs.Web
{
    public class BundleConfig : IBundleConfig
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            RegisterScripts(bundles);
            RegisterStyles(bundles);
            RegisterOrder(bundles);
        }
        private void RegisterScripts(BundleCollection bundles)
        {
            Bundle privateLib = new ScriptBundle("~/scripts/private/lib.js")
                .Include("~/scripts/jquery/jquery.js")
                .IncludeDirectory("~/scripts/jquery", "*.js", true)
                .Include("~/scripts/jqueryui/jquery-ui.js")
                .IncludeDirectory("~/scripts/jqueryui", "*.js", true)
                .IncludeDirectory("~/scripts/mvcdatalist", "*.js", true)
                .IncludeDirectory("~/scripts/mvcgrid", "*.js", true)
                .Include("~/scripts/mvctree/*.js")
                .Include("~/scripts/bootstrap/*.js")
                .Include("~/scripts/shared/widgets/*.js");

            Bundle publicLib = new ScriptBundle("~/scripts/public/lib.js")
                .Include("~/scripts/jquery/jquery.js")
                .IncludeDirectory("~/scripts/jquery", "*.js", true)
                .Include("~/scripts/bootstrap/*.js")
                .Include("~/scripts/shared/widgets/*.js");

            Bundle app = new ScriptBundle("~/scripts/app/shared.js")
                .Include("~/scripts/shared/*.js");

            bundles.Add(privateLib);
            bundles.Add(publicLib);
            bundles.Add(app);
        }
        private void RegisterStyles(BundleCollection bundles)
        {
            Bundle privateLib = new StyleBundle("~/content/private/lib.css")
                .Include("~/content/jqueryui/*.css")
                .Include("~/content/bootstrap/*.css")
                .Include("~/content/fontawesome/*.css")
                .Include("~/content/mvcgrid/*.css")
                .Include("~/content/mvctree/*.css")
                .Include("~/content/mvcdatalist/*.css")
                .Include("~/content/shared/*.css");

            Bundle publicLib = new StyleBundle("~/content/public/lib.css")
                .Include("~/content/bootstrap/*.css")
                .Include("~/content/fontawesome/*.css")
                .Include("~/content/shared/*.css");

            bundles.Add(privateLib);
            bundles.Add(publicLib);
        }
        private void RegisterOrder(BundleCollection bundles)
        {
            foreach (Bundle bundle in bundles)
                bundle.Orderer = new NaturalOrder();
        }

        private class NaturalOrder : IBundleOrderer
        {
            public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
            {
                return files;
            }
        }
    }
}

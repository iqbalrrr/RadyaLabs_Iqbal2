using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace RadyaLabs.Components.Extensions
{
    public static class ContentExtensions
    {
        private static Dictionary<String, IHtmlString> Scripts { get; }
        private static Dictionary<String, IHtmlString> Styles { get; }

        static ContentExtensions()
        {
            Scripts = new Dictionary<String, IHtmlString>();
            Styles = new Dictionary<String, IHtmlString>();
        }

        public static IHtmlString RenderControllerScript(this HtmlHelper html)
        {
            return RenderScript(html, "shared");
        }
        public static IHtmlString RenderActionScript(this HtmlHelper html)
        {
            return RenderScript(html, html.ViewContext.RouteData.Values["action"]);
        }

        public static IHtmlString RenderControllerStyle(this HtmlHelper html)
        {
            return RenderStyle(html, "shared");
        }
        public static IHtmlString RenderActionStyle(this HtmlHelper html)
        {
            return RenderStyle(html, html.ViewContext.RouteData.Values["action"]);
        }

        private static IHtmlString RenderScript(HtmlHelper html, Object action)
        {
            String path = FormPath(html.ViewContext.RouteData.Values, action) + ".js";

            if (Scripts.ContainsKey(path))
                return Scripts[path];

            if (!ContentExists(html, "~/scripts/application/" + path))
                return Scripts[path] = MvcHtmlString.Empty;

            BundleTable.Bundles.Add(new ScriptBundle("~/scripts/app/" + path).Include("~/scripts/application/" + path));

            return Scripts[path] = System.Web.Optimization.Scripts.Render("~/scripts/app/" + path);
        }
        private static IHtmlString RenderStyle(HtmlHelper html, Object action)
        {
            String path = FormPath(html.ViewContext.RouteData.Values, action) + ".css";

            if (Styles.ContainsKey(path))
                return Styles[path];

            if (!ContentExists(html, "~/content/application/" + path))
                return Styles[path] = MvcHtmlString.Empty;

            BundleTable.Bundles.Add(new StyleBundle("~/content/app/" + path).Include("~/content/application/" + path));

            return Styles[path] = System.Web.Optimization.Styles.Render("~/content/app/" + path);
        }

        private static String FormPath(RouteValueDictionary route, Object action)
        {
            return ((route["Area"] == null ? null : route["Area"] + "/") + route["controller"] + "/" + action).ToLower();
        }
        private static Boolean ContentExists(HtmlHelper html, String path)
        {
            return File.Exists(html.ViewContext.HttpContext.Server.MapPath(path));
        }
    }
}

using System.Web.Mvc;

namespace RadyaLabs.Components.Mvc
{
    public class ViewEngine : RazorViewEngine
    {
        public ViewEngine()
        {
            AreaViewLocationFormats = new[]
            {
                "~/Views/{2}/{1}/{0}.cshtml",
                "~/Views/{2}/Shared/{0}.cshtml"
            };
            AreaMasterLocationFormats = new[]
            {
                "~/Views/{2}/{1}/{0}.cshtml",
                "~/Views/{2}/Shared/{0}.cshtml"
            };
            AreaPartialViewLocationFormats = new[]
            {
                "~/Views/{2}/{1}/{0}.cshtml",
                "~/Views/{2}/Shared/{0}.cshtml"
            };
        }
    }
}

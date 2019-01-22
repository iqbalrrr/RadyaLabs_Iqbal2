using System;
using System.Web.Mvc;

namespace RadyaLabs.Controllers.Administration
{
    public class AdministrationAreaRegistration : AreaRegistration
    {
        public override String AreaName => "Administration";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context
                .MapRoute(
                    "AdministrationMultilingual",
                    "{language}/Administration/{controller}/{action}/{id}",
                    new { area = "Administration", action = "Index", id = UrlParameter.Optional },
                    new { language = "lt", id = "[0-9]*" },
                    new[] { "RadyaLabs.Controllers.Administration" });

            context
                .MapRoute(
                    "Administration",
                    "Administration/{controller}/{action}/{id}",
                    new { language = "en", area = "Administration", action = "Index", id = UrlParameter.Optional },
                    new { language = "en", id = "[0-9]*" },
                    new[] { "RadyaLabs.Controllers.Administration" });
        }
    }
}

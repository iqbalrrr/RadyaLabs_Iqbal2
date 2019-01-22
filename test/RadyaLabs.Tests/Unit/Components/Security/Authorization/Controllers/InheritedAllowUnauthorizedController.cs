using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

namespace RadyaLabs.Tests.Unit.Components.Security
{
    [ExcludeFromCodeCoverage]
    public class InheritedAllowUnauthorizedController : AllowUnauthorizedController
    {
        [HttpGet]
        public ViewResult InheritanceAction()
        {
            return null;
        }
    }
}

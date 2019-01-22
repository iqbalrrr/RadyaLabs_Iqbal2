using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

namespace RadyaLabs.Tests.Unit.Components.Security
{
    [ExcludeFromCodeCoverage]
    public class InheritedAuthorizedController : AuthorizedController
    {
        [HttpGet]
        public ViewResult InheritanceAction()
        {
            return null;
        }
    }
}

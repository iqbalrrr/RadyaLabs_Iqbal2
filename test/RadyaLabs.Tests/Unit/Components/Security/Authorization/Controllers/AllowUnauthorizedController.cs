using RadyaLabs.Components.Security;
using System.Diagnostics.CodeAnalysis;

namespace RadyaLabs.Tests.Unit.Components.Security
{
    [AllowUnauthorized]
    [ExcludeFromCodeCoverage]
    public class AllowUnauthorizedController : AuthorizedController
    {
    }
}

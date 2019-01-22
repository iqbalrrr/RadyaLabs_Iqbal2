using RadyaLabs.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Xunit;

namespace RadyaLabs.Tests.Unit.Controllers
{
    public class SecurityTests
    {
        #region ValidateAntiForgeryToken

        [Fact]
        public void PostMethods_HasValidateAntiForgeryToken()
        {
            IEnumerable<MethodInfo> methods = typeof(BaseController)
                .Assembly
                .GetTypes()
                .Where(type => typeof(Controller).IsAssignableFrom(type))
                .SelectMany(type => type.GetMethods())
                .Where(method => method.IsDefined(typeof(HttpPostAttribute), false));

            foreach (MethodInfo method in methods)
                Assert.True(method.IsDefined(typeof(ValidateAntiForgeryTokenAttribute), false),
                    $"{method.ReflectedType.Name}.{method.Name} method does not have ValidateAntiForgeryToken attribute specified.");
        }

        #endregion
    }
}

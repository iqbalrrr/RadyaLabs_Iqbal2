using RadyaLabs.Web;
using System;
using System.Web.Optimization;
using Xunit;

namespace RadyaLabs.Tests.Unit.Web
{
    public class BundleConfigTests
    {
        #region RegisterBundles(BundleCollection bundles)

        [Theory]
        [InlineData("~/scripts/app/shared.js")]
        [InlineData("~/scripts/public/lib.js")]
        [InlineData("~/scripts/private/lib.js")]
        public void RegisterBundles_ForScripts(String path)
        {
            BundleCollection bundles = new BundleCollection();

            new BundleConfig().RegisterBundles(bundles);

            Assert.IsType<ScriptBundle>(bundles.GetBundleFor(path));
        }

        [Theory]
        [InlineData("~/content/public/lib.css")]
        [InlineData("~/content/private/lib.css")]
        public void RegisterBundles_ForStyles(String path)
        {
            BundleCollection bundles = new BundleCollection();

            new BundleConfig().RegisterBundles(bundles);

            Assert.IsType<StyleBundle>(bundles.GetBundleFor(path));
        }

        #endregion
    }
}

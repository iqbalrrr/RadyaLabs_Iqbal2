using System.Web.Optimization;

namespace RadyaLabs.Web
{
    public interface IBundleConfig
    {
        void RegisterBundles(BundleCollection bundles);
    }
}

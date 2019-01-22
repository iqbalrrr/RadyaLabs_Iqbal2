using System.Web.Routing;

namespace RadyaLabs.Controllers
{
    public interface IRouteConfig
    {
        void RegisterRoutes(RouteCollection routes);
    }
}

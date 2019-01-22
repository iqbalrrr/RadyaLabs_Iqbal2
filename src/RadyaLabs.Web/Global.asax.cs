using LightInject;
using LightInject.Mvc;
using RadyaLabs.Components.Logging;
using RadyaLabs.Components.Mvc;
using RadyaLabs.Components.Security;
using RadyaLabs.Controllers;
using RadyaLabs.Resources.Shared;
using RadyaLabs.Web.DependencyInjection;
using Newtonsoft.Json;
using NonFactors.Mvc.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace RadyaLabs.Web
{
    public class MvcApplication : HttpApplication
    {
        public void Application_Start()
        {
            RegisterSecureResponseConfiguration();
            RegisterCurrentDependencyResolver();
            RegisterGlobalizationLanguages();
            RegisterModelMetadataProvider();
            RegisterDataTypeValidator();
            RegisterSiteMapProvider();
            RegisterAuthorization();
            RegisterModelBinders();
            RegisterViewEngine();
            RegisterAdapters();
            RegisterBundles();
            RegisterAreas();
            RegisterRoute();
        }
        public void Application_Error()
        {
            ILogger logger = DependencyResolver.Current.GetService<ILogger>();
            Exception exception = Server.GetLastError();
            logger.Log(exception);

            if (Request.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                Response.Clear();
                Server.ClearError();
                Response.StatusCode = 500;
                Response.ContentType = "application/json; charset=utf-8";

                if (Context.IsCustomErrorEnabled)
                    Response.Write(JsonConvert.SerializeObject(new { status = "error", data = new { message = Strings.SystemError } }));
                else
                    Response.Write(JsonConvert.SerializeObject(new { status = "error", data = new { message = Strings.SystemError, trace = exception.Message + Environment.NewLine + exception.StackTrace } }));
            }
            else if (Context.IsCustomErrorEnabled)
            {
                Server.ClearError();
                UrlHelper url = new UrlHelper(Request.RequestContext);
                RouteValueDictionary route = new RouteValueDictionary();
                HttpException httpException = exception as HttpException;

                route["language"] = Request.RequestContext.RouteData.Values["language"];
                route["controller"] = "Home";
                route["action"] = "Error";
                route["area"] = "";

                if (httpException?.GetHttpCode() == 404)
                    route["action"] = "NotFound";

                Response.TrySkipIisCustomErrors = true;
                Response.Redirect(url.RouteUrl(route));
            }
        }
        public void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");
        }

        public virtual void RegisterSecureResponseConfiguration()
        {
            AntiForgeryConfig.CookieName = WebConfigurationManager.AppSettings["AntiForgeryCookieName"];
            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;
            MvcHandler.DisableMvcResponseHeader = true;
        }
        public virtual void RegisterCurrentDependencyResolver()
        {
            MainContainer container = new MainContainer();
            container.RegisterControllers(typeof(BaseController).Assembly);
            container.RegisterServices();
            container.EnableMvc();

            DependencyResolver.SetResolver(new LightInjectMvcDependencyResolver(container));
        }
        public virtual void RegisterGlobalizationLanguages()
        {
            GlobalizationManager.Languages = DependencyResolver.Current.GetService<ILanguages>();
            MvcGrid.Filters.BooleanFalseOptionText = () => Strings.No;
            MvcGrid.Filters.BooleanTrueOptionText = () => Strings.Yes;
        }
        public virtual void RegisterModelMetadataProvider()
        {
            ModelMetadataProviders.Current = new DisplayNameMetadataProvider();
        }
        public virtual void RegisterDataTypeValidator()
        {
            ModelValidatorProviders.Providers.Remove(ModelValidatorProviders.Providers.SingleOrDefault(provider => provider is ClientDataTypeModelValidatorProvider));
            ModelValidatorProviders.Providers.Add(new DataTypeValidatorProvider());
        }
        public virtual void RegisterSiteMapProvider()
        {
            MvcSiteMap.Provider = DependencyResolver.Current.GetService<IMvcSiteMapProvider>();
        }
        public virtual void RegisterAuthorization()
        {
            Authorization.Provider = DependencyResolver.Current.GetService<IAuthorizationProvider>();
            Authorization.Provider?.Refresh();
        }
        public virtual void RegisterModelBinders()
        {
            ModelBinders.Binders.Add(typeof(String), new TrimmingModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime), new DateTimeModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new DateTimeModelBinder());
            ModelBinders.Binders.Add(typeof(IList<HttpPostedFileBase>), new HttpPostedFilesModelBinder());
        }
        public virtual void RegisterViewEngine()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new ViewEngine());
        }
        public virtual void RegisterAdapters()
        {
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RangeAttribute), typeof(RangeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DigitsAttribute), typeof(DigitsAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(EqualToAttribute), typeof(EqualToAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(IntegerAttribute), typeof(IntegerAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RequiredAttribute), typeof(RequiredAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(MinValueAttribute), typeof(MinValueAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(MaxValueAttribute), typeof(MaxValueAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(FileSizeAttribute), typeof(FileSizeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(MinLengthAttribute), typeof(MinLengthAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(AcceptFilesAttribute), typeof(AcceptFilesAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(GreaterThanAttribute), typeof(GreaterThanAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(EmailAddressAttribute), typeof(EmailAddressAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(StringLengthAttribute), typeof(StringLengthAdapter));
        }
        public virtual void RegisterBundles()
        {
            DependencyResolver.Current.GetService<IBundleConfig>().RegisterBundles(BundleTable.Bundles);
        }
        public virtual void RegisterAreas()
        {
            AreaRegistration.RegisterAllAreas();
        }
        public virtual void RegisterRoute()
        {
            DependencyResolver.Current.GetService<IRouteConfig>().RegisterRoutes(RouteTable.Routes);
        }
    }
}

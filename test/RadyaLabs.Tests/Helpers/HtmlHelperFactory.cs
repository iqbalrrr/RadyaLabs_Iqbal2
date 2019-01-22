using NSubstitute;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RadyaLabs.Tests
{
    public static class HtmlHelperFactory
    {
        public static HtmlHelper CreateHtmlHelper()
        {
            return CreateHtmlHelper<Object>(null);
        }
        public static HtmlHelper<T> CreateHtmlHelper<T>(T model)
        {
            ViewContext context = CreateViewContext(CreateControllerContext());
            IViewDataContainer container = new ViewPage();
            container.ViewData = context.ViewData;

            HtmlHelper<T> html = new HtmlHelper<T>(context, container, RouteTable.Routes);
            html.ViewData.Model = model;

            return html;
        }

        private static ControllerContext CreateControllerContext()
        {
            HttpContextBase http = HttpContextFactory.CreateHttpContextBase();

            return new ControllerContext(http, http.Request.RequestContext.RouteData, Substitute.For<ControllerBase>());
        }
        private static ViewContext CreateViewContext(ControllerContext controller)
        {
            ViewContext context = new ViewContext(
                controller,
                Substitute.For<IView>(),
                new ViewDataDictionary(),
                new TempDataDictionary(),
                new StringWriter());

            context.ClientValidationEnabled = true;

            return context;
        }
    }
}

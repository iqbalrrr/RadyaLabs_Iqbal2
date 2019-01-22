using RadyaLabs.Components.Security;
using RadyaLabs.Resources;
using RadyaLabs.Resources.Shared;
using NonFactors.Mvc.Grid;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RadyaLabs.Components.Extensions
{
    public static class MvcGridExtensions
    {
        public static IGridColumn<T, IHtmlString> AddActionLink<T>(this IGridColumnsOf<T> columns, String action, String iconClass) where T : class
        {
            if (!IsAuthorizedTo(columns.Grid.ViewContext.HttpContext, action))
                return new GridColumn<T, IHtmlString>(columns.Grid, model => null);

            return columns.Add(model => GetLink(columns.Grid.ViewContext.HttpContext, model, action, iconClass)).Css("action-cell");
        }

        public static IGridColumn<T, DateTime> AddDateProperty<T>(this IGridColumnsOf<T> columns, Expression<Func<T, DateTime>> expression)
        {
            return columns.AddProperty(expression).Formatted("{0:d}");
        }
        public static IGridColumn<T, DateTime?> AddDateProperty<T>(this IGridColumnsOf<T> columns, Expression<Func<T, DateTime?>> expression)
        {
            return columns.AddProperty(expression).Formatted("{0:d}");
        }
        public static IGridColumn<T, Boolean> AddBooleanProperty<T>(this IGridColumnsOf<T> columns, Expression<Func<T, Boolean>> expression)
        {
            Func<T, Boolean> valueFor = expression.Compile();

            return columns
                .AddProperty(expression)
                .MultiFilterable(false)
                .RenderedAs(model => valueFor(model) ? Strings.Yes : Strings.No);
        }
        public static IGridColumn<T, Boolean?> AddBooleanProperty<T>(this IGridColumnsOf<T> columns, Expression<Func<T, Boolean?>> expression)
        {
            Func<T, Boolean?> valueFor = expression.Compile();

            return columns
                .AddProperty(expression)
                .MultiFilterable(false)
                .RenderedAs(model =>
                    valueFor(model) != null
                        ? valueFor(model) == true
                            ? Strings.Yes
                            : Strings.No
                        : "");
        }
        public static IGridColumn<T, DateTime> AddDateTimeProperty<T>(this IGridColumnsOf<T> columns, Expression<Func<T, DateTime>> expression)
        {
            return columns.AddProperty(expression).Formatted("{0:g}");
        }
        public static IGridColumn<T, DateTime?> AddDateTimeProperty<T>(this IGridColumnsOf<T> columns, Expression<Func<T, DateTime?>> expression)
        {
            return columns.AddProperty(expression).Formatted("{0:g}");
        }
        public static IGridColumn<T, TProperty> AddProperty<T, TProperty>(this IGridColumnsOf<T> columns, Expression<Func<T, TProperty>> expression)
        {
            return columns
                .Add(expression)
                .Css(GetCssClassFor<TProperty>())
                .Titled(ResourceProvider.GetPropertyTitle(expression));
        }

        public static IHtmlGrid<T> ApplyDefaults<T>(this IHtmlGrid<T> grid)
        {
            return grid
                .Pageable(pager => { pager.RowsPerPage = 16; })
                .Empty(Strings.NoDataFound)
                .AppendCss("table-hover")
                .Filterable()
                .Sortable();
        }

        private static IHtmlString GetLink<T>(HttpContextBase context, T model, String action, String iconClass)
        {
            TagBuilder anchor = new TagBuilder("a");
            anchor.AddCssClass(action.ToLower() + "-action");
            anchor.Attributes["href"] = new UrlHelper(context.Request.RequestContext).Action(action, GetRouteFor(model));

            TagBuilder icon = new TagBuilder("span");
            icon.AddCssClass(iconClass);

            anchor.InnerHtml = icon.ToString();

            return new HtmlString(anchor.ToString());
        }
        private static Boolean IsAuthorizedTo(HttpContextBase context, String action)
        {
            if (Authorization.Provider == null)
                return true;

            Int32? accountId = context.User.Id();
            String area = context.Request.RequestContext.RouteData.Values["area"] as String;
            String controller = context.Request.RequestContext.RouteData.Values["controller"] as String;

            return Authorization.Provider.IsAuthorizedFor(accountId, area, controller, action);
        }
        private static RouteValueDictionary GetRouteFor<T>(T model)
        {
            PropertyInfo key = typeof(T)
                .GetProperties()
                .FirstOrDefault(property => property.IsDefined(typeof(KeyAttribute), false));

            if (key == null)
                throw new Exception(typeof(T).Name + " type does not have a key property.");

            return new RouteValueDictionary { [key.Name] = key.GetValue(model) };
        }
        private static String GetCssClassFor<TProperty>()
        {
            Type type = Nullable.GetUnderlyingType(typeof(TProperty)) ?? typeof(TProperty);
            if (type.IsEnum)
                return "text-left";

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return "text-right";
                case TypeCode.Boolean:
                case TypeCode.DateTime:
                    return "text-center";
                default:
                    return "text-left";
            }
        }
    }
}

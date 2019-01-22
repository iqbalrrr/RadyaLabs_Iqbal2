using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Web.Routing;

namespace RadyaLabs.Resources
{
    public static class ResourceProvider
    {
        private static Dictionary<String, ResourceManager> ViewTitles { get; }
        private static Dictionary<String, ResourceManager> Resources { get; }

        static ResourceProvider()
        {
            Resources = new Dictionary<String, ResourceManager>();
            ViewTitles = new Dictionary<String, ResourceManager>();
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                PropertyInfo property = type.GetProperty("ResourceManager", BindingFlags.Public | BindingFlags.Static);
                if (property != null)
                {
                    ResourceManager manager = property.GetValue(null) as ResourceManager;
                    manager.IgnoreCase = true;

                    if (type.FullName.StartsWith("RadyaLabs.Resources.Views") && type.FullName.EndsWith("View.Titles"))
                        ViewTitles.Add(type.Namespace.Split('.').Last(), manager);
                    else
                        Resources.Add(type.FullName, manager);
                }
            }
        }

        public static String GetDatalistTitle(String datalist)
        {
            return GetResource("RadyaLabs.Resources.Datalist.Titles", datalist ?? "");
        }
        public static String GetPageTitle(RouteValueDictionary values)
        {
            String area = values["area"] as String;
            String action = values["action"] as String;
            String controller = values["controller"] as String;

            return GetResource("RadyaLabs.Resources.Shared.Pages", area + controller + action);
        }
        public static String GetSiteMapTitle(String area, String controller, String action)
        {
            return GetResource("RadyaLabs.Resources.SiteMap.Titles", area + controller + action);
        }

        public static String GetPermissionAreaTitle(String area)
        {
            return GetResource("RadyaLabs.Resources.Permission.Area.Titles", area ?? "");
        }
        public static String GetPermissionControllerTitle(String area, String controller)
        {
            return GetResource("RadyaLabs.Resources.Permission.Controller.Titles", area + controller);
        }
        public static String GetPermissionActionTitle(String area, String controller, String action)
        {
            return GetResource("RadyaLabs.Resources.Permission.Action.Titles", area + controller + action);
        }

        public static String GetPropertyTitle<TModel, TProperty>(Expression<Func<TModel, TProperty>> property)
        {
            return GetPropertyTitle(property.Body);
        }
        public static String GetPropertyTitle(Type view, String property)
        {
            return GetPropertyTitle(view.Name, property ?? "");
        }
        public static String GetPropertyTitle(Expression property)
        {
            MemberExpression expression = property as MemberExpression;

            return expression == null ? null : GetPropertyTitle(expression.Expression.Type, expression.Member.Name);
        }

        private static String GetPropertyTitle(String view, String property)
        {
            String title = GetViewTitle(view, property);
            if (title != null)
                return title;

            String[] properties = SplitCamelCase(property);
            for (Int32 skipped = 0; skipped < properties.Length; skipped++)
            {
                for (Int32 viewSize = 1; viewSize < properties.Length - skipped; viewSize++)
                {
                    String joinedView = String.Concat(properties.Skip(skipped).Take(viewSize)) + "View";
                    String joinedProperty = String.Concat(properties.Skip(viewSize + skipped));

                    title = GetViewTitle(joinedView, joinedProperty);
                    if (title != null)
                        return title;
                }
            }

            return null;
        }
        private static String GetViewTitle(String type, String key)
        {
            return ViewTitles.ContainsKey(type) ? ViewTitles[type].GetString(key) : null;
        }
        private static String GetResource(String type, String key)
        {
            return Resources.ContainsKey(type) ? Resources[type].GetString(key) : null;
        }
        private static String[] SplitCamelCase(String value)
        {
            return Regex.Split(value, "(?<!^)(?=[A-Z])");
        }
    }
}

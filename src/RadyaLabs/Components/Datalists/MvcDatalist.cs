using Datalist;
using RadyaLabs.Data.Core;
using RadyaLabs.Objects;
using RadyaLabs.Resources;
using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace RadyaLabs.Components.Datalists
{
    public class MvcDatalist<TModel, TView> : MvcDatalist<TView>
        where TModel : BaseModel
        where TView : BaseView
    {
        protected IUnitOfWork UnitOfWork { get; set; }

        public MvcDatalist(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
        public MvcDatalist(UrlHelper url)
        {
            String view = typeof(TView).Name.Replace("View", "");
            Url = url.Action(view, "Datalist", new { area = "" });
            Title = ResourceProvider.GetDatalistTitle(view);
        }

        public override String GetColumnHeader(PropertyInfo property)
        {
            return ResourceProvider.GetPropertyTitle(typeof(TView), property.Name) ?? "";
        }
        public override String GetColumnCssClass(PropertyInfo property)
        {
            Type type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
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

        public override IQueryable<TView> GetModels()
        {
            return UnitOfWork.Select<TModel>().To<TView>();
        }
    }
}

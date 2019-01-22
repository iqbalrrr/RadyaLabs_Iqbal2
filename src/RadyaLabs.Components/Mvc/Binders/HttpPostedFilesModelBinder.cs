using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadyaLabs.Components.Mvc
{
    public class HttpPostedFilesModelBinder : IModelBinder
    {
        public Object BindModel(ControllerContext context, ModelBindingContext binding)
        {
            ValueProviderResult value = binding.ValueProvider.GetValue(binding.ModelName);
            if (value?.AttemptedValue == null)
                return null;

            return (value.RawValue as IList<HttpPostedFileBase>).Where(file => file != null).ToList();
        }
    }
}

using RadyaLabs.Resources.Form;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadyaLabs.Components.Mvc
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class AcceptFilesAttribute : ValidationAttribute
    {
        public String Extensions { get; }

        public AcceptFilesAttribute(String extensions)
            : base(() => Validations.AcceptFiles)
        {
            Extensions = extensions;
        }

        public override String FormatErrorMessage(String name)
        {
            return String.Format(ErrorMessageString, name, Extensions);
        }
        public override Boolean IsValid(Object value)
        {
            IEnumerable<HttpPostedFileBase> files = ToFiles(value);

            if (value == null)
                return true;

            if (files == null)
                return false;

            return files.All(file => Extensions.Split(',').Any(extension => file.FileName?.EndsWith(extension) == true));
        }

        private IEnumerable<HttpPostedFileBase> ToFiles(Object value)
        {
            HttpPostedFileBase file = value as HttpPostedFileBase;

            return file != null ? new[] { file } : value as IEnumerable<HttpPostedFileBase>;
        }
    }
}

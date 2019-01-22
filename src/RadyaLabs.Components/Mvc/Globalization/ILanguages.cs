using System;

namespace RadyaLabs.Components.Mvc
{
    public interface ILanguages
    {
        Language Default { get; }
        Language[] Supported { get; }
        Language Current { get; set; }

        Language this[String abbreviation] { get; }
    }
}

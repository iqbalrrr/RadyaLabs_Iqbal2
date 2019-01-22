using RadyaLabs.Components.Mvc;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Mvc
{
    public class LanguagesTests
    {
        private Languages languages;

        static LanguagesTests()
        {
            XElement english = new XElement("language");
            english.SetAttributeValue("default", "true");
            english.SetAttributeValue("name", "English");
            english.SetAttributeValue("culture", "en-GB");
            english.SetAttributeValue("abbreviation", "en");

            XElement lithuanian = new XElement("language");
            lithuanian.SetAttributeValue("name", "Lietuvių");
            lithuanian.SetAttributeValue("culture", "lt-LT");
            lithuanian.SetAttributeValue("abbreviation", "lt");

            XElement languages = new XElement("languages");
            languages.Add(lithuanian);
            languages.Add(english);

            languages.Save("Languages.config");
        }
        public LanguagesTests()
        {
            languages = new Languages("Languages.config");
        }

        #region Current

        [Fact]
        public void Current_ReturnsCurrentLanguage()
        {
            Thread.CurrentThread.CurrentUICulture = languages["en"].Culture;

            Language actual = languages.Current;
            Language expected = languages["en"];

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Current_SetsCurrentLanguage()
        {
            languages.Current = languages.Supported.Last();

            CultureInfo expectedCulture = languages.Supported.Last().Culture;
            CultureInfo actualUICulture = CultureInfo.CurrentUICulture;
            CultureInfo actualCulture = CultureInfo.CurrentCulture;

            Assert.Same(expectedCulture, actualUICulture);
            Assert.Same(expectedCulture, actualCulture);
        }

        #endregion

        #region Languages(String path)

        [Fact]
        public void Languages_SetsSupported()
        {
            Language ltLanguage = languages.Supported.First();
            Language enLanguage = languages.Supported.Last();

            Assert.Equal(new CultureInfo("en-GB"), enLanguage.Culture);
            Assert.Equal("en", enLanguage.Abbreviation);
            Assert.Equal("English", enLanguage.Name);
            Assert.True(enLanguage.IsDefault);

            Assert.Equal(new CultureInfo("lt-LT"), ltLanguage.Culture);
            Assert.Equal("lt", ltLanguage.Abbreviation);
            Assert.Equal("Lietuvių", ltLanguage.Name);
            Assert.False(ltLanguage.IsDefault);
        }

        [Fact]
        public void Languages_SetsDefault()
        {
            Language actual = languages.Default;

            Assert.Equal(new CultureInfo("en-GB"), actual.Culture);
            Assert.Equal("en", actual.Abbreviation);
            Assert.Equal("English", actual.Name);
            Assert.True(actual.IsDefault);
        }

        #endregion

        #region this[String abbreviation]

        [Fact]
        public void Indexer_ReturnsLanguage()
        {
            Language actual = languages["en"];

            Assert.Equal(new CultureInfo("en-GB"), actual.Culture);
            Assert.Equal("en", actual.Abbreviation);
            Assert.Equal("English", actual.Name);
            Assert.True(actual.IsDefault);
        }

        #endregion
    }
}

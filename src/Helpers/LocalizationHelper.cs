namespace SearchDemo.Helpers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Persistence;

    /// <summary>
    /// The localization helper.
    /// </summary>
    public static class LocalizationHelper
    {
        /// <summary>
        /// Returns a collection of installed languages.
        /// <remarks>This method hits the database but the results are cached after the first run.</remarks>
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{Language}"/>.
        /// </returns>
        public static IEnumerable<Language> GetInstalledLanguages()
        {
            const string Key = "installed.languages";

            List<Language> languages = (List<Language>)SiteCache.GetItem(Key);

            if (languages == null)
            {
                languages = new List<Language>();

                // Return a list of all languages in use.
                UmbracoDatabase db = ApplicationContext.Current.DatabaseContext.Database;
                languages.AddRange(
                    db.Query<string>("SELECT [languageISOCode] FROM [umbracoLanguage]")
                      .Select(CultureInfo.GetCultureInfo)
                      .Select(x => new Language(x.Name) { IsoCode = x.Name }));

                SiteCache.AddItem(Key, languages);
            }

            return languages;
        }
    }
}

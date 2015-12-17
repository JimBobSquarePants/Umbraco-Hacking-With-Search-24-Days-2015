namespace SearchDemo.ComponentModel
{
    using System.Collections.Generic;
    using System.Text;

    using Newtonsoft.Json;

    using Our.Umbraco.Vorto.Extensions;

    using SearchDemo.Helpers;

    using Umbraco.Core.Models;

    /// <summary>
    /// The Vorto search resolver. Used to resolve a value suitable for indexing with Examine.
    /// </summary>
    public class VortoSearchValueResolver : SearchValueResolver<SearchResolverAttribute>
    {
        /// <summary>
        /// The resolve value.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ResolveValue()
        {
            IEnumerable<Language> languages = LocalizationHelper.GetInstalledLanguages();
            StringBuilder stringBuilder = new StringBuilder();
            VortoValue vortoValue = JsonConvert.DeserializeObject<VortoValue>(this.RawValue);
            string name = this.Property.Name;

            foreach (Language language in languages)
            {
                string iso = language.IsoCode;
                if (this.Content.HasVortoValue(name, iso))
                {
                    object value;

                    // Umbraco method Parse internal links fails since we are operating on a background thread.
                    try
                    {
                        value = this.Content.GetVortoValue(name, iso);
                    }
                    catch
                    {
                        value = vortoValue.Values[iso];
                    }

                    stringBuilder.Append(string.Format(SearchConstants.CultureTemplate, iso, value));
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Represents the Vorto value stored in the database.
        /// </summary>
        internal class VortoValue
        {
            /// <summary>
            /// Gets or sets the values.
            /// </summary>
            [JsonProperty("values")]
            public IDictionary<string, object> Values { get; set; }
        }
    }
}

namespace SearchDemo.ComponentModel
{
    using System.Globalization;
    using System.Reflection;

    using Umbraco.Core.Models;

    /// <summary>
    /// Provides methods to convert values to values suitable for indexing with Examine.
    /// </summary>
    public abstract class SearchValueResolver
    {
        /// <summary>
        /// Gets or sets the associated attribute.
        /// </summary>
        public SearchResolverAttribute Attribute { get; protected set; }

        /// <summary>
        /// Gets or sets the content to resolve the value for.
        /// </summary>
        public IPublishedContent Content { get; protected set; }

        /// <summary>
        /// Gets or sets the property to resolve the value for.
        /// </summary>
        public PropertyInfo Property { get; protected set; }

        /// <summary>
        /// Gets or sets the raw value.
        /// </summary>
        public string RawValue { get; protected set; }

        /// <summary>
        /// Gets or sets the culture object.
        /// </summary>
        public CultureInfo Culture { get; protected set; }

        /// <summary>
        /// Performs the value resolution.
        /// </summary>
        /// /// <returns>
        /// The <see cref="string"/> representing the converted value.
        /// </returns>
        public abstract string ResolveValue();

        /// <summary>
        /// Converts the raw value from an Umbraco property into a format that can be indexed by Examine.
        /// </summary>
        /// <param name="attribute">
        /// The <see cref="SearchResolverAttribute"/> containing additional information  indicating how to resolve the property.
        /// </param>
        /// <param name="content">The <see cref="IPublishedContent"/>to resolve the value for.</param>
        /// <param name="property">The <see cref="PropertyInfo"/> to resolve the value for.</param>
        /// <param name="rawValue">The raw property value from Umbraco.</param>
        /// <param name="culture"> The <see cref="CultureInfo"/> to help parse values with the correct culture.</param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal virtual string ResolveValue(SearchResolverAttribute attribute, IPublishedContent content, PropertyInfo property, string rawValue, CultureInfo culture)
        {
            this.Attribute = attribute;
            this.Content = content;
            this.RawValue = rawValue;
            this.Culture = culture;

            return this.ResolveValue();
        }
    }
}
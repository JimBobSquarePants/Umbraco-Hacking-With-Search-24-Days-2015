namespace SearchDemo.ComponentModel
{
    using System;
    using System.Globalization;
    using System.Reflection;

    using global::Umbraco.Core.Models;

    /// <summary>
    /// Provides methods to convert values to values suitable for indexing with Examine.
    /// </summary>
    /// <typeparam name="TAttributeType">
    /// The <see cref="Type"/> of attribute that provides information for this resolver.
    /// </typeparam>
    public abstract class SearchValueResolver<TAttributeType> : SearchValueResolver
        where TAttributeType : SearchResolverAttribute
    {
        /// <summary>
        /// Gets or sets the associated attribute.
        /// </summary>
        public new TAttributeType Attribute { get; protected set; }

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
        internal override string ResolveValue(SearchResolverAttribute attribute, IPublishedContent content, PropertyInfo property, string rawValue, CultureInfo culture)
        {
            if (!(attribute is TAttributeType))
            {
                throw new ArgumentException(
                    "The resolver attribute must be of type " + typeof(TAttributeType).AssemblyQualifiedName,
                    nameof(attribute));
            }

            this.Attribute = (TAttributeType)attribute;
            this.Content = content;
            this.Property = property;
            this.RawValue = rawValue;
            this.Culture = culture;

            return this.ResolveValue();
        }
    }
}

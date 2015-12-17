namespace SearchDemo.ComponentModel
{
    using Our.Umbraco.Ditto;
    using Our.Umbraco.Vorto.Extensions;

    using Umbraco.Core.Models;

    /// <summary>
    /// The Vorto value resolver.
    /// </summary>
    public class VortoValueResolver : DittoValueResolver<DittoValueResolverContext, VortoPropertyAttribute>
    {
        /// <summary>
        /// Default back to the default culture;
        /// </summary>
        private static string fallbackCultureName = "en-AU";

        /// <summary>
        /// Gets the raw value for the current property from Umbraco.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/> representing the raw value.
        /// </returns>
        public override object ResolveValue()
        {
            string umbracoPropertyName = this.Attribute.PropertyName;
            bool recursive = this.Attribute.Recursive;

            if (this.Context.PropertyDescriptor != null)
            {
                if (string.IsNullOrWhiteSpace(umbracoPropertyName))
                {
                    umbracoPropertyName = this.Context.PropertyDescriptor.Name;
                }
            }

            IPublishedContent content = this.Context.Instance as IPublishedContent;
            return content.GetVortoValue(umbracoPropertyName, null, recursive, null, fallbackCultureName);
        }
    }
}

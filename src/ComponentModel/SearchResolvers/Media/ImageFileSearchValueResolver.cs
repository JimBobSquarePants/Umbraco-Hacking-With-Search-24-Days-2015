namespace SearchDemo.ComponentModel
{
    using Umbraco.Core;
    using Umbraco.Web;
    using Umbraco.Web.Models;

    /// <summary>
    /// The image filename search resolver. Used to resolve a value suitable for indexing with Examine.
    /// </summary>
    public class ImageFileSearchValueResolver : SearchValueResolver<SearchResolverAttribute>
    {
        /// <summary>
        /// Performs the value resolution.
        /// </summary>
        /// /// <returns>
        /// The <see cref="string"/> representing the converted value.
        /// </returns>
        public override string ResolveValue()
        {
            string umbracoFile = Constants.Conventions.Media.File;
            return this.Content.GetPropertyValue<ImageCropDataSet>(umbracoFile).Src;
        }
    }
}

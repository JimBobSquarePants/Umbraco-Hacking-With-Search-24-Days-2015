namespace SearchDemo.Models
{
    using System;
    using System.Web;

    /// <summary>
    /// Provides extension methods for the <see cref="Image"/> class.
    /// </summary>
    public static class ImageExtensions
    {
        /// <summary>
        /// Gets the url to the image. Use this instead of the url property.
        /// </summary>
        /// <param name="image">The <see cref="Image"/> this method extends.</param>
        /// <returns>
        /// The <see cref="string"/> representing the url.
        /// </returns>
        public static string Url(this Image image)
        {
            return image.Crops.Src;
        }

        /// <summary>
        /// Gets the absolute url to the image.
        /// </summary>
        /// <param name="image">The <see cref="Image"/> this method extends.</param>
        /// <returns>
        /// The <see cref="string"/> representing the url.
        /// </returns>
        public static string UrlAbsolute(this Image image)
        {
            string root = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            return new Uri(new Uri(root, UriKind.Absolute), image.Crops.Src).ToString();
        }

        /// <summary>
        /// Gets the ImageProcessor Url by the crop alias (from the "umbracoFile" property alias) 
        /// on the <see cref="Image"/> item. 
        /// </summary>
        /// <param name="image">The <see cref="Image"/> this method extends.</param>
        /// <param name="alias">The crop alias <example>thumbnail</example>.</param>
        /// <param name="useCropDimensions"></param>
        /// <param name="useFocalPoint">Whether to use the focal point.</param>
        /// <param name="quality">The quality of jpeg images.</param>
        /// <returns>The <see cref="ImageProcessor.Web"/> Url. </returns>
        public static string GetCropUrl(this Image image, string alias, bool useCropDimensions = true, bool useFocalPoint = false, int quality = 85)
        {
            return $"{image.Crops.Src}{image.Crops.GetCropUrl(alias, useCropDimensions, useFocalPoint) + "&quality=" + quality}";
        }
    }
}

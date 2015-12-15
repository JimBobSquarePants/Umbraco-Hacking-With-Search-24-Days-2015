namespace SearchDemo.Models
{
    using System.ComponentModel;

    using Newtonsoft.Json;

    using Our.Umbraco.Ditto;

    using SearchDemo.ComponentModel;

    using Umbraco.Core;
    using Umbraco.Web.Models;

    /// <summary>
    /// Encapsulates the basic properties to identify the object as an image
    /// to the Umbraco back office.
    /// </summary>
    [SearchCategory(new[] { "Media" })]
    [TypeConverter(typeof(DittoPickerConverter))]
    public class Image : PublishedEntity
    {
        /// <summary>
        /// Gets or sets a name.
        /// </summary>
        [SearchMergedField("nodeName")]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        [UmbracoProperty(Constants.Conventions.Media.File)]
        public virtual string FileName { get; set; }

        /// <summary>
        /// Gets or sets the size of the media file in bytes.
        /// </summary>
        [UmbracoProperty(Constants.Conventions.Media.Bytes)]
        public virtual int Bytes { get; set; }

        /// <summary>
        /// Gets or sets the file extension.
        /// </summary>
        [UmbracoProperty(Constants.Conventions.Media.Extension)]
        public virtual string Extension { get; set; }

        /// <summary>
        /// Gets or sets the width in pixels.
        /// </summary>
        [UmbracoProperty(Constants.Conventions.Media.Width)]
        public virtual int Width { get; set; }

        /// <summary>
        /// Gets or sets the height in pixels.
        /// </summary>
        [UmbracoProperty(Constants.Conventions.Media.Height)]
        public virtual int Height { get; set; }

        /// <summary>
        /// Gets or sets the crops.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [UmbracoProperty(Constants.Conventions.Media.File)]
        public virtual ImageCropDataSet Crops { get; set; }

        /// <summary>
        /// Gets the url.
        /// <remarks>
        /// Umbraco cannot internally resolve both the url and the crops due to an 
        /// issue that prevents using the property value converter to do the calculation. 
        /// Use the <see cref="M:Url"/> method instead.
        /// </remarks>
        /// </summary>
        [DittoIgnore]
        [JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Url => string.Empty;
    }
}

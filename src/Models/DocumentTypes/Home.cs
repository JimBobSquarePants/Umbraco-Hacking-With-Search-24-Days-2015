namespace SearchDemo.Models
{
    using System.Web;

    using SearchDemo.ComponentModel;

    /// <summary>
    /// The home document type.
    /// </summary>
    [SearchCategory(new[] { "Content" })]
    public class Home : PageBase
    {
        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public virtual Image Image { get; set; }

        /// <summary>
        /// Gets or sets the body text.
        /// </summary>
        [SearchMergedField]
        public virtual HtmlString BodyText { get; set; }
    }
}
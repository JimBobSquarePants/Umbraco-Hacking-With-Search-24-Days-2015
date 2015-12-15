namespace SearchDemo.Models
{
    using System.Globalization;

    using Umbraco.Web;

    /// <summary>
    /// The render meta page base model for rendering pages with metadata.
    /// <remarks>
    /// Not abstract as we need to create instances of it for 3rd party plugin integration.
    /// </remarks>
    /// </summary>
    public class RenderPageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderPageBase"/> class. 
        /// </summary>
        /// <param name="content">The <see cref="PageBase"/> to create the view model from.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> providing information about the specific culture.</param>
        public RenderPageBase(PageBase content, CultureInfo culture)
        {
            this.Content = content;
            this.CurrentCulture = culture;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderPageBase"/> class. 
        /// </summary>
        /// <param name="content">
        /// The <see cref="PageBase"/> to create the view model from.</param>
        public RenderPageBase(PageBase content)
        {
            this.Content = content;
            this.CurrentCulture = UmbracoContext.Current.PublishedContentRequest.Culture;
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        public PageBase Content { get; private set; }

        /// <summary>
        /// Gets the culture.
        /// </summary>
        public CultureInfo CurrentCulture { get; private set; }
    }
}

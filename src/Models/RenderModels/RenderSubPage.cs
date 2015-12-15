namespace SearchDemo.Models
{
    using System.Globalization;

    /// <summary>
    /// The render model for the sub page document type.
    /// </summary>
    public class RenderSubPage : RenderPageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderSubPage"/> class. 
        /// </summary>
        /// <param name="content">The <see cref="SubPage"/> to create the view model from.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> providing information about the specific culture.</param>
        public RenderSubPage(SubPage content, CultureInfo culture)
            : base(content, culture)
        {
            this.Content = content;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderSubPage"/> class. 
        /// </summary>
        /// <param name="content">The <see cref="SubPage"/> to create the view model from.</param>
        public RenderSubPage(SubPage content)
            : base(content)
        {
            this.Content = content;
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        public new SubPage Content { get; private set; }
    }
}
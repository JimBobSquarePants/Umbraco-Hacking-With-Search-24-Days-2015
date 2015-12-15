namespace SearchDemo.Models
{
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// The render model for the home document type.
    /// </summary>
    public class RenderHome : RenderPageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderHome"/> class. 
        /// </summary>
        /// <param name="content">The <see cref="Home"/> to create the view model from.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> providing information about the specific culture.</param>
        public RenderHome(Home content, CultureInfo culture)
            : base(content, culture)
        {
            this.Content = content;
            this.SubPages = new HashSet<SubPage>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderHome"/> class. 
        /// </summary>
        /// <param name="content">The <see cref="Home"/> to create the view model from.</param>
        public RenderHome(Home content)
            : base(content)
        {
            this.Content = content;
            this.SubPages = new HashSet<SubPage>();
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        public new Home Content { get; private set; }

        /// <summary>
        /// Gets or sets the sub pages.
        /// </summary>
        public ICollection<SubPage> SubPages { get; set; }
    }
}
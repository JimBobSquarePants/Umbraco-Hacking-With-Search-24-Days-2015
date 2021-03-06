﻿namespace SearchDemo.Models
{
    using System.Web;

    using SearchDemo.ComponentModel;

    /// <summary>
    /// The sub page document type.
    /// </summary>
    [SearchCategory(new[] { "Content" })]
    public class SubPage : PageBase
    {
        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public virtual Image Image { get; set; }

        /// <summary>
        /// Gets or sets the body text.
        /// </summary>
        [SearchMergedField]
        [VortoProperty]
        [VortoSearchResolver]
        public virtual HtmlString BodyText { get; set; }
    }
}
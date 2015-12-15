namespace SearchDemo.Models
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Web;

    using Our.Umbraco.Ditto;

    using SearchDemo.ComponentModel;
    using SearchDemo.Extensions;
    using SearchDemo.Helpers;

    using Umbraco.Core;

    /// <summary>
    /// Provides base functionality for pages with extended meta data.
    /// Not abstract so we can use it as a property.
    /// </summary>
    [TypeConverter(typeof(DittoPickerConverter))]
    public class PageBase : PublishedEntity
    {
        /// <summary>
        /// Ensures operations are atomic.
        /// </summary>
        private readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        /// <summary>
        /// The lazily initialized absolute url.
        /// </summary>
        private Lazy<string> urlAbsolute;

        /// <summary>
        /// Gets or sets a name for the the navigable page.
        /// </summary>
        [SearchMergedField("nodeName")]
        [UmbracoProperty("NavigationNameOverride", "Name")]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the url for the navigable page.
        /// </summary>
        [UmbracoProperty(Constants.Conventions.Content.UrlName, "Url")]
        public virtual string Url { get; set; }

        /// <summary>
        /// Gets the absolute url for this page.
        /// </summary>
        [DittoIgnore]
        public string UrlAbsolute
        {
            get
            {
                this.locker.EnterReadLock();
                try
                {
                    if (this.urlAbsolute == null)
                    {
                        this.urlAbsolute = new Lazy<string>(
                            () =>
                            {
                                string url = ContentHelper.Instance.UmbracoHelper.UrlAbsolute(this.Id);

                                // Hackathon! 
                                // Certain virtual pages such as Articulate blog pages are only returning a relative url.
                                if (!url.IsAbsoluteUrl())
                                {
                                    string root = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                                    url = new Uri(new Uri(root, UriKind.Absolute), url).ToString();
                                }

                                return url;
                            });
                    }
                }
                finally
                {
                    this.locker.ExitReadLock();
                }

                return this.urlAbsolute.Value;
            }
        }
    }
}

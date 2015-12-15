namespace SearchDemo.ComponentModel
{
    using System;

    /// <summary>
    /// The image filename search resolver attribute. 
    /// Used to indicate that the search engine should run additional conversion code to 
    /// index this property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ImageFileSearchResolverAttribute : SearchResolverAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFileSearchResolverAttribute"/> class.
        /// </summary>
        public ImageFileSearchResolverAttribute()
            : base(typeof(ImageFileSearchValueResolver))
        {
        }
    }
}

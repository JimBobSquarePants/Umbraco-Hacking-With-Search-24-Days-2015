namespace SearchDemo.ComponentModel
{
    using System;

    /// <summary>
    /// The vorto search resolver attribute. Used to indicate that the search engine should run additional conversion code to 
    /// index this property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class VortoSearchResolverAttribute : SearchResolverAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VortoSearchResolverAttribute"/> class.
        /// </summary>
        public VortoSearchResolverAttribute()
            : base(typeof(VortoSearchValueResolver))
        {
        }
    }
}

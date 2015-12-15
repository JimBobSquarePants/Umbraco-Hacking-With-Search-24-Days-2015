namespace SearchDemo.ComponentModel
{
    using System;

    /// <summary>
    /// The search merged field attribute. Used to tell Examine that the contents of this property should be added to
    /// a single merged search field for indexing. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SearchMergedFieldAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchMergedFieldAttribute"/> class.
        /// </summary>
        public SearchMergedFieldAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchMergedFieldAttribute"/> class.
        /// </summary>
        /// <param name="examineKey">
        /// The examine key identifier for nodes that do not match the name of your property.
        /// </param>
        public SearchMergedFieldAttribute(string examineKey)
        {
            this.ExamineKey = examineKey;
        }

        /// <summary>
        /// Gets or sets the examine key used by Umbraco that overrides the property name.
        /// </summary>
        public string ExamineKey { get; set; }
    }
}

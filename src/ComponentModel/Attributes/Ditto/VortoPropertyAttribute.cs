namespace SearchDemo.ComponentModel
{
    using Our.Umbraco.Ditto;

    /// <summary>
    /// The vorto property attribute.
    /// Used for returning multilingual properties from Umbraco via the Vorto plugin.
    /// </summary>
    public class VortoPropertyAttribute : DittoValueResolverAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VortoPropertyAttribute"/> class.
        /// </summary>
        public VortoPropertyAttribute()
            : base(typeof(VortoValueResolver))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VortoPropertyAttribute"/> class.
        /// </summary>
        /// <param name="recursive">Whether the property should be retrieved recursively up the tree.</param>
        public VortoPropertyAttribute(bool recursive = false)
            : base(typeof(VortoValueResolver))
        {
            this.Recursive = recursive;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VortoPropertyAttribute"/> class.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="recursive">Whether the property should be retrieved recursively up the tree.</param>
        public VortoPropertyAttribute(string propertyName, bool recursive = false)
            : base(typeof(VortoValueResolver))
        {
            this.PropertyName = propertyName;
            this.Recursive = recursive;
        }

        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the property should be retrieved recursively up the tree.
        /// </summary>
        public bool Recursive { get; set; }
    }
}

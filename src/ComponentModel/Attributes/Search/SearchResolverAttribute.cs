namespace SearchDemo.ComponentModel
{
    using System;

    /// <summary>
    /// Defines how a properties value can be resolved to a value suitable for indexing with Examine.
    /// </summary>
    public class SearchResolverAttribute : Attribute
    {
        /// <summary>
        /// The resolver type
        /// </summary>
        private readonly Type resolverType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResolverAttribute"/> class.
        /// </summary>
        /// <param name="resolverType">Type of the resolver.</param>
        /// <exception cref="ArgumentException">Resolver type must inherit from DittoValueResolver;resolverType</exception>
        public SearchResolverAttribute(Type resolverType)
        {
            if (!typeof(SearchValueResolver).IsAssignableFrom(resolverType))
            {
                throw new ArgumentException("Resolver type must inherit from SearchValueResolver", nameof(resolverType));
            }

            this.resolverType = resolverType;
        }

        /// <summary>
        /// Gets the type of the resolver.
        /// </summary>
        /// <value>
        /// The type of the resolver.
        /// </value>
        public Type ResolverType => this.resolverType;

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            SearchResolverAttribute other = obj as SearchResolverAttribute;
            return (other != null) && other.ResolverType.AssemblyQualifiedName == this.resolverType.AssemblyQualifiedName;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            // ReSharper disable once PossibleNullReferenceException
            return this.resolverType.AssemblyQualifiedName.GetHashCode();
        }
    }
}

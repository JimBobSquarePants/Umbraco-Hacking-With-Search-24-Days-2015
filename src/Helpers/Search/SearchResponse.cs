﻿namespace SearchDemo.Helpers
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains the matches for a given search request.
    /// </summary>
    internal class SearchResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResponse"/> class.
        /// </summary>
        public SearchResponse()
        {
            this.SearchMatches = new HashSet<SearchMatch>();
        }

        /// <summary>
        /// Gets or sets the total number of search results that could have been returned for the <see cref="SearchRequest"/> executed.
        /// </summary>
        public int TotalCount { get; internal set; }

        /// <summary>
        /// Gets a collection of <see cref="SearchMatch"/> that are content items, together with a highlight snippet.
        /// </summary>
        public ICollection<SearchMatch> SearchMatches { get; private set; }
    }
}

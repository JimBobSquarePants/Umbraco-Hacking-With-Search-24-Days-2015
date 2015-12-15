namespace SearchDemo.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Web;

    /// <summary>
    /// Provides methods allowing searching for objects within the application.
    /// </summary>
    public static class SearchEngine
    {
        /// <summary>
        /// Searches within the current site for the given query.
        /// </summary>
        /// <param name="query">The query containing information to search for.</param>
        /// <param name="categories">The categories, if any, to restrict a search to.</param>
        /// <param name="cultures">The collection of <see cref="CultureInfo"/>, if any, to restrict the search to.</param>
        /// <param name="skip">The number of matches to skip.</param>
        /// <param name="take">The number of matches to take.</param>
        /// <returns>
        /// The <see cref="IEnumerable{SearchMatch}"/>.
        /// </returns>
        public static IEnumerable<SearchMatch> SearchSite(string query, string[] categories = null, CultureInfo[] cultures = null, int skip = 0, int take = int.MaxValue)
        {
            SearchRequest request = new SearchRequest
            {
                Query = query,
                Categories = categories ?? new string[0],
                Cultures = cultures ?? new[] { Thread.CurrentThread.CurrentUICulture },
                Skip = skip,
                Take = take
            };

            // We want to constrain searches to the current site only.
            string root = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            if (cultures == null)
            {
                return request.Execute().SearchMatches.Where(m => m.UrlAbsolute.StartsWith(root));
            }

            // Search results should match the current culture.
            return request.Execute().SearchMatches.Where(m => m.UrlAbsolute.StartsWith(root));
        }

        /// <summary>
        /// Searches across multiple sites for the given query.
        /// </summary>
        /// <param name="query">The query containing information to search for.</param>
        /// <param name="categories">The categories, if any, to restrict a search to.</param>
        /// <param name="cultures">The collection of <see cref="CultureInfo"/>, if any, to restrict the search to.</param>
        /// <param name="skip">The number of matches to skip.</param>
        /// <param name="take">The number of matches to take.</param>
        /// <returns>
        /// The <see cref="IEnumerable{SearchMatch}"/>.
        /// </returns>
        public static IEnumerable<SearchMatch> SearchMultipleSites(string query, string[] categories = null, CultureInfo[] cultures = null, int skip = 0, int take = int.MaxValue)
        {
            SearchRequest request = new SearchRequest()
            {
                Query = query,
                Categories = categories ?? new string[0],
                Cultures = cultures ?? new[] { Thread.CurrentThread.CurrentUICulture },
                Skip = skip,
                Take = take
            };

            return request.Execute().SearchMatches;
        }
    }
}

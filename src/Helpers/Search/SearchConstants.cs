namespace SearchDemo.Helpers
{
    /// <summary>
    /// Constants for ensuring the correct values are used across search functionality.
    /// </summary>
    public static class SearchConstants
    {
        /// <summary>
        /// The name of the Examine index to search within.
        /// </summary>
        public const string IndexerName = "ExternalIndexer";

        /// <summary>
        /// The name of the Examine searcher to use to perform searches.
        /// </summary>
        public const string SearcherName = "ExternalSearcher";

        /// <summary>
        /// The name of the field that will contain and merged data.
        /// </summary>
        public const string MergedDataField = "SearchDemoMergedData";

        /// <summary>
        /// The name of the field that will contain category specific information.
        /// </summary>
        public const string CategoryField = "SearchDemoCategoryField";

        /// <summary>
        /// The maximum number of highlight fragments to display in a search result.
        /// </summary>
        public const int HighlightFragements = 3;

        /// <summary>
        /// The culture template for specifying a culture for the search result.
        /// </summary>
        public const string CultureTemplate = "\u0000SearchDemoCulture:{0}:{1}\u0000";

        /// <summary>
        /// The regular expression for detecting culture identifiers in search results.
        /// </summary>
        public const string CultureRegexTemplate = "\u0000SearchDemoCulture:{0}:.*{1}[^\u0000]+\u0000";

        /// <summary>
        /// The regular expression for detecting all culture identifiers in search results.
        /// </summary>
        public const string AllCultureRegexTemplate = "\u0000SearchDemoCulture:[^\u0000]+:(?<replacement>[^\u0000]+)\u0000";

        /// <summary>
        /// The search query regex template.
        /// </summary>
        public const string QueryRegexTemplate = ".*{0}+";
    }
}

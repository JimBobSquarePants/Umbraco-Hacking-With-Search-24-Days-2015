namespace SearchDemo.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;

    using Examine;
    using Examine.LuceneEngine;
    using Examine.Providers;
    using Examine.SearchCriteria;
    using Examine.LuceneEngine.SearchCriteria;

    using Umbraco.Core;
    using Umbraco.Core.Models;

    using Lucene.Net.Analysis;
    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.Highlight;
    using Lucene.Net.QueryParsers;
    using Lucene.Net.Search;

    /// <summary>
    /// Allows the creation and execution of searches against the Examine index.
    /// </summary>
    internal class SearchRequest
    {
        /// <summary>
        /// The culture regex for parsing all culture matches.
        /// </summary>
        private static readonly Regex AllCultureRegex = new Regex(SearchConstants.AllCultureRegexTemplate, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// The collection of saved languages
        /// </summary>
        private readonly IEnumerable<Language> languages;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchRequest"/> class.
        /// </summary>
        public SearchRequest()
        {
            this.Query = string.Empty;
            this.Categories = new string[0];
            this.Cultures = new[] { Thread.CurrentThread.CurrentUICulture };
            this.Skip = 0;
            this.Take = int.MaxValue;

            this.languages = LocalizationHelper.GetInstalledLanguages();
        }

        /// <summary>
        /// Gets or sets the search phrase - free text search query.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets the number of results to skip from the beginning of the result set.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Gets or sets the limit of the number of results to return.
        /// </summary>
        public int Take { get; set; }

        /// <summary>
        /// Gets or sets the categories by which to filter the search.
        /// </summary>
        public string[] Categories { get; set; }

        /// <summary>
        /// Gets or sets the cultures by which to filter the search.
        /// </summary>
        public CultureInfo[] Cultures { get; set; }

        /// <summary>
        /// Executes the search.
        /// </summary>
        /// <returns>The <see cref="SearchResponse"/> containing the search results.</returns>
        public SearchResponse Execute()
        {
            SearchResponse searchResponse = new SearchResponse();
            BaseSearchProvider searchProvider = ExamineManager.Instance.SearchProviderCollection[SearchConstants.SearcherName];

            IBooleanOperation searchCriteria = searchProvider.CreateSearchCriteria().OrderBy(string.Empty);

            if (!string.IsNullOrWhiteSpace(this.Query))
            {
                searchCriteria = searchProvider
                    .CreateSearchCriteria()
                    .GroupedOr(SearchConstants.MergedDataField.AsEnumerableOfOne(),
                    this.Query.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(w => w.Trim().MultipleCharacterWildcard())
                    .ToArray());
            }

            if (this.Categories.Any())
            {
                searchCriteria.And().Field(SearchConstants.CategoryField, string.Join(" ", this.Categories));
            }

            if (searchCriteria != null)
            {
                ISearchResults searchResults = null;
                try
                {
                    searchResults = searchProvider.Search(searchCriteria.Compile());
                }
                catch (NullReferenceException)
                {
                    // If the query object can't be compiled then an exception within Examine is raised
                }

                if (searchResults != null)
                {
                    Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
                    Formatter formatter = new SimpleHTMLFormatter("<strong>", "</strong>");

                    foreach (SearchResult searchResult in searchResults.OrderByDescending(x => x.Score))
                    {
                        // Check to see if the result is culture specific.
                        // This is a bit hacky but there is no way with property wrappers like Vorto to separate the results into 
                        // different indexes so we have to fall back to regular expressions.
                        string fieldResult = searchResult.Fields[SearchConstants.MergedDataField];
                        RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline;

                        string opts = $"({string.Join("|", this.Query.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries))})";

                        // First check to see if there is any matches for any installed languages and remove any
                        // That are not in our culture collection.
                        // ReSharper disable once LoopCanBeConvertedToQuery
                        foreach (Language language in this.languages)
                        {
                            if (!this.Cultures.Contains(language.CultureInfo))
                            {
                                fieldResult = Regex.Replace(
                                    fieldResult,
                                    string.Format(SearchConstants.CultureRegexTemplate, language.IsoCode, opts),
                                    string.Empty,
                                    options);
                            }
                        }

                        // Now clean up the languages we do have a result for.
                        MatchCollection matches = AllCultureRegex.Matches(fieldResult);

                        foreach (Match match in matches)
                        {
                            if (match.Success)
                            {
                                string replacement = match.Groups["replacement"].Value;

                                fieldResult = Regex.Replace(
                                fieldResult,
                                Regex.Escape(match.Value),
                                replacement + " ",
                                options);
                            }
                        }

                        // Now check to see if we have any match left over. If not, break out.
                        if (!new Regex(string.Format(SearchConstants.QueryRegexTemplate, opts), options).Match(fieldResult).Success)
                        {
                            continue;
                        }

                        this.AddSearchMatch(analyzer, formatter, searchResults, searchResponse, searchResult, fieldResult);
                    }

                    searchResponse.TotalCount = searchResponse.SearchMatches.Count;
                }
            }

            return searchResponse;
        }

        private void AddSearchMatch(Analyzer analyzer, Formatter formatter, ISearchResults searchResults, SearchResponse searchResponse, SearchResult searchResult, string fieldResult)
        {
            string highlight = this.GetHighlight(
                           analyzer,
                           formatter,
                           (SearchResults)searchResults,
                           fieldResult);

            string[] categories = searchResult.Fields[SearchConstants.CategoryField]?.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];

            switch (searchResult.Fields["__IndexType"])
            {
                case "content":
                    searchResponse.SearchMatches.Add(new SearchMatch(searchResult.Id, highlight, categories));
                    break;

                case "media":
                    searchResponse.SearchMatches.Add(new SearchMatch(searchResult.Id, highlight, categories));
                    break;
            }
        }

        /// <summary>
        /// Gets a highlight text for a given search result item
        /// </summary>
        /// <param name="analyzer">The analyzer which extracts index terms from text.</param>
        /// <param name="formatter">The formatter that adds markup highlighting matches.</param>
        /// <param name="searchResults">The search results</param>
        /// <param name="searchResultFieldValue">The field value</param>
        /// <returns>The <see cref="string"/></returns>
        private string GetHighlight(Analyzer analyzer, Formatter formatter, SearchResults searchResults, string searchResultFieldValue)
        {
            Highlighter highlighter = new Highlighter(formatter, FragmentScorer(this.Query, SearchConstants.MergedDataField, searchResults));

            TokenStream tokenStream = analyzer.TokenStream(SearchConstants.MergedDataField, new StringReader(searchResultFieldValue));

            return highlighter.GetBestFragments(tokenStream, searchResultFieldValue, SearchConstants.HighlightFragements, "...");
        }

        /// <summary>
        /// Gets the query fragment scorer for highlighting.
        /// </summary>
        /// <param name="query">The query search term.</param>
        /// <param name="highlightField">The highlight field from which to create highlights.</param>
        /// <param name="searchResults">The search results.</param>
        /// <returns>The <see cref="QueryScorer"/></returns>
        private static QueryScorer FragmentScorer(string query, string highlightField, SearchResults searchResults)
        {
            return new QueryScorer(GetLuceneQueryObject(query, highlightField).Rewrite(((IndexSearcher)searchResults.LuceneSearcher).GetIndexReader()));
        }

        /// <summary>
        /// Gets the Lucene query for creating highlight from.
        /// </summary>
        /// <param name="query">The query search term.</param>
        /// <param name="highlightField">The highlight field from which to create highlights.</param>
        /// <returns>The <see cref="Query"/></returns>
        private static Query GetLuceneQueryObject(string query, string highlightField)
        {
            QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, highlightField, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29));

            // Allow for wildcard fragments. 
            parser.SetMultiTermRewriteMethod(MultiTermQuery.SCORING_BOOLEAN_QUERY_REWRITE);

            return parser.Parse($"{highlightField}:{string.Join(" ", query.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(w => w.Trim().MultipleCharacterWildcard().Value))}");
        }
    }
}

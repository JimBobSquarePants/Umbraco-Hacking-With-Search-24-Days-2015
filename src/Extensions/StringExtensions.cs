namespace SearchDemo.Extensions
{
    using System;

    /// <summary>
    /// Extension methods for the <see cref="String"/> class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a value indicating whether a given url is absolute.
        /// </summary>
        /// <param name="url">
        /// The input string that this method extends.
        /// </param>
        /// <returns>
        /// The true if the url is absolute; otherwise, false.
        /// </returns>
        public static bool IsAbsoluteUrl(this string url)
        {
            if (url.StartsWith("//"))
            {
                return true;
            }

            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }
    }
}

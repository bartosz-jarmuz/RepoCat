using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RepoCat.Utilities
{
    /// <summary>
    /// Gets tokens (words, phrases or exclusions) from a search string
    /// </summary>
    public static class QueryStringTokenizer
    {
        /// <summary>
        ///  Gets tokens (words, phrases or exclusions) from a search string
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public static List<string> GetTokens(string queryString)
        {
            if (!string.IsNullOrEmpty(queryString))
            {
                var re = new Regex("(?<=\")[^\"]*(?=\")|[^\" ]+");
                return re.Matches(queryString).Cast<Match>().Select(m => m.Value).ToList();
            }
            return new List<string>();
        }

       
    }
}

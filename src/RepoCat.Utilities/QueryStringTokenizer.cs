using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RepoCat.Utilities
{
    public static class QueryStringTokenizer
    {
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;
using RepoCat.Portal.Models;
using RepoCat.Portal.Models.Domain;

namespace RepoCat.Portal.Services
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

using System;
using System.Collections.Generic;
using RepoCat.Utilities;

namespace RepoCat.Portal.Models
{
    public class ManifestQueryResultViewModel
    {
        private List<string> searchTokens;
        public bool IsRegex { get; set; }

        public string QueryString { get; set; }

        private List<string> GetSearchTokens()
        {
            if (!this.IsRegex)
            {
                return QueryStringTokenizer.GetTokens(this.QueryString);
            }
            else
            {
               return new List<string>() {this.QueryString };
            }

        }

        public List<string> SearchTokens
        {
            get
            {
                if (this.searchTokens == null)
                {
                    this.searchTokens = this.GetSearchTokens();
                }
                return this.searchTokens;
            }
            set => this.searchTokens = value;
        }

        public List<ProjectManifestViewModel> Manifests { get; set; } = new List<ProjectManifestViewModel>();
        public TimeSpan Elapsed { get; set; }
        public string RepoStamp { get; set; }
        public string Repo { get; set; }
    }
}
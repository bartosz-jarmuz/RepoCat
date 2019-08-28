using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoCat.Portal.Data.RepoCatDb
{
    namespace BooksApi.Models
    {
        public class RepoCatDbSettings : IRepoCatDbSettings
        {
            public string ManifestsCollectionName { get; set; }
            public string ConnectionString { get; set; }
            public string DatabaseName { get; set; }
        }

        public interface IRepoCatDbSettings
        {
            string ManifestsCollectionName { get; set; }
            string ConnectionString { get; set; }
            string DatabaseName { get; set; }
        }
    }
}

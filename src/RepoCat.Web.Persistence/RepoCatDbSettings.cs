namespace RepoCat.Web.Persistence
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

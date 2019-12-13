namespace RepoCat.Persistence.Service
{
    /// <inheritdoc />
    public class RepoCatDbSettings : IRepoCatDbSettings
    {
        /// <inheritdoc />
        public string RepositoriesCollectionName { get; set; }
        /// <inheritdoc />
        public string ProjectsCollectionName { get; set; }
        /// <inheritdoc />
        public string ConnectionString { get; set; }
        /// <inheritdoc />
        public string DatabaseName { get; set; }
    }
}

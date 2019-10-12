namespace RepoCat.Persistence.Service
{
    /// <inheritdoc />
    public class RepoCatDbSettings : IRepoCatDbSettings
    {
        /// <inheritdoc />
        public string ManifestsCollectionName { get; set; }
        /// <inheritdoc />
        public string ConnectionString { get; set; }
        /// <inheritdoc />
        public string DatabaseName { get; set; }
    }
}

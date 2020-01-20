namespace RepoCat.Persistence.Service
{
    /// <summary>
    /// Interface IRepoCatDbSettings
    /// </summary>
    public interface IRepoCatDbSettings
    {
        /// <summary>
        /// Gets or sets the name of the manifests collection.
        /// </summary>
        /// <value>The name of the manifests collection.</value>
        string ProjectsCollectionName { get; set; }
        
        /// <summary>
        /// Name of the repositories collection
        /// </summary>
        string RepositoriesCollectionName { get; set; }
        
        /// <summary>
        /// Search statistics
        /// </summary>
        string SearchStatisticsCollectionName { get; set; }
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        string ConnectionString { get; set; }
        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        /// <value>The name of the database.</value>
        string DatabaseName { get; set; }
    }
}
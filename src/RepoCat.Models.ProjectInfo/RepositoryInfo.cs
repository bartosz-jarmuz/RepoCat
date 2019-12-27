namespace RepoCat.Transmission.Models
{
    /// <summary>
    /// Info about repository
    /// </summary>
    public class RepositoryInfo
    {
        /// <summary>
        /// Name of the repository
        /// </summary>
        public string RepositoryName { get; set; }

        /// <summary>
        /// Name of the organization owning the repository
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// The mode
        /// </summary>
        public RepositoryMode RepositoryMode { get; set; }
    }
}
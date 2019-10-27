namespace RepoCat.Transmission.Models
{
    /// <summary>
    /// Specifies how the repository data should be collected.
    /// </summary>
    public enum RepositoryMode
    {
        /// <summary>
        /// In default mode, when a project info is transmitted to RepoCat, it will look for this project in the database <br/>
        /// and update the existing entity - or create one if not found
        /// </summary>
        Default,

        /// <summary>
        /// Snapshot mode is for when entire repository (multiple projects &amp; components) is built at the same time, having the same repository stamp.<br/>
        /// In this mode, every time projects are transmitted, they are created as new entities under a given repository stamp<br/>
        /// This way RepoCat can present a current and previous snapshots of a repository, rather than all projects that are currently or used to be in it.<br/>
        /// </summary>
        Snapshot
    }
}
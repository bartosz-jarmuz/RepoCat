namespace RepoCat.Portal.Areas.Catalog.Models
{
    /// <summary>
    /// Class TagBadgeViewModel.
    /// </summary>
    public class TagBadgeViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagBadgeViewModel"/> class.
        /// </summary>
        /// <param name="organizationName">Name of the organization in which repo is</param>
        /// <param name="repositoryName">Name of the repo.</param>
        /// <param name="tagText">The tag text.</param>
        public TagBadgeViewModel(string organizationName, string repositoryName, string tagText)
        {
            this.OrganizationName = organizationName;
            this.RepositoryName = repositoryName;
            this.TagText = tagText;
        }

        /// <summary>
        /// Name of the organization in which repo is
        /// </summary>
        public string OrganizationName { get; }

        /// <summary>
        /// Gets or sets the name of the repo.
        /// </summary>
        /// <value>The name of the repo.</value>
        public string RepositoryName { get; set; }

        /// <summary>
        /// Gets or sets the tag text.
        /// </summary>
        /// <value>The tag text.</value>
        public string TagText { get; set; }
    }
}

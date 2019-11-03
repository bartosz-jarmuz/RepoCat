namespace RepoCat.ProjectFileReaders
{
    public class ProjectItem
    {
        public string ItemType { get; set; }

        public string Include { get; set; }

        public string EvaluatedInclude { get; set; }
        public string CopyToOutputDirectory { get; set; }
    }
}
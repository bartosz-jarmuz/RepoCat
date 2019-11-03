using System.IO;

namespace RepoCat.ProjectFileReaders
{
    public interface IProjectFileFactory
    {
        Project GetProject(FileInfo projectFile);
    }
}
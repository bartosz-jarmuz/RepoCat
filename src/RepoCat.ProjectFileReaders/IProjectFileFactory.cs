using System.IO;
using RepoCat.ProjectFileReaders.ProjectModel;

namespace RepoCat.ProjectFileReaders
{
    /// <summary>
    /// Creates an instance of project object regardless of the framework
    /// </summary>
    public interface IProjectFileFactory
    {

        /// <summary>
        /// Gets an instance
        /// </summary>
        /// <param name="projectFile"></param>
        /// <returns></returns>
        Project GetProject(FileInfo projectFile);
    }
}
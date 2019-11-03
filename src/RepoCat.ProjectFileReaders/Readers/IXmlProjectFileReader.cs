using System.IO;
using System.Xml.Linq;
using RepoCat.ProjectFileReaders.ProjectModel;

namespace RepoCat.ProjectFileReaders.Readers
{
    internal interface IXmlProjectFileReader
    {
        Project ReadFile(FileInfo projectFile, XDocument projectXml);

    }
}
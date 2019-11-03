using System.IO;
using System.Xml.Linq;

namespace RepoCat.ProjectFileReaders.Readers
{
    internal interface IXmlProjectFileReader
    {
        Project ReadFile(FileInfo projectFile, XDocument projectXml);

    }
}
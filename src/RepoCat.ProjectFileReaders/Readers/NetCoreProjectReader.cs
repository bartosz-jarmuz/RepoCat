using System;
using System.IO;
using System.Xml.Linq;

namespace RepoCat.ProjectFileReaders.Readers
{
    internal class NetCoreProjectReader : IXmlProjectFileReader
    {
        public Project ReadFile(FileInfo projectFile, XDocument projectXml)
        {
            throw new NotImplementedException();
        }
    }
}
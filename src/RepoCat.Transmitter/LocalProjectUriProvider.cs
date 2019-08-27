using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RepoCat.Transmitter
{
    public class LocalProjectUriProvider : IProjectUriProvider
    {
        public IEnumerable<string> GetUris(string rootUri)
        {
            var codeDirectory = new DirectoryInfo(rootUri);
            if (codeDirectory.Exists)
            {
                return codeDirectory.EnumerateFiles()
                    .Where(x => x.FullName.EndsWith(".csproj", StringComparison.CurrentCultureIgnoreCase))
                    .Select(x => x.FullName);
            }

            return new string[] { };
        }
    }
}
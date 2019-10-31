using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RepoCat.Transmission.Client.Interface;

namespace RepoCat.Transmission.Client.Implementation
{
    /// <summary>
    /// Class LocalProjectUriProvider.
    /// </summary>
    /// <seealso cref="IProjectUriProvider" />
    public class LocalProjectUriProvider : IProjectUriProvider
    {
        /// <summary>
        /// Finds the URIs of the projects in a local (file system) directory 
        /// </summary>
        /// <param name="rootUri">The root URI.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GetUris(string rootUri)
        {
            var codeDirectory = new DirectoryInfo(rootUri);
            if (codeDirectory.Exists)
            {
                return codeDirectory.EnumerateFiles("*",SearchOption.AllDirectories)
                    .Where(x => x.FullName.EndsWith(".csproj", StringComparison.CurrentCultureIgnoreCase))
                    .Select(x => x.FullName);
            }

            return new string[] { };
        }
    }
}
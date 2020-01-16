using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RepoCat.Transmission.Contracts
{
    /// <summary>
    /// Provides the URIs of all projects available under the specified root URI
    /// </summary>
    public interface IInputUriProvider
    {
        /// <summary>
        /// Provides the URIs of all projects available under the specified root URI
        /// </summary>
        /// <param name="rootUri">The root URI.</param>
        /// <param name="ignoredPathsRegex"></param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        IEnumerable<string> GetUris(string rootUri, Regex ignoredPathsRegex = null);
    }
}
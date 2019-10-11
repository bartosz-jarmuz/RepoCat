using System.Collections.Generic;

namespace RepoCat.Transmitter
{
    /// <summary>
    /// Provides the URIs of all projects available under the specified root URI
    /// </summary>
    public interface IProjectUriProvider
    {
        /// <summary>
        /// Provides the URIs of all projects available under the specified root URI
        /// </summary>
        /// <param name="rootUri">The root URI.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        IEnumerable<string> GetUris(string rootUri);
    }
}
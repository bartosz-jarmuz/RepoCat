using System.Collections.Generic;

namespace RepoCat.Transmitter
{
    public interface IProjectUriProvider
    {
        IEnumerable<string> GetUris(string rootUri);
    }
}
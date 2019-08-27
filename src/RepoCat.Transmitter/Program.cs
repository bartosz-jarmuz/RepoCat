using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RepoCat.Transmitter
{
    class Program
    {
        static void Main(string[] args)
        {
            var codeFolderPath = args[0];

            var uriProvider = new LocalProjectUriProvider();
            var uris = uriProvider.GetUris(codeFolderPath);

            var infoProvider = new ProjectInfoProvider();
            var infos = infoProvider.GetInfos(uris);


            Directory.CreateDirectory(@"C:\test\");
            File.WriteAllLines(@"C:\test\output.txt", args);
        }
    }
}

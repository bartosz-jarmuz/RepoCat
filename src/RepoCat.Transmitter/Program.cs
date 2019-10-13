using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using log4net;
using RepoCat.Transmission.Core;
using RepoCat.Transmission.Core.Implementation;

namespace RepoCat.Transmitter
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = LogManager.GetLogger(typeof(Program));
            var transmissionClient = new TransmissionClient(log);
            transmissionClient.Work(new TransmitterArguments(log, args) ).GetAwaiter().GetResult();
        }
    }
}

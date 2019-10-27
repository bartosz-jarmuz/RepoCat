using System;
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

            try
            {
                log.Info($"Console transmitter [{GetAssemblyFileVersion()}] starting...");
                TransmissionClient client = new TransmissionClient(log);
                client.Work(args).GetAwaiter().GetResult();
                log.Info($"{typeof(Program).Assembly.GetName().Name} - Finished");
            }
            catch (Exception ex)
            {
                log.Fatal("Something went wrong", ex);
            }
        }
        public static string GetAssemblyFileVersion()
        {
            try
            {
                return FileVersionInfo.GetVersionInfo(typeof(Program).Assembly.Location).FileVersion;
            }
            catch
            {
                return "0.0.0.1";
            }
        }
    }
}

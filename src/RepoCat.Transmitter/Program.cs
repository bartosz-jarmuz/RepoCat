using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using log4net;

namespace RepoCat.Transmitter
{
    class Program
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            var worker = new Worker();
            worker.Work(args[0], args[1]).GetAwaiter().GetResult();
            Console.ReadKey();
        }
    }
}

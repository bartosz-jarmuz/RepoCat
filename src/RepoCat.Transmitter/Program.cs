using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace RepoCat.Transmitter
{
    class Program
    {
        static void Main(string[] args)
        {
            var worker = new Worker();
            worker.Work(args[0], args[1]).GetAwaiter().GetResult();
            Console.ReadKey();
            //Directory.CreateDirectory(@"C:\test\");
            //File.WriteAllLines(@"C:\test\output.txt", args);
        }
    }
}

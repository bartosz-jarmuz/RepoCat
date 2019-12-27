using System;

namespace RepoCat.Transmission.Client
{
    public class ConsoleLogger : ILogger
    {
        public void Debug(string message)
        {
            Console.WriteLine($"Debug - {message}");
        }

        public void Info(string message)
        {
            Console.WriteLine($"DEBUG - {message}");

        }

        public void Error(string message, Exception exception)
        {
            Console.WriteLine($"Error - {message}");

        }

        public void Error(string message)
        {
            Console.WriteLine($"Error - {message}");

        }

        public void Warn(string message)
        {
            Console.WriteLine($"Warn - {message}");

        }

        public void Fatal(Exception exception)
        {
            Console.WriteLine($"Fatal - {exception}");

        }

        public void Fatal(string message, Exception exception)
        {
            Console.WriteLine($"Fatal - {message}");

        }
    }
}

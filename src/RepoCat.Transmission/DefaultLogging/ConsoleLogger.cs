using System;

namespace RepoCat.Transmission
{
    public class ConsoleLogger : LoggerDecorator
    {
        public ConsoleLogger(ILogger logger) : base(logger)
        {
        }

        public override void Debug(string message)
        {
            base.Debug(message);
            Console.WriteLine("Debug - " + message);
        }

        public override void Info(string message)
        {
            base.Info(message);
            Console.WriteLine("Info - " + message);
        }

        public override void Error(string message, Exception exception)
        {
            base.Error(message, exception);
            Console.WriteLine("Error - " + message + exception);
        }

        public override void Error(string message)
        {
            base.Error(message);
            Console.WriteLine("Error - " + message);
        }

        public override void Warn(string message)
        {
            base.Warn(message);
            Console.WriteLine("Warn - " + message);
        }

        public override void Fatal(Exception exception)
        {
            base.Fatal(exception);
            Console.WriteLine("Fatal - " + exception);
        }

        public override void Fatal(string message, Exception exception)
        {
            base.Fatal(message, exception);
            Console.WriteLine("Fatal - " + message + exception);
        }

       
    }
}
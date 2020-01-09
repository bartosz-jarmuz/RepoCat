using System;
using System.Diagnostics;

namespace RepoCat.Transmission
{
    public class TraceLogger : ILogger
    {
        private readonly LogLevel logLevel;

        public TraceLogger(LogLevel logLevel)
        {
            this.logLevel = logLevel;
        }

        public void Debug(string message)
        {
            if (this.logLevel <= LogLevel.Debug)
            {
                Trace.WriteLine($"Debug - {message}");
            }
        }

        public void Info(string message)
        {
            if (this.logLevel <= LogLevel.Info)
            {
                Trace.WriteLine($"Info - {message}");
            }
        }
        public void Warn(string message)
        {
            if (this.logLevel <= LogLevel.Warn)
            {
                Trace.WriteLine($"Warn - {message}");
            }
        }

        public void Error(string message, Exception exception)
        {
            if (this.logLevel <= LogLevel.Error)
            {
                Trace.WriteLine($"Error - {message}");
            }
        }

        public void Error(string message)
        {
            if (this.logLevel <= LogLevel.Error)
            {
                Trace.WriteLine($"Error - {message}");
            }
        }

        public void Fatal(Exception exception)
        {
            if (this.logLevel <= LogLevel.Fatal)
            {
                Trace.WriteLine($"Fatal - {exception}");
            }
        }

        public void Fatal(string message, Exception exception)
        {
            if (this.logLevel <= LogLevel.Fatal)
            {
                Trace.WriteLine($"Fatal - {message}");
            }
        }
    }
}
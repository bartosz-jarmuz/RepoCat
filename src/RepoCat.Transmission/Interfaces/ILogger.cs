using System;

namespace RepoCat.Transmission
{
    public interface ILogger
    {
        void Debug(string message);
        void Info(string message);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Standard naming")]
        void Error(string message, Exception exception);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Standard naming")]
        void Error(string message);
        void Warn(string message);
        void Fatal(Exception exception);
        void Fatal(string message, Exception exception);
    }
}

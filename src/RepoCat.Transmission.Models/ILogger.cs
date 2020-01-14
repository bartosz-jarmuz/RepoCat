using System;

// ReSharper disable once CheckNamespace
namespace RepoCat.Transmission
{  
    /// <summary>
    /// Interface for the transmitter logger
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Debug(string message);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Info(string message);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Standard naming")]
        void Error(string message, Exception exception);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Standard naming")]
        void Error(string message);

        ///
        void Warn(string message);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        void Fatal(Exception exception);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Fatal(string message, Exception exception);
    }
}
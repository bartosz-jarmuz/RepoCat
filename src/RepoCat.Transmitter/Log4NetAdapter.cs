// -----------------------------------------------------------------------
//  <copyright file="Log4NetAdapter.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using log4net;
using RepoCat.Transmission.Client;

namespace RepoCat.Transmission.ConsoleClient
{
    /// <summary>
    /// Logger
    /// </summary>
    public class Log4NetAdapter : ILogger
    {
        private readonly ILog log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        public Log4NetAdapter(ILog log)
        {
            this.log = log;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Debug(string message)
        {
            this.log.Debug(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message)
        {
            this.log.Info(message);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Error(string message, Exception exception)
        {
            this.log.Error(message, exception);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
            this.log.Error(message);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Warn(string message)
        {
            this.log.Warn(message);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        public void Fatal(Exception exception)
        {
            this.log.Fatal(exception);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Fatal(string message, Exception exception)
        {
            this.log.Fatal(message, exception);

        }
    }
}
// -----------------------------------------------------------------------
//  <copyright file="LoggerDecorator.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;

namespace RepoCat.Transmission
{
    public abstract class LoggerDecorator : ILogger
    {
        private readonly ILogger logger;

        protected LoggerDecorator(ILogger logger)
        {
            this.logger = logger;
        }

        public virtual void Debug(string message)
        {
            this.logger.Debug(message);
        }

        public virtual void Info(string message)
        {
            this.logger.Info(message);

        }

#pragma warning disable CA1716 // Identifiers should not match keywords
        public virtual void Error(string message, Exception exception)
        {
            this.logger.Error(message, exception);

        }

        public virtual void Error(string message)
        {
            this.logger.Error(message);

        }
#pragma warning restore CA1716 // Identifiers should not match keywords

        public virtual void Warn(string message)
        {
            this.logger.Warn(message);

        }

        public virtual void Fatal(Exception exception)
        {
            this.logger.Fatal(exception);

        }

        public virtual void Fatal(string message, Exception exception)
        {
            this.logger.Fatal(message, exception);
        }
    }
}
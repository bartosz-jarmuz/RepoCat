// -----------------------------------------------------------------------
//  <copyright file="AppInsightsLogger.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using RepoCat.Transmission;

namespace RepoCat.Telemetry
{
    public class AppInsightsLogger : ILogger
    {
        private readonly TelemetryClient telemetryClient;

        public AppInsightsLogger(TelemetryClient telemetryClient)
        {
            this.telemetryClient = telemetryClient;
        }

        public void Debug(string message)
        {
            this.telemetryClient.TrackTrace(message, SeverityLevel.Verbose, new Dictionary<string, string>()
            {
                { PropertyKeys.Verbosity, nameof(this.Debug) },
                { PropertyKeys.Origin, nameof(AppInsightsLogger) }
            });
        }

        public void Info(string message)
        {
            this.telemetryClient.TrackTrace(message, SeverityLevel.Information, new Dictionary<string, string>()
            {
                { PropertyKeys.Verbosity, nameof(this.Info) }
                ,{ PropertyKeys.Origin, nameof(AppInsightsLogger) }
                
            });
        }

        public void Error(string message, Exception exception)
        {
            this.telemetryClient.TrackTrace(message, SeverityLevel.Error, new Dictionary<string, string>()
            {
                { PropertyKeys.Verbosity, nameof(this.Error) },
                { PropertyKeys.Exception, exception.ToString() },
                { PropertyKeys.Origin, nameof(AppInsightsLogger) }
            });
        }

        public void Error(string message)
        {
            this.telemetryClient.TrackTrace(message, SeverityLevel.Error, new Dictionary<string, string>()
            {
                { PropertyKeys.Verbosity, nameof(this.Error) },
                { PropertyKeys.Origin, nameof(AppInsightsLogger) }
            });
        }

        public void Warn(string message)
        {
            this.telemetryClient.TrackTrace(message, SeverityLevel.Warning, new Dictionary<string, string>()
            {
                { PropertyKeys.Verbosity, nameof(this.Warn) }
               , { PropertyKeys.Origin, nameof(AppInsightsLogger) }
            });
        }

        public void Fatal(Exception exception)
        {
            this.telemetryClient.TrackTrace("FATAL EXCEPTION", SeverityLevel.Critical, new Dictionary<string, string>()
            {
                { PropertyKeys.Exception, exception.ToString() },
                { PropertyKeys.Origin, nameof(AppInsightsLogger) }
            });
        }

        public void Fatal(string message, Exception exception)
        {
            this.telemetryClient.TrackTrace(message, SeverityLevel.Critical, new Dictionary<string, string>()
            {
                { PropertyKeys.Verbosity, nameof(this.Fatal) },
                { PropertyKeys.Exception, exception.ToString() },
                { PropertyKeys.Origin, nameof(AppInsightsLogger) }
            });
        }
    }
}
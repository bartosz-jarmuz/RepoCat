// -----------------------------------------------------------------------
//  <copyright file="MockTelemetryChannel.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Repocat.Persistence.Tests
{
    public class TelemetryMock : ITelemetryChannel
    {
        public ConcurrentBag<ITelemetry> SentTelemtries = new ConcurrentBag<ITelemetry>();
        public bool IsFlushed { get; private set; }
        public bool? DeveloperMode { get; set; }
        public string EndpointAddress { get; set; }

        public void Send(ITelemetry item)
        {
            this.SentTelemtries.Add(item);
        }

        public void Flush()
        {
            this.IsFlushed = true;
        }

        public void Dispose()
        {

        }

        public static TelemetryClient InitializeMockTelemetryClient()
        {
            // Application Insights TelemetryClient doesn't have an interface (and is sealed)
            // Spin -up our own homebrew mock object
            TelemetryMock telemetryMock = new TelemetryMock();
            TelemetryConfiguration mockTelemetryConfig = new TelemetryConfiguration
            {
                TelemetryChannel = telemetryMock,
                InstrumentationKey = Guid.NewGuid().ToString(),
            };

            TelemetryClient mockTelemetryClient = new TelemetryClient(mockTelemetryConfig);
            return mockTelemetryClient;
        }
    }
}
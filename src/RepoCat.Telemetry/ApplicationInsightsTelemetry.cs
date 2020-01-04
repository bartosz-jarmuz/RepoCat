using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;

namespace RepoCat.Telemetry
{
    //public class ApplicationInsightsTelemetry : IRepoCatTelemetry
    //{
    //    private readonly TelemetryClient client;

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="client"></param>
    //    public ApplicationInsightsTelemetry(TelemetryClient client)
    //    {
    //        this.client = client;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="eventName"></param>
    //    /// <param name="properties"></param>
    //    /// <param name="metrics"></param>
    //    public void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
    //    {
    //        this.client.TrackEvent(eventName, properties, metrics);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="exception"></param>
    //    public void TrackException(Exception exception)
    //    {
    //        this.client.TrackException(exception);
    //    }
    //}
}
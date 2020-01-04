// -----------------------------------------------------------------------
//  <copyright file="HealthcheckController.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;
using RepoCat.Telemetry;

namespace RepoCat.Portal.Controllers.api
{
    /// <summary>
    /// Handles healthcheck requests
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/healthcheck")]
    [ApiController]
    public class HealthcheckController : Controller
    {
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Handles healthcheck requests
        /// </summary>
        /// <param name="telemetryClient"></param>
        public HealthcheckController(TelemetryClient telemetryClient)
        {
            this.telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Is it alive?
        /// </summary>
        /// <returns></returns>
        [Route("Ping")]
        [HttpGet]
        public ActionResult Ping()
        {
            return this.Ok("Pong");
        }

        /// <summary>
        /// Is it alive?
        /// </summary>
        /// <returns></returns>
        [Route("ErrorNotification")]
        [HttpPost]
        public ActionResult ErrorNotification(Exception exception)
        {
            this.telemetryClient.TrackException(exception);
            return this.Accepted();
        }

    }
}
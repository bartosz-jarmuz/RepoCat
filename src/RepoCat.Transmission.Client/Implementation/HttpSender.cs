using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;
using RepoCat.Transmission.Core.Interface;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Core.Implementation
{
    /// <summary>
    /// Class that sends the project manifests to the RepoCat API over HTTP
    /// </summary>
    public class HttpSender : ISender
    {
        /// <summary>
        /// The client
        /// </summary>
        private readonly HttpClient client;

        private readonly ILog log;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpSender"/> class.
        /// </summary>
        /// <param name="baseAddress">The base address.</param>
        /// <param name="log"></param>
        public HttpSender(Uri baseAddress, ILog log)
        {
            this.log = log;
            this.client = new HttpClient()
            {
                BaseAddress = baseAddress
            };
        }

        /// <summary>
        /// Sends the specified infos.
        /// </summary>
        /// <param name="infos">The infos.</param>
        /// <returns>Task.</returns>
        public async Task Send(IEnumerable<ProjectInfo> infos)
        {
            var tasks = new List<Task>();

            var infoCounter = 0;
            foreach (ProjectInfo projectInfo in infos)
            {
                infoCounter++;
                tasks.Add(this.Send(projectInfo));
            }
            this.log.Info($"Waiting for all {infoCounter} project infos to be sent.");

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Sends the specified information.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>Task.</returns>
        public async Task Send(ProjectInfo info)
        {
            this.log.Debug($"Sending {info.ProjectName} project info");

            string serialized;
            try
            {
                serialized = JsonConvert.SerializeObject(info);
            }   
            catch (Exception ex)
            {
                this.log.Error($"Error while serializing project info: {info.ProjectName}", ex);
                return;
            }

            try
            {
                StringContent content = new StringContent(serialized, Encoding.UTF8, "application/json");
                HttpResponseMessage result = await this.client.PostAsync("api/manifest", content);
                if (result.IsSuccessStatusCode)
                {
                    string response = await result.Content.ReadAsStringAsync();
                    this.log.Info($"Sent [{info.ProjectName}]. StatusCode: [{result.StatusCode}]. Response [{response}]. Location: [{result.Headers?.Location}]");
                }
                else
                {
                    this.log.Error($"Error - [{result.StatusCode}] - [{result.ReasonPhrase}] - while sending [{info.ProjectName}].");
                }
            }
            catch (Exception ex)
            {
                this.log.Error($"Error while sending project info: {info.ProjectName}. {serialized}", ex);
            }

        }
    }
}
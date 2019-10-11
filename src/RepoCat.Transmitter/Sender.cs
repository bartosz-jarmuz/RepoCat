using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RepoCat.Models.ProjectInfo;

namespace RepoCat.Transmitter
{
    /// <summary>
    /// Class that sends the project manifests to the RepoCat API over HTTP
    /// </summary>
    public class Sender
    {
        /// <summary>
        /// The client
        /// </summary>
        private readonly HttpClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sender"/> class.
        /// </summary>
        /// <param name="baseAddress">The base address.</param>
        public Sender(Uri baseAddress)
        {
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
            Program.Log.Info($"Waiting for all {infoCounter} project infos to be sent.");

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Sends the specified information.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>Task.</returns>
        public async Task Send(ProjectInfo info)
        {
            Program.Log.Debug($"Sending {info.ProjectName} project info");

            string serialized;
            try
            {
                serialized = JsonConvert.SerializeObject(info);
            }   
            catch (Exception ex)
            {
                Program.Log.Error($"Error while serializing project info: {info.ProjectName}", ex);
                return;
            }

            try
            {
                StringContent content = new StringContent(serialized, Encoding.UTF8, "application/json");
                HttpResponseMessage result = await this.client.PostAsync("api/manifest", content);
                if (result.IsSuccessStatusCode)
                {
                    Program.Log.Info($"Sent {info.ProjectName} project info OK.");
                }
                else
                {
                    Program.Log.Error($"Error - {result.StatusCode} - {result.ReasonPhrase} - while sending {info.ProjectName}.");
                }
            }
            catch (Exception ex)
            {
                Program.Log.Error($"Error while sending project info: {info.ProjectName}. {serialized}", ex);
            }

        }
    }
}
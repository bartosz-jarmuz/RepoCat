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
    public class Sender
    {
        private readonly HttpClient client;

        public Sender(Uri baseAddress)
        {
            this.client = new HttpClient()
            {
                BaseAddress = baseAddress
            };
        }

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

        public async Task Send(ProjectInfo info)
        {
            Program.Log.Debug($"Sending {info.GetName()} project info");

            string serialized;
            try
            {
                serialized = JsonConvert.SerializeObject(info);
            }   
            catch (Exception ex)
            {
                Program.Log.Error($"Error while serializing project info: {info.GetName()}", ex);
                return;
            }

            try
            {
                var content = new StringContent(serialized, Encoding.UTF8, "application/json");
                await this.client.PostAsync("api/manifest", content);
                Program.Log.Info($"Sent {info.GetName()} project info OK.");
            }
            catch (Exception ex)
            {
                Program.Log.Error($"Error while sending project info: {info.GetName()}. {serialized}", ex);
            }

        }
    }
}
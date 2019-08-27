using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RepoCat.Transmitter
{
    public class Sender
    {
        private readonly HttpClient client;

        public Sender(Uri baseAddress)
        {
            //this.client = new HttpClient()
            //{
            //    BaseAddress = baseAddress
            //};
        }

        public async Task Send(ProjectInfo info)
        {
            Directory.CreateDirectory(@"C:\test");
            string serialized;
            try
            {
                serialized = JsonConvert.SerializeObject(info);
                File.WriteAllText(Path.Combine(@"C:\test", info.AssemblyName), serialized);
            }   
            catch
            {
                return;
                //todo
            }

            try
            {
                await Task.Delay(1);
                //var content = new StringContent(serialized);
                //await this.client.PostAsync("api/manifest", content);
            }
            catch
            {
                //todo
            }
        }
    }
}
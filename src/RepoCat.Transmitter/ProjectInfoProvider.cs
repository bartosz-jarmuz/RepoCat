using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using Microsoft.Build.Evaluation;
using RepoCat.Transmitter.Models;

namespace RepoCat.Transmitter
{
    class ProjectInfoProvider : IProjectInfoProvider
    {
        public IEnumerable<ProjectInfo> GetInfos(IEnumerable<string> uris, string repo, string repoStamp)
        {
            var counter = 0;
            foreach (string uri in uris)
            {
                Program.Log.Debug($"Checking project #{counter} for manifest file. {uri}");

                counter++;
                var info = this.GetInfo(uri,repo, repoStamp);
                if (info != null)
                {
                    yield return info;
                }
                Program.Log.Debug($"Project #{counter} does not contain a manifest file. {uri}");

            }
            Program.Log.Info($"Loaded project infos for {counter} projects.");

        }


        public ProjectInfo GetInfo(string uri, string repo, string repoStamp)
        {
            try
            {
                Project prj = new Project(uri);
                var manifestInclude = prj.Items.FirstOrDefault(x =>
                    x.EvaluatedInclude.EndsWith("RepoCat.xml", StringComparison.CurrentCultureIgnoreCase));
                if (manifestInclude != null)
                {
                     Program.Log.Debug($"Reading manifest info from {uri}");

                    var info = new ProjectInfo()
                    {
                        AssemblyName = prj.Properties.FirstOrDefault(x => x.Name.Equals("AssemblyName", StringComparison.CurrentCultureIgnoreCase))?.EvaluatedValue,
                        ProjectUri = prj.FullPath,
                        ProjectName = Path.GetFileNameWithoutExtension(prj.FullPath),
                        RepositoryName = repo,
                        RepositoryStamp = repoStamp,
                    };

                    string manifestPath = Directory.GetFiles(prj.DirectoryPath, manifestInclude.EvaluatedInclude, SearchOption.AllDirectories).FirstOrDefault();
                    if (string.IsNullOrEmpty(manifestPath))
                    {
                        Program.Log.Warn($"Manifest not found for project {uri}!");
                    }
                    else
                    {
                        string manifestContent = File.ReadAllText(manifestPath);
                        info.Components = ManifestDeserializer.LoadComponents(manifestContent);
                        Program.Log.Info($"Read OK from {uri}");
                    }
                 

                    return info;
                }
            }
            catch (Exception ex)
            {
                Program.Log.Warn("Error while loading project info " +  ex.Message);
            }
            return null;
        }

      
    }
}
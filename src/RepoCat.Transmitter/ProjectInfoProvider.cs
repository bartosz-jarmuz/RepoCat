using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using Microsoft.Build.Evaluation;
using RepoCat.Models.ProjectInfo;

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
                        TargetExt = prj.Properties.FirstOrDefault(x => x.Name.Equals("TargetExt", StringComparison.CurrentCultureIgnoreCase))?.EvaluatedValue,
                        OutputType = prj.Properties.FirstOrDefault(x => x.Name.Equals("OutputType", StringComparison.CurrentCultureIgnoreCase))?.EvaluatedValue,
                        ProjectPath = prj.FullPath,
                        ProjectName = Path.GetFileNameWithoutExtension(prj.FullPath),
                        Repo = repo,
                        RepoStamp = repoStamp,
                        RepoCatManifestPath = Directory.GetFiles(prj.DirectoryPath, manifestInclude.EvaluatedInclude, SearchOption.AllDirectories).FirstOrDefault(),
                    };

                    info.RepoCatManifest = File.ReadAllText(info.RepoCatManifestPath);

                    Program.Log.Info($"Read OK from {uri}");

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
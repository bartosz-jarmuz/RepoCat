using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using Microsoft.Build.Evaluation;

namespace RepoCat.Transmitter
{
    class ProjectInfoProvider : IProjectInfoProvider
    {
        public IEnumerable<ProjectInfo> GetInfos(IEnumerable<string> uris)
        {
            foreach (string uri in uris)
            {
                var info = this.GetInfo(uri);
                if (info != null)
                {
                    yield return info;
                }
            }
        }


        public ProjectInfo GetInfo(string uri)
        {
            try
            {
                Project prj = new Project(uri);
                var manifestInclude = prj.Items.FirstOrDefault(x =>
                    x.EvaluatedInclude.EndsWith("RepoCat.xml", StringComparison.CurrentCultureIgnoreCase));
                if (manifestInclude != null)
                {
                     Program.Log.Debug($"Reading info from {uri}");

                    var info = new ProjectInfo()
                    {
                        AssemblyName = prj.Properties.FirstOrDefault(x => x.Name.Equals("AssemblyName", StringComparison.CurrentCultureIgnoreCase))?.EvaluatedValue,
                        TargetExt = prj.Properties.FirstOrDefault(x => x.Name.Equals("TargetExt", StringComparison.CurrentCultureIgnoreCase))?.EvaluatedValue,
                        OutputType = prj.Properties.FirstOrDefault(x => x.Name.Equals("OutputType", StringComparison.CurrentCultureIgnoreCase))?.EvaluatedValue,
                        ProjectPath = prj.FullPath
                    };

                    info.RepoCatManifestPath = Directory.GetFiles(prj.DirectoryPath, manifestInclude.EvaluatedInclude, SearchOption.AllDirectories).FirstOrDefault();

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
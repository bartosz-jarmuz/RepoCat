using System;
using System.Collections.Generic;
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
                yield return this.GetInfo(uri);
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
                    var info = new ProjectInfo()
                    {
                        AssemblyName = prj.Properties.FirstOrDefault(x => x.Name.Equals("AssemblyName", StringComparison.CurrentCultureIgnoreCase))?.EvaluatedValue,
                        TargetExt = prj.Properties.FirstOrDefault(x => x.Name.Equals("TargetExt", StringComparison.CurrentCultureIgnoreCase))?.EvaluatedValue,
                        OutputType = prj.Properties.FirstOrDefault(x => x.Name.Equals("OutputType", StringComparison.CurrentCultureIgnoreCase))?.EvaluatedValue,
                        RepoCatManifestPath = manifestInclude.EvaluatedInclude,
                        ProjectPath = prj.FullPath
                    };
                    return info;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;
using RepoCat.Transmission.Client.Interface;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Client.Implementation
{
    public class ProjectInfoProvider : IProjectInfoProvider
    {
        private readonly ILog log;

        public ProjectInfoProvider(ILog log)
        {
            this.log = log;
            if (MSBuildLocator.CanRegister)
            {
                MSBuildLocator.RegisterDefaults();
            }
        }

        public IEnumerable<ProjectInfo> GetInfos(IEnumerable<string> uris, string repo, string repoStamp)
        {
            var counter = 0;
            foreach (string uri in uris)
            {
                this.log.Debug($"Checking project #{counter} for manifest file. {uri}");

                counter++;
                var info = this.GetInfo(uri, repo, repoStamp);
                if (info != null)
                {
                    yield return info;
                }
                this.log.Debug($"Project #{counter} does not contain a manifest file. {uri}");

            }
            this.log.Info($"Loaded project infos for {counter} projects.");

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
                     this.log.Debug($"Reading manifest info from {uri}");

                    var info = new ProjectInfo()
                    {
                        AssemblyName = prj.Properties.FirstOrDefault(x => x.Name.Equals("AssemblyName", StringComparison.CurrentCultureIgnoreCase))?.EvaluatedValue,
                        ProjectUri = prj.FullPath,
                        ProjectName = Path.GetFileNameWithoutExtension(prj.FullPath),
                        RepositoryName = repo,
                        RepositoryStamp = repoStamp,
                        OutputType = prj.Properties.FirstOrDefault(x => x.Name.Equals("OutputType", StringComparison.CurrentCultureIgnoreCase))?.EvaluatedValue,
                        TargetExtension = prj.Properties.FirstOrDefault(x => x.Name.Equals("TargetExt", StringComparison.CurrentCultureIgnoreCase))?.EvaluatedValue,
                    };

                    string manifestPath = Directory.GetFiles(prj.DirectoryPath, manifestInclude.EvaluatedInclude, SearchOption.AllDirectories).FirstOrDefault();
                    if (string.IsNullOrEmpty(manifestPath))
                    {
                        this.log.Warn($"Manifest not found for project {uri}!");
                    }
                    else
                    {
                        string manifestContent = File.ReadAllText(manifestPath);
                        info.Components = ManifestDeserializer.DeserializeComponents(manifestContent);
                        this.log.Info($"Read OK from {uri}");
                    }
                 

                    return info;
                }
            }
            catch (Exception ex)
            {
                this.log.Warn("Error while loading project info " +  ex.Message);
            }
            return null;
        }

      
    }
}
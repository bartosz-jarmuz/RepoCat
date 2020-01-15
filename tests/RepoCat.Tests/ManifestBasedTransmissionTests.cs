using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RepoCat.Portal;
using RepoCat.Portal.Mapping;
using RepoCat.Transmission;
using RepoCat.Transmission.Models;

namespace RepoCat.Tests
{
    [TestFixture]
    public class ManifestBasedTransmissionTests
    {
        private static DirectoryInfo RepoRoot => new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "SampleScriptsRepository"));
       
        [Test]
        public void ManifestFilesPaths_ProvidedOk()
        {
            ManifestBasedUriProvider uriProvider = new ManifestBasedUriProvider(new TraceLogger(LogLevel.Debug));
            List<string> uris = uriProvider.GetUris(RepoRoot.FullName).ToList();
            Assert.AreEqual(2, uris.Count);
            Assert.IsTrue(uris.Any(x => x.Contains("ScriptOneManifest.RepoCat.xml", StringComparison.OrdinalIgnoreCase)));
            Assert.IsTrue(uris.Any(x => x.Contains("ScriptTwoManifest.RepoCat.xml", StringComparison.OrdinalIgnoreCase)));
        }
      
        
        [Test]
        public void ProjectInfo_RepoStampsShouldBeEqual()
        {
            //arrange
            ManifestBasedUriProvider uriProvider = new ManifestBasedUriProvider(new TraceLogger(LogLevel.Debug));
            List<string> uris = uriProvider.GetUris(RepoRoot.FullName).ToList();
            IProjectInfoBuilder builder = ProjectInfoBuilderFactory.Get(new TransmitterArguments() { TransmissionMode = TransmissionMode.LocalManifestBased }, new TraceLogger(LogLevel.Debug));

            //act
            List<ProjectInfo> infos = builder.GetInfos(uris).ToList();

            //assert

            //the stamp of both scripts should be the same (as they are loaded at the same time)
            //otherwise, they would be visible in different snapshots of the repo
            Assert.AreEqual(infos[0].RepositoryStamp, infos[1].RepositoryStamp);
        }


        [Test]
        public void ProjectInfo_ProvidedOk()
        {
            //arrange
            ManifestBasedUriProvider uriProvider = new ManifestBasedUriProvider(new TraceLogger(LogLevel.Debug));
            List<string> uris = uriProvider.GetUris(RepoRoot.FullName).ToList();
            IProjectInfoBuilder builder = ProjectInfoBuilderFactory.Get(new TransmitterArguments() { TransmissionMode = TransmissionMode.LocalManifestBased }, new TraceLogger(LogLevel.Debug));

            //act
            List<ProjectInfo> infos = builder.GetInfos(uris).ToList();

            //assert
            Assert.AreEqual(2, infos.Count);
            var scriptOne = infos.Single(x => x.ProjectName == "ScriptOne");
            var scriptTwo = infos.Single(x => x.ProjectName == "ScriptTwo");
            //most properties are the same. Also, we care simply about whether the files exist
            ValidateProjectInfo(scriptOne);
            ValidateProjectInfo(scriptTwo);
        }

        [Test]
        public void RelativePathPropertyKey_ResolvedOk()
        {
            //arrange
            ManifestBasedUriProvider uriProvider = new ManifestBasedUriProvider(new TraceLogger(LogLevel.Debug));
            List<string> uris = uriProvider.GetUris(RepoRoot.FullName).ToList();
            IProjectInfoBuilder builder = ProjectInfoBuilderFactory.Get(new TransmitterArguments() { TransmissionMode = TransmissionMode.LocalManifestBased }, new TraceLogger(LogLevel.Debug));

            //act
            List<ProjectInfo> infos = builder.GetInfos(uris).ToList();

            //assert
            var scriptTwo = infos.Single(x => x.ProjectName == "ScriptTwo");
            var samplePath = scriptTwo.Components.Single().Properties["SamplePath"];
            FileAssert.Exists(samplePath);
        }


        private static void ValidateProjectInfo(ProjectInfo scriptOne)
        {
            Assert.AreEqual("RepoCat Scripts", scriptOne.RepositoryInfo.RepositoryName);
            Assert.AreEqual("RepoCat Organization", scriptOne.RepositoryInfo.OrganizationName);
            DirectoryAssert.Exists(scriptOne.ProjectUri);
            FileAssert.Exists(scriptOne.DownloadLocation);
            Assert.IsFalse(string.IsNullOrEmpty(scriptOne.RepositoryStamp));
            
            ComponentManifest feature = scriptOne.Components.Single();
            FileAssert.Exists(feature.DocumentationUri);
            Assert.IsFalse(string.IsNullOrEmpty(feature.Description));
            Assert.IsTrue(feature.Tags.Any());
            Assert.IsTrue(feature.Properties.Any());
        }
    }
}
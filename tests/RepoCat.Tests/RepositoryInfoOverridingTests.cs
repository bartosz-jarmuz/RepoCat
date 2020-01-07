// -----------------------------------------------------------------------
//  <copyright file="RepositoryInfoOverridingTests.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RepoCat.Transmission.Client;
using RepoCat.Transmission.Models;

namespace RepoCat.Tests
{
    [TestFixture]
    public class RepositoryInfoOverridingTests
    {
        private static DirectoryInfo RepoRoot => new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "SampleScriptsRepository"));

        private ProjectInfo LoadFromManifestWithRepoIncluded(bool allowOverride, RepositoryInfo repositoryInfo)
        {
            //arrange
            ManifestBasedUriProvider uriProvider = new ManifestBasedUriProvider();
            List<string> uris = uriProvider.GetUris(RepoRoot.FullName).ToList();
            IProjectInfoProvider provider = ProjectInfoProviderFactory.Get(new TransmitterArguments()
            {
                RepositoryName = repositoryInfo?.RepositoryName,
                OrganizationName = repositoryInfo?.OrganizationName,
                RepositoryMode = repositoryInfo?.RepositoryMode??RepositoryMode.Default,
                TransmissionMode = TransmissionMode.LocalManifestBased, 
                ManifestCanOverrideRepositoryInfo = allowOverride
            }, new TraceLogger(LogLevel.Debug));

            //act
            List<ProjectInfo> infos = provider.GetInfos(uris).ToList();

            //assert
            var scriptOne = infos.Single(x => x.ProjectName == "ScriptOne");
            return scriptOne;
        }


        [Test]
        public void OverrideFalse_InfoInManifest_NoInfoInArgs_ShoudBeAsInManifest()
        {
            //arrange and act
            var scriptOne = this.LoadFromManifestWithRepoIncluded(false, null);
            //assert
            Assert.AreEqual("RepoCat Scripts", scriptOne.RepositoryInfo.RepositoryName);
            Assert.AreEqual("RepoCat Organization", scriptOne.RepositoryInfo.OrganizationName);
        }

        [Test]
        public void OverrideFalse_InfoInManifest_InfoInArgs_ShoudBeAsInArgs()
        {
            //arrange and act
            var info = new RepositoryInfo()
            {
                RepositoryName = "TestRepo",
                OrganizationName = "TestOrg"
            };
            var scriptOne = this.LoadFromManifestWithRepoIncluded(false, info);

            //assert
            Assert.AreEqual("TestRepo", scriptOne.RepositoryInfo.RepositoryName);
            Assert.AreEqual("TestOrg", scriptOne.RepositoryInfo.OrganizationName);
        }

        [Test]
        public void OverrideTrue_InfoInManifest_NoInfoInArgs_ShoudBeAsInManifest()
        {
            //arrange and act
            var scriptOne = this.LoadFromManifestWithRepoIncluded(true, null);

            //assert
            Assert.AreEqual("RepoCat Scripts", scriptOne.RepositoryInfo.RepositoryName);
            Assert.AreEqual("RepoCat Organization", scriptOne.RepositoryInfo.OrganizationName);
        }

        [Test]
        public void OverrideTrue_InfoInManifest_InfoInArgs_ShoudBeAsInManifest()
        {
            //arrange and act
            var info = new RepositoryInfo()
            {
                RepositoryName = "TestRepo",
                OrganizationName = "TestOrg"
            };
            var scriptOne = this.LoadFromManifestWithRepoIncluded(true, info);

            //assert
            Assert.AreEqual("RepoCat Scripts", scriptOne.RepositoryInfo.RepositoryName);
            Assert.AreEqual("RepoCat Organization", scriptOne.RepositoryInfo.OrganizationName);
        }

    }
}
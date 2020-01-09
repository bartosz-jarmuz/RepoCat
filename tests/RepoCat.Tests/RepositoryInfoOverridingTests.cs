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
        private static DirectoryInfo Samples => new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "SampleManifestFiles"));

        private ProjectInfo LoadFromManifestWithoutRepositoryInfo(bool allowOverride, RepositoryInfo repositoryInfoFromTransmitter)
        {
            //arrange
            var file = Samples.GetFiles("ProjectManifestWithoutRepository.RepoCat.xml").Single();

            IProjectInfoBuilder builder = ProjectInfoBuilderFactory.Get(new TransmitterArguments()
            {
                RepositoryName = repositoryInfoFromTransmitter?.RepositoryName,
                OrganizationName = repositoryInfoFromTransmitter?.OrganizationName,
                RepositoryMode = repositoryInfoFromTransmitter?.RepositoryMode ?? RepositoryMode.Default,
                TransmissionMode = TransmissionMode.LocalManifestBased,
                ManifestCanOverrideRepositoryInfo = allowOverride
            }, new TraceLogger(LogLevel.Debug));

            //act
            List<ProjectInfo> infos = builder.GetInfos(new []{file.FullName}).ToList();

            //assert
            var scriptOne = infos.Single();
            return scriptOne;
        }

        private ProjectInfo LoadFromManifestWithRepoIncluded(bool allowOverride, RepositoryInfo repositoryInfoFromTransmitter)
        {
            //arrange
            ManifestBasedUriProvider uriProvider = new ManifestBasedUriProvider();
            List<string> uris = uriProvider.GetUris(RepoRoot.FullName).ToList();
            IProjectInfoBuilder builder = ProjectInfoBuilderFactory.Get(new TransmitterArguments()
            {
                RepositoryName = repositoryInfoFromTransmitter?.RepositoryName,
                OrganizationName = repositoryInfoFromTransmitter?.OrganizationName,
                RepositoryMode = repositoryInfoFromTransmitter?.RepositoryMode??RepositoryMode.Default,
                TransmissionMode = TransmissionMode.LocalManifestBased, 
                ManifestCanOverrideRepositoryInfo = allowOverride
            }, new TraceLogger(LogLevel.Debug));

            //act
            List<ProjectInfo> infos = builder.GetInfos(uris).ToList();

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



        [Test]
        public void OverrideFalse_WithoutInfoInManifest_NoInfoInArgs_ShoudBeAsInManifest()
        {
            //arrange and act
            var scriptOne = this.LoadFromManifestWithoutRepositoryInfo(false, null);
            //assert
            //assert
            Assert.AreEqual("Unspecified", scriptOne.RepositoryInfo.RepositoryName);
            Assert.AreEqual("Unspecified", scriptOne.RepositoryInfo.OrganizationName);
        }

        [Test]
        public void OverrideFalse_WithoutInfoInManifest_InfoInArgs_ShoudBeAsInArgs()
        {
            //arrange and act
            var info = new RepositoryInfo()
            {
                RepositoryName = "TestRepo",
                OrganizationName = "TestOrg"
            };
            var scriptOne = this.LoadFromManifestWithoutRepositoryInfo(false, info);

            //assert
            Assert.AreEqual("TestRepo", scriptOne.RepositoryInfo.RepositoryName);
            Assert.AreEqual("TestOrg", scriptOne.RepositoryInfo.OrganizationName);
        }

        [Test]
        public void OverrideTrue_WithoutInfoInManifest_NoInfoInArgs_ShoudBeAsInManifest()
        {
            //arrange and act
            var scriptOne = this.LoadFromManifestWithoutRepositoryInfo(true, null);

            //assert
            Assert.AreEqual("Unspecified", scriptOne.RepositoryInfo.RepositoryName);
            Assert.AreEqual("Unspecified", scriptOne.RepositoryInfo.OrganizationName);
        }

        [Test]
        public void OverrideTrue_WithoutInfoInManifest_InfoInArgs_ShoudBeAsInManifest()
        {
            //arrange and act
            var info = new RepositoryInfo()
            {
                RepositoryName = "TestRepo",
                OrganizationName = "TestOrg"
            };
            var scriptOne = this.LoadFromManifestWithoutRepositoryInfo(true, info);

            //assert
            Assert.AreEqual("TestRepo", scriptOne.RepositoryInfo.RepositoryName);
            Assert.AreEqual("TestOrg", scriptOne.RepositoryInfo.OrganizationName);
        }


    }
}
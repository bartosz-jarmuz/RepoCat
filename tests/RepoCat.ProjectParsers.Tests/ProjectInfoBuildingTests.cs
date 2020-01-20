﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using System.Xml.Linq;
using NUnit.Framework;
using RepoCat.Transmission;
using RepoCat.Transmission.Contracts;
using RepoCat.Transmission.Models;

namespace RepoCat.ProjectParsers.Tests
{
    [TestFixture]
    public class ProjectInfoBuildingTests
    {
        [Test]
        public void NetFrameworkProject_NoManifest_Load()
        {
            var path = TestUtils.GetSampleProject(@"RepoCat.TestApps.NetFramework.NoManifest.csproj");

            var repo = new RepositoryInfo()
            {
                RepositoryName = "Test",
                OrganizationName = "TestOrg"
            };
            var provider = ProjectInfoBuilderFactory.Get(new TransmitterArguments()
            {
                TransmissionMode = TransmissionMode.LocalDotNetProjects,
                RepositoryName = repo.RepositoryName,
                OrganizationName = repo.OrganizationName,
            }, new Mock<ILogger>().Object);



            var info = provider.GetInfo(path.FullName);
            info.Should().NotBeNull();
            info.RepositoryInfo.Should().BeEquivalentTo(repo);
            info.Autogenerated.Should().BeTrue();
            info.RepositoryStamp.Should().NotBeNullOrEmpty();
            info.ProjectName.Should().BeEquivalentTo("RepoCat.TestApps.NetFramework.NoManifest");
            info.AssemblyName.Should().Be("RepoCat.TestApps.NetFramework.NoManifest");
            info.Components.Count.Should().Be(0);
        }

        [Test]
        public void NetFrameworkProject_NoManifest_DoNotLoad()
        {
            var path = TestUtils.GetSampleProject(@"RepoCat.TestApps.NetFramework.NoManifest.csproj");

            var provider = ProjectInfoBuilderFactory.Get(new TransmitterArguments()
            {
                TransmissionMode = TransmissionMode.LocalDotNetProjects,
                SkipProjectsWithoutManifest = true
            }, new Mock<ILogger>().Object);

            var info = provider.GetInfo(path.FullName);
            info.Should().BeNull();
        }

        [Test]
        public void NetFrameworkProject_Load()
        {
            var path = TestUtils.GetSampleProject(@"RepoCat.TestApps.NetFramework.csproj");

            var repo = new RepositoryInfo()
            {
                RepositoryName = "Test",
                OrganizationName = "TestOrg"
            };
            var provider = ProjectInfoBuilderFactory.Get(new TransmitterArguments()
            {
                TransmissionMode = TransmissionMode.LocalDotNetProjects, 
                RepositoryName = repo.RepositoryName,
                OrganizationName = repo.OrganizationName
            }, new Mock<ILogger>().Object);


           
            var info = provider.GetInfo(path.FullName);
            info.Should().NotBeNull();
            info.RepositoryInfo.Should().BeEquivalentTo(repo);
            info.DocumentationUri.Should().BeEquivalentTo("http://google.com/Somewhere");
            info.RepositoryStamp.Should().NotBeNullOrEmpty();
            info.ProjectName.Should().BeEquivalentTo("OverridenProjectName");
            info.AssemblyName.Should().Be("RepoCat.TestApps.NetFramework");
            info.Components.Single().Name.Should().Be("SampleNetFrameworkConsoleApp");
            info.Components.Single().Tags.Count.Should().Be(3);
        }

        [Test]
        public void NetCoreProject_Load()
        {
            var path = TestUtils.GetSampleProject(@"RepoCat.TestApps.NetCore.csproj");

            var repo = new RepositoryInfo()
            {
                RepositoryName = "Test",
                OrganizationName = "TestOrg"
            };
            var provider = ProjectInfoBuilderFactory.Get(new TransmitterArguments() { TransmissionMode = TransmissionMode.LocalDotNetProjects, RepositoryName = repo.RepositoryName, OrganizationName = repo.OrganizationName}, new Mock<ILogger>().Object);
            
            var info = provider.GetInfo(path.FullName);
            info.Should().NotBeNull();
            info.RepositoryInfo.Should().BeEquivalentTo(repo);
            info.Owner.Should().BeEquivalentTo("Jim The Beam");
            info.ProjectDescription.Should().BeEquivalentTo("Description is optional");
            info.ProjectName.Should().BeEquivalentTo("RepoCat.TestApps.NetCore");
            info.RepositoryStamp.Should().NotBeNullOrEmpty();
            info.AssemblyName.Should().Be("RepoCat.TestApps.NetCore");
            info.Tags.Should().BeEquivalentTo(new[] { "These", "Tags", "Are", "Optional"});
            info.Properties.Should().Contain(new KeyValuePair<string, string>("EntireProjectProperties", "AreAlsoOptional"));
            
            info.Components.Single().Name.Should().Be("SampleNetCoreConsoleApp");
            info.Components.Single().Tags.Count.Should().Be(3);


        }
    }
}

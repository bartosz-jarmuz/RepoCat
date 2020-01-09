using System.Linq;
using FluentAssertions;
using Moq;
using System.Xml.Linq;
using NUnit.Framework;
using RepoCat.Transmission;
using RepoCat.Transmission.Models;

namespace RepoCat.ProjectParsers.Tests
{
    [TestFixture]
    public class ProjectInfoBuildingTests
    {
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
            info.RepositoryStamp.Should().NotBeNullOrEmpty();
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
            info.RepositoryStamp.Should().NotBeNullOrEmpty();
            info.AssemblyName.Should().Be("RepoCat.TestApps.NetCore");
            info.Components.Single().Name.Should().Be("SampleNetCoreConsoleApp");
            info.Components.Single().Tags.Count.Should().Be(3);
        }
    }
}

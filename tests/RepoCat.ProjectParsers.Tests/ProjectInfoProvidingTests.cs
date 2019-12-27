using System.Linq;
using FluentAssertions;
using Moq;
using System.Xml.Linq;
using NUnit.Framework;
using RepoCat.Transmission.Client;
using RepoCat.Transmission.Models;

namespace RepoCat.ProjectParsers.Tests
{
    [TestFixture]
    public class ProjectInfoProvidingTests
    {
        [Test]
        public void NetFrameworkProject_Load()
        {
            var path = TestUtils.GetSampleProject(@"RepoCat.TestApps.NetFramework.csproj");

            var provider = new ProjectInfoProvider(new Mock<ILogger>().Object);
            var repo = new RepositoryInfo()
            {
                RepositoryName = "Test",
                OrganizationName = "TestOrg"
            };
            var info = provider.GetInfo(path.FullName, repo, "");
            info.Should().NotBeNull();
            info.AssemblyName.Should().Be("RepoCat.TestApps.NetFramework");
            info.Components.Single().Name.Should().Be("SampleNetFrameworkConsoleApp");
            info.Components.Single().Tags.Count.Should().Be(3);
        }

        [Test]
        public void NetCoreProject_Load()
        {
            var path = TestUtils.GetSampleProject(@"RepoCat.TestApps.NetCore.csproj");

            var provider = new ProjectInfoProvider(new Mock<ILogger>().Object);
            var repo = new RepositoryInfo()
            {
                RepositoryName = "Test",
                OrganizationName = "TestOrg"
            };
            var info = provider.GetInfo(path.FullName, repo, "");
            info.Should().NotBeNull();
            info.AssemblyName.Should().Be("RepoCat.TestApps.NetCore");
            info.Components.Single().Name.Should().Be("SampleNetCoreConsoleApp");
            info.Components.Single().Tags.Count.Should().Be(3);
        }
    }
}

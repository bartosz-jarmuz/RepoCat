using System.Linq;
using FluentAssertions;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoCat.Transmission.Client.Implementation;
using System.Xml.Linq;

namespace RepoCat.ProjectParsers.Tests
{
    [TestClass]
    public class ProjectInfoProvidingTests
    {
        [TestMethod]
        public void NetFrameworkProject_Load()
        {
            var path = TestUtils.GetSampleProject(@"RepoCat.TestApps.NetFramework.csproj");

            var provider = new ProjectInfoProvider(new Mock<ILog>().Object);

            var info = provider.GetInfo(path.FullName, "Test", "");
            info.Should().NotBeNull();
            info.AssemblyName.Should().Be("RepoCat.TestApps.NetFramework");
            info.Components.Single().Name.Should().Be("SampleNetFrameworkConsoleApp");
            info.Components.Single().Tags.Count.Should().Be(3);
        }

        [TestMethod]
        public void NetCoreProject_Load()
        {
            var path = TestUtils.GetSampleProject(@"RepoCat.TestApps.NetCore.csproj");

            var provider = new ProjectInfoProvider(new Mock<ILog>().Object);

            var info = provider.GetInfo(path.FullName, "Test", "");
            info.Should().NotBeNull();
            info.AssemblyName.Should().Be("RepoCat.TestApps.NetCore");
            info.Components.Single().Name.Should().Be("SampleNetCoreConsoleApp");
            info.Components.Single().Tags.Count.Should().Be(3);
        }
    }
}

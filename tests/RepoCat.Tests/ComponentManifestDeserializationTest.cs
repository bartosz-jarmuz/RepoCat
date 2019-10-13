using System.IO;
using FluentAssertions;
using NUnit.Framework;
using RepoCat.Transmission.Core;

namespace RepoCat.Tests
{
    public class ComponentManifestDeserializationTest
    {

        [Test]
        public void SampleManifest_MultipleComponents_WorksOK()
        {
            string text = File.ReadAllText(TestContext.CurrentContext.TestDirectory + "\\TestFiles\\SampleManifest.RepoCat.xml");

            var components = ManifestDeserializer.LoadComponents(text);

            components.Count.Should().Be(2);
            var first = components[0];
            var second = components[1];


            first.Name.Should().Be("PneumaticPick");
            first.Description.Should().Be("You don't even know you need it");
            first.Properties["Author"].Should().Be("JimBeam");
            first.Properties["Weight"].Should().Be("25kg");
            first.DocumentationUri.Should().Be("http://google.com");
            first.Tags.Should().BeEquivalentTo(new []{ "Hygiene" ,"Teeth", "Mining"  });

            second.Name.Should().Be("SteamPick");
            second.Properties["Author"].Should().Be("Jack Black");

            second.Tags.Should().BeEquivalentTo(new[] { "Coal", "Steam", "Injury" });
        }
    }
}
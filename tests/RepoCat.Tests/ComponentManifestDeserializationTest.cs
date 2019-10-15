using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using FluentAssertions;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using NUnit.Framework;
using RepoCat.Transmission.Core;
using RepoCat.Transmission.Models;

namespace RepoCat.Tests
{
    public class ComponentManifestDeserializationTest
    {
        [Test]
        public void Serialization_BackAndForth()
        {
            var info = new ProjectInfo();
            info.Components = new List<ComponentManifest>()
            {
                new ComponentManifest()
                {
                    Name = "bb",
                    Properties = new Dictionary<string, string>()
                    {
                        {"KEEY", "VAAAAAAAAL" }
                    }
                },
                new ComponentManifest()
                {
                    Name = "bb",
                    Properties = new Dictionary<string, string>()
                    {
                        {"2222", "333" }
                    }
                    ,Tags = new List<string>()
                    {
                        "Tag1",
                        "Tag2",
                        "3"
                    }
                }
            };

            XElement serialized = ManifestSerializer.SerializeComponents(info.Components);

            var components = ManifestSerializer.DeserializeComponents(serialized.ToString());

            components.Should().BeEquivalentTo(info.Components);
        }

        [Test]
        public void SampleManifest_MultipleComponents_WorksOK()
        {
            string text = File.ReadAllText(TestContext.CurrentContext.TestDirectory + "\\TestFiles\\SampleManifest.RepoCat.xml");

            var components = ManifestSerializer.DeserializeComponents(text);

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
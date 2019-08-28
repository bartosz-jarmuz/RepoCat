using System.IO;
using NUnit.Framework;
using RepoCat.Portal.Models;
using Shouldly;

namespace Tests
{
    public class ComponentManifestDeserializationTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SampleManifest_MultipleComponents_WorksOK()
        {
            string text = File.ReadAllText(TestContext.CurrentContext.TestDirectory + "\\SampleManifest.RepoCat.xml");

            var components = ManifestDeserializer.LoadComponents(text);

            components.Count.ShouldBe(2);
            var first = components[0];
            var second = components[1];


            first.Name.ShouldBe("PneumaticPick");
            first.Description.ShouldBe("You don't even know you need it");
            first.Authors.ShouldBe("Jim Beam");
            first.DocumentationUri.ShouldBe("http://google.com");
            first.Tags.ShouldSatisfyAllConditions(
                ()=> first.Tags.Count.ShouldBe(3),
                () => first.Tags.ShouldContain("Mining"),
                () => first.Tags.ShouldContain("Teeth"),
                () => first.Tags.ShouldContain("Hygiene")
            );

            second.Name.ShouldBe("SteamPick");
            second.Authors.ShouldBe("Jack Black");

            second.Tags.ShouldSatisfyAllConditions(
                () => first.Tags.Count.ShouldBe(3),
                () => second.Tags.ShouldContain("Coal"),
                () => second.Tags.ShouldContain("Steam"),
                () => second.Tags.ShouldContain("Injury")
            );


        }
    }
}
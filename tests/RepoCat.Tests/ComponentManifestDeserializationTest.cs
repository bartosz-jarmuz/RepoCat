﻿// -----------------------------------------------------------------------
//  <copyright file="ComponentManifestDeserializationTest.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using FluentAssertions;
using NUnit.Framework;
using RepoCat.Portal.Utilities;
using RepoCat.Serialization;
using RepoCat.Transmission.Models;

namespace RepoCat.Tests
{
    public class ComponentManifestDeserializationTest
    {
        [Test]
        public void Serialization_BackAndForth()
        {
            ProjectInfo info = SampleManifestXmlProvider.GetSampleProjectInfo();
            info.Properties.Add("Collection", new List<string>(){"First", "Second"});
            info.Components[0].Properties.Add("ComponentCollection", new List<string>() { "Third", "Second" });
           
            XElement projectInfoSerialized = ManifestSerializer.SerializeProjectInfo(info);

            ProjectInfo projectInfoDeserialized = ManifestDeserializer.DeserializeProjectInfo(projectInfoSerialized);


            projectInfoDeserialized.Should().BeEquivalentTo(info);
        }



        [Test]
        public void SampleManifest_MultipleComponents_WorksOK()
        {
            string text = File.ReadAllText(TestContext.CurrentContext.TestDirectory +
                                           "\\SampleManifestFiles\\SampleManifest.RepoCat.xml");

            var manifest = ManifestDeserializer.DeserializeProjectInfo(XElement.Parse(text));
            manifest.ProjectName.Should().Be("OptionallyProvidedProjectName");
            manifest.Tags.Should().BeEquivalentTo(new[] {"These", "Tags", "Are", "Optional"});
            manifest.Properties.Should()
                .ContainEquivalentOf(new Property("EntireProjectProperties", "AreAlsoOptional"));

        }

        [Test]
        public void SampleManifest_CheckComponents()
        {
            string text = File.ReadAllText(TestContext.CurrentContext.TestDirectory + "\\SampleManifestFiles\\SampleManifest.RepoCat.xml");

            var manifest = ManifestDeserializer.DeserializeProjectInfo(XElement.Parse(text));
            var components = manifest.Components;
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
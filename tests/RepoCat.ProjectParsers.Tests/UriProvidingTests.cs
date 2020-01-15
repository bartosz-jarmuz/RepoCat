// -----------------------------------------------------------------------
//  <copyright file="UriProvidingTests.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using FluentAssertions;
using NUnit.Framework;
using RepoCat.ProjectParsers.Tests;
using RepoCat.Schemas;
using RepoCat.Transmission;

namespace RepoCat.Tests
{
    [TestFixture]
    public class UriProvidingTests
    {
        [Test]
        public void IgnoredPaths_AreIgnored()
        {
            var root = TestUtils.GetSampleProjectsRoot();

            var uriProvider = new LocalDotNetProjectUriProvider(new TraceLogger(LogLevel.Debug));
            List<string> uris = uriProvider.GetUris(root.FullName).ToList();
            Assert.AreEqual(2, uris.Count);

            var regex = new Regex(@".*\\SampleProjects\\.*");
            uris = uriProvider.GetUris(root.FullName, regex).ToList();
            Assert.AreEqual(0, uris.Count);

            regex = new Regex(@"TestApps\.NetFramework");
            uris = uriProvider.GetUris(root.FullName, regex).ToList();
            Assert.AreEqual(1, uris.Count);
            StringAssert.Contains("RepoCat.TestApps.NetCore.csproj", uris.Single());
            
            regex = new Regex(@"TestApps\.NetFramework|TestApps\.NetCore");
            uris = uriProvider.GetUris(root.FullName, regex).ToList();
            Assert.AreEqual(0, uris.Count);
        }

        [Test]
        public void TestSchema_SampleFiles_AllAreValid()
        {
            Dictionary<string, List<string>> filesAndErrors = new Dictionary<string, List<string>>();
            foreach (var manifest in TestUtils.GetSampleProjectsRoot().EnumerateFiles( $"*.*", SearchOption.AllDirectories  ).Where(x=>x.FullName.EndsWith(Strings.ManifestSuffix, StringComparison.OrdinalIgnoreCase)))
            {
                string text = File.ReadAllText(manifest.FullName);
                var xDoc = XDocument.Parse(text);
                var validator = new SchemaValidator();
                var errors = validator.ValidateManifest(xDoc);

                filesAndErrors.Add(manifest.FullName, errors);
            }

            filesAndErrors.Count.Should().BeGreaterOrEqualTo(2);

            if (filesAndErrors.SelectMany(x => x.Value).Any())
            {
                var sb = new StringBuilder();
                foreach (KeyValuePair<string, List<string>> fileAndError in filesAndErrors)
                {
                    if (fileAndError.Value.Any())
                    {
                        sb.AppendLine($"File {fileAndError.Key} errors: \r\n\r\n{string.Join("\r\n", fileAndError.Value)}");
                    }
                }
                Assert.Fail(sb.ToString());
            }
        }


    }
}
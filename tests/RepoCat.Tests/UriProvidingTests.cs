// -----------------------------------------------------------------------
//  <copyright file="UriProvidingTests.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RepoCat.Transmission;

namespace RepoCat.Tests
{
    [TestFixture]
    public class UriProvidingTests
    {
        private static DirectoryInfo RepoRoot => new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "SampleScriptsRepository"));


        [Test]
        public void IgnoredPaths_AreIgnored()
        {
            ManifestBasedUriProvider uriProvider = new ManifestBasedUriProvider();
            List<string> uris = uriProvider.GetUris(RepoRoot.FullName).ToList();
            Assert.AreEqual(2, uris.Count);
            Assert.IsTrue(
                uris.Any(x => x.Contains("ScriptOneManifest.RepoCat.xml", StringComparison.OrdinalIgnoreCase)));
            Assert.IsTrue(
                uris.Any(x => x.Contains("ScriptTwoManifest.RepoCat.xml", StringComparison.OrdinalIgnoreCase)));
        }

        [Test]
        public void ManifestFilesPaths_ProvidedOk()
        {
            ManifestBasedUriProvider uriProvider = new ManifestBasedUriProvider();
            List<string> uris = uriProvider.GetUris(RepoRoot.FullName).ToList();
            Assert.AreEqual(2, uris.Count);
            Assert.IsTrue(uris.Any(x => x.Contains("ScriptOneManifest.RepoCat.xml", StringComparison.OrdinalIgnoreCase)));
            Assert.IsTrue(uris.Any(x => x.Contains("ScriptTwoManifest.RepoCat.xml", StringComparison.OrdinalIgnoreCase)));
        }

    }
}
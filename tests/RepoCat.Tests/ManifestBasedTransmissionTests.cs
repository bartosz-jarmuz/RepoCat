using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RepoCat.Portal;
using RepoCat.Portal.Mapping;
using RepoCat.Transmission.Client;

namespace RepoCat.Tests
{
    [TestFixture]
    public class ManifestBasedTransmissionTests
    {
        private static DirectoryInfo RepoRoot => new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "SampleScriptsRepository"));


        [Test]
        public void CheckManifestFilesPathsProvidedOk()
        {
            var uriProvider = new ManifestBasedUriProvider();
            var uris = uriProvider.GetUris(RepoRoot.FullName).ToList();
            Assert.AreEqual(2, uris.Count);
            Assert.IsTrue(uris.Any(x=>x.Contains("ScriptOneManifest.RepoCat.xml", StringComparison.OrdinalIgnoreCase)));
            Assert.IsTrue(uris.Any(x=>x.Contains("ScriptTwoManifest.RepoCat.xml", StringComparison.OrdinalIgnoreCase)));
        }
    }
}
// -----------------------------------------------------------------------
//  <copyright file="UriProvidingTests.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using RepoCat.ProjectParsers.Tests;
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

            var uriProvider = new LocalDotNetProjectUriProvider();
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

       

    }
}
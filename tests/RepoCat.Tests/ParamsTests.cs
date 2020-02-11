// -----------------------------------------------------------------------
//  <copyright file="ParamsTests.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using RepoCat.Transmission.Contracts;

namespace RepoCat.Tests
{
    [TestFixture]
    public class ParamsTests
    {
        [Test]
        public void TestPathsArray()
        {

            var arg = new TransmitterArguments()
            {
                RepositoryName = "MakeSureThisIsNotSplitAsPathsAre",
                ProjectPaths = new List<string>() {@"C:\Folder1\Project", @"C:\Folder2\Project", @"C:\Folder3\Project"}
            };
            
            var stringified = arg.SaveAsParameters();
            stringified.Should().Contain("MakeSureThisIsNotSplitAsPathsAre");
            var resolved = new TransmitterArguments();
            resolved.LoadParameters(stringified);

            resolved.ProjectPaths.Should().Contain(arg.ProjectPaths);
            resolved.RepositoryName.Should().Be("MakeSureThisIsNotSplitAsPathsAre");
        }
    }
}
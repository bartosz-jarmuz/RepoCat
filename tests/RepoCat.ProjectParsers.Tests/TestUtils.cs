﻿// -----------------------------------------------------------------------
//  <copyright file="TestUtils.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace RepoCat.ProjectParsers.Tests
{
    public static class TestUtils
    {
        public static DirectoryInfo GetSolutionDirectory()
        {
            var dir = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
            var slnDir = dir.Parent.Parent.Parent.Parent;
            ThrowIfNotExists(slnDir.FullName);
            return slnDir;
        }

        public static DirectoryInfo GetSampleProjectsRoot()
        {
            var sln = GetSolutionDirectory();

            DirectoryInfo samplesFolder = new DirectoryInfo(Path.Combine(sln.FullName, "tests", "SampleProjects"));
            ThrowIfNotExists(samplesFolder.FullName);
            return samplesFolder;
        }

        public static FileInfo GetSampleProject(string name)
        {
            var samplesFolder = GetSampleProjectsRoot();
            var projectFile = samplesFolder.EnumerateFiles("*.*", SearchOption.AllDirectories)
                .FirstOrDefault(x => x.Name == name);
            if (projectFile == null)
            {
                throw new FileNotFoundException($"Failed to find [{name}] anywhere under [{samplesFolder.FullName}]");
            }
            ThrowIfNotExists(projectFile);

            return projectFile;
        }

        private static void ThrowIfNotExists(FileInfo path)
        {
            if (!path.Exists)
            {
                throw new FileNotFoundException($"Failed to find a required test object: [{path}]");
            }
        }

        private static void ThrowIfNotExists(DirectoryInfo path)
        {
            if (!path.Exists)
            {
                throw new DirectoryNotFoundException($"Failed to find a required test object: [{path}]");
            }
        }

        private static void ThrowIfNotExists(string path)
        {
            if (!Directory.Exists(path) && !File.Exists(path))
            {
                throw new InvalidOperationException($"Failed to find a required test object: [{path}]");
            }
        }
    }
}
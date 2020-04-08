// -----------------------------------------------------------------------
//  <copyright file="ResultsSorterTests.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RepoCat.Persistence.Models;
using RepoCat.RepositoryManagement.Service;

namespace RepoCat.Tests
{
    [TestFixture]
    public class ResultsSorterTests
    {
        [Test]
        public void TestSorting_ProjectNameWorthMoreThaTags()
        {
            //arrange
            IManifestQueryResultSorter sorter = new ManifestQueryResultSorter();
            var tokens = new string[] { "ProjectName", "TagOne", "TagTwo", "TagThree", "part of description" };
            var projects = new List<Project>()
            {

                new Project()
                {
                    ProjectInfo = new ProjectInfo()
                    {
                        ProjectName = "AnotherName",
                        ProjectDescription = "Contains part of description",
                        Tags = { "TagOne", "TagTwo"}
                    }
                },
                new Project()
                {
                    ProjectInfo = new ProjectInfo()
                    {
                        ProjectName = "OtherProject",
                        Tags = { "TagOne", "TagTwo", "TagThree"}
                    }
                },
                new Project()
                {
                    ProjectInfo = new ProjectInfo()
                    {
                        ProjectName = "ProjectName"
                    }
                },
                new Project()
                {
                    ProjectInfo = new ProjectInfo()
                    {
                        ProjectName = "I Don't match",
                        Tags = { "Even", "Tags"}
                    }
                },
            };

            //act
            var result = sorter.Sort(projects, tokens).ToList();

            //assert
            Assert.AreEqual("ProjectName", result[0].ProjectInfo.ProjectName);
            Assert.IsTrue(result[0].SearchAccuracyScore > 1);

            Assert.AreEqual("OtherProject", result[1].ProjectInfo.ProjectName);
            Assert.IsTrue(result[1].SearchAccuracyScore> 1);

            Assert.AreEqual("AnotherName", result[2].ProjectInfo.ProjectName);
            Assert.IsTrue(result[2].SearchAccuracyScore> 1);

            Assert.AreEqual("I Don't match", result[3].ProjectInfo.ProjectName);
            Assert.AreEqual(0, result[3].SearchAccuracyScore);

        }

        [Test]
        public void TestSorting_ComponentNameWorthMoreThanTags()
        {
            //arrange
            IManifestQueryResultSorter sorter = new ManifestQueryResultSorter();
            var tokens = new string[] { "ComponentName", "TagOne", "TagTwo", "TagThree", "part of description" };
            var projects = new List<Project>()
            {

                new Project()
                {
                    ProjectInfo = new ProjectInfo()
                    {
                        ProjectName = "ShouldBeThird",
                        Components = { new ComponentManifest(){
                            Name = "AnotherName",
                            Description = "Contains part of description",
                            Tags = { "TagOne", "TagTwo"}
                        }},

                        
                    }
                },
                new Project()
                {
                    ProjectInfo = new ProjectInfo()
                    {
                        ProjectName = "ShouldBeSecond",
                        Components = { new ComponentManifest(){
                            Name = "Other",
                            Tags = { "TagOne", "TagTwo", "TagThree"}
                        }},
                    }
                },
                new Project()
                {
                    ProjectInfo = new ProjectInfo()
                    {
                        ProjectName = "ShouldBeFirst",
                        Components = { new ComponentManifest(){
                            Name = "ComponentName",
                        }},
                    }
                },
                new Project()
                {
                    ProjectInfo = new ProjectInfo()
                    {
                        ProjectName = "NoMatch",
                        Components = { new ComponentManifest(){
                            Name = "I don't match",
                            Tags = { "Even", "Tags"}

                        }},
                    }
                },
            };

            //act
            var result = sorter.Sort(projects, tokens).ToList();

            //assert
            Assert.AreEqual("ShouldBeFirst", result[0].ProjectInfo.ProjectName);
            Assert.IsTrue(result[0].SearchAccuracyScore> 1);

            Assert.AreEqual("ShouldBeSecond", result[1].ProjectInfo.ProjectName);
            Assert.IsTrue(result[1].SearchAccuracyScore> 1);

            Assert.AreEqual("ShouldBeThird", result[2].ProjectInfo.ProjectName);
            Assert.IsTrue(result[2].SearchAccuracyScore> 1);

            Assert.AreEqual("NoMatch", result[3].ProjectInfo.ProjectName);
            Assert.AreEqual(0, result[3].SearchAccuracyScore);

        }

        [Test]
        public void TestSorting_OneProjectHasABetterPercentageBonus_SoHasBetterMatchScore()
        {
            //arrange
            IManifestQueryResultSorter sorter = new ManifestQueryResultSorter();
            var tokens = new string[] { "SomeValue" };
            var projects = new List<Project>()
            {

                new Project()
                {
                    ProjectInfo = new ProjectInfo()
                    {
                        ProjectName = "SomethingWithSomeValueAndALotMore",
                    }
                },
                new Project()
                {
                    ProjectInfo = new ProjectInfo()
                    {
                        ProjectName = "WithSomeValueTest",
                    }
                },

            };

            //act
            var result = sorter.Sort(projects, tokens).ToList();

            //assert
            Assert.AreEqual("WithSomeValueTest", result[0].ProjectInfo.ProjectName);
            Assert.IsTrue(result[0].SearchAccuracyScore> result[1].SearchAccuracyScore);


        }

        [Test]
        public void TestSorting_PropertyValueShortContains_PenaltyAdded()
        {
            //arrange
            IManifestQueryResultSorter sorter = new ManifestQueryResultSorter();
            var tokens = new string[] {"xml"};
            var projects = new List<Project>()
            {

                new Project()
                {
                    ProjectInfo = new ProjectInfo()
                    {
                        ProjectName = "ProjectOne",
                        Properties = new PropertiesCollection()
                        {

                            {
                                "Property", new List<string>()
                                {
                                    "SomefileName.xml",
                                    "SomeOtherFileName.xml",
                                    "YetAnotherFileName.xml",
                                }

                            }
                        }

                    }
                },
                new Project()
                {
                    ProjectInfo = new ProjectInfo()
                    {
                        ProjectName = "ProjectTwo",
                        Properties = new PropertiesCollection()
                        {
                            {
                                "Property", new List<string>()
                                {
                                    "XmlValidator",
                                }

                            }
                        }
                    }
                },

            };

            //act
            var result = sorter.Sort(projects, tokens).ToList();

            //assert
            Assert.AreEqual("ProjectTwo", result[0].ProjectInfo.ProjectName);
            Assert.IsTrue(result[0].SearchAccuracyScore> result[1].SearchAccuracyScore);


        }


        [Test]
        public void TestSorting_TokenizedContains_WorthMoreThanStartsWith()
        {
            //arrange
            IManifestQueryResultSorter sorter = new ManifestQueryResultSorter();
            var tokens = new string[] { "Project" };
            var projects = new List<Project>()
            {

                new Project()
                {
                    ProjectInfo = new ProjectInfo()
                    {
                        ProjectName = "This.Is.My.Long.Namespace.Project.For.Xml.dll",
                    }
                },
                new Project()
                {
                    ProjectInfo = new ProjectInfo()
                    {
                        ProjectName = "OtherProjectName",
                    }
                },
            };

            //act
            var result = sorter.Sort(projects, tokens).ToList();

            //assert
            Assert.AreEqual("This.Is.My.Long.Namespace.Project.For.Xml.dll", result[0].ProjectInfo.ProjectName);
            Assert.IsTrue(result[0].SearchAccuracyScore > result[1].SearchAccuracyScore);


        }
    }
}
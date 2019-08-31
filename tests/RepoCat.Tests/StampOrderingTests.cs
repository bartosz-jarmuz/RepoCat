using System;
using System.Collections.ObjectModel;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using RepoCat.Portal.Models;
using RepoCat.Portal.Services;

namespace Tests
{
    public class StampOrderingTests
    {

        [Test]
        public void TestSorting_EmptyCollection_ShouldBeNull()
        {
            var collection = new Collection<string>();

            var result = StampSorter.GetNewestStamp(collection);

            result.Should().BeNull();
        }

        [Test]
        public void TestSorting_OnlyDateTimes_ReturnsNewest()
        {
            var collection = new Collection<string>();
            collection.Add(new DateTime(2000,01,01).ToString("O"));
            collection.Add(new DateTime(2000,01,03).ToString("O"));
            collection.Add(new DateTime(2000, 01, 02).ToString("O"));

            var result = StampSorter.GetNewestStamp(collection);

            result.Should().BeEquivalentTo(new DateTime(2000, 01, 03).ToString("O"));
        }

        [Test]
        public void TestSorting_DateTimeMixedWithIncorrectValue_ReturnsNewest()
        {
            var collection = new Collection<string>();
            collection.Add(new DateTime(2000, 01, 01).ToString("O"));
            collection.Add("NOT A DATE");
            collection.Add(new DateTime(2000, 01, 03).ToString("O"));
            collection.Add(new DateTime(2000, 01, 02).ToString("O"));

            var result = StampSorter.GetNewestStamp(collection);

            result.Should().BeEquivalentTo(new DateTime(2000, 01, 03).ToString("O"));
        }

        [Test]
        public void TestSorting_DateTimeCollectionStartsWithIncorrectValue_ReturnsNewest()
        {
            var collection = new Collection<string>();
            collection.Add("NOT A DATE");
            collection.Add(new DateTime(2000, 01, 01).ToString("O"));
            collection.Add(new DateTime(2000, 01, 03).ToString("O"));
            collection.Add(new DateTime(2000, 01, 02).ToString("O"));

            var result = StampSorter.GetNewestStamp(collection);

            result.Should().BeEquivalentTo(new DateTime(2000, 01, 03).ToString("O"));
        }
    }
}
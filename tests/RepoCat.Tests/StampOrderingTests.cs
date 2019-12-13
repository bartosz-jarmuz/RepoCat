using System;
using System.Collections.ObjectModel;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using RepoCat.Utilities;

namespace RepoCat.Tests
{
    [TestFixture]
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
            collection.Add(new DateTime(2000,01,01).ToString("O", CultureInfo.InvariantCulture));
            collection.Add(new DateTime(2000,01,03).ToString("O", CultureInfo.InvariantCulture));
            collection.Add(new DateTime(2000, 01, 02).ToString("O", CultureInfo.InvariantCulture));

            var result = StampSorter.GetNewestStamp(collection);

            result.Should().BeEquivalentTo(new DateTime(2000, 01, 03).ToString("O", CultureInfo.InvariantCulture));
        }

        [Test]
        public void TestSorting_DateTimeMixedWithIncorrectValue_ReturnsNewest()
        {
            var collection = new Collection<string>();
            collection.Add(new DateTime(2000, 01, 01).ToString("O", CultureInfo.InvariantCulture));
            collection.Add("NOT A DATE");
            collection.Add(new DateTime(2000, 01, 03).ToString("O", CultureInfo.InvariantCulture));
            collection.Add(new DateTime(2000, 01, 02).ToString("O", CultureInfo.InvariantCulture));

            var result = StampSorter.GetNewestStamp(collection);

            result.Should().BeEquivalentTo(new DateTime(2000, 01, 03).ToString("O", CultureInfo.InvariantCulture));
        }

        [Test]
        public void TestSorting_DateTimeCollectionStartsWithIncorrectValue_ReturnsNewest()
        {
            var collection = new Collection<string>();
            collection.Add("NOT A DATE");
            collection.Add(new DateTime(2000, 01, 01).ToString("O", CultureInfo.InvariantCulture));
            collection.Add(new DateTime(2000, 01, 03).ToString("O", CultureInfo.InvariantCulture));
            collection.Add(new DateTime(2000, 01, 02).ToString("O", CultureInfo.InvariantCulture));

            var result = StampSorter.GetNewestStamp(collection);

            result.Should().BeEquivalentTo(new DateTime(2000, 01, 03).ToString("O", CultureInfo.InvariantCulture));
        }
    }
}
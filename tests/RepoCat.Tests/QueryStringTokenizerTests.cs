// -----------------------------------------------------------------------
//  <copyright file="QueryStringTokenizerTests.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using RepoCat.Utilities;

namespace RepoCat.Tests
{
    public class QueryStringTokenizerTests
    {

        [Test]
        public void TestSplit_StringWithPhrases_WorksOK()
        {
            string input = "Find some \"subqueries with phrases\" inside -exclude";

            var result = QueryStringTokenizer.GetTokens(input);

            result.Should().BeEquivalentTo(new List<string>()
            {
                "Find",
                "some",
                "subqueries with phrases",
                "inside",
                "-exclude"
            });
        }

        [Test]
        public void TestSplit_NullOrEmpty_ReturnEmpty()
        {
            string input = "";

            var result = QueryStringTokenizer.GetTokens(input);
            result.Should().BeEquivalentTo(new List<string>());

            result = QueryStringTokenizer.GetTokens(null);
            result.Should().BeEquivalentTo(new List<string>());
        }
    }


}
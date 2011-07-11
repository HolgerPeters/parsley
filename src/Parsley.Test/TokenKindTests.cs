﻿using System;
using NUnit.Framework;

namespace Parsley
{
    [TestFixture]
    public class TokenKindTests
    {
        private TokenKind lower;
        private TokenKind upper;
        private Text abcDEF;

        [SetUp]
        public void SetUp()
        {
            lower = new TokenKind("Lowercase", @"[a-z]+");
            upper = new TokenKind("Uppercase", @"[A-Z]+");
            abcDEF = new Text("abcDEF");
        }

        [Test]
        public void ProducesNullTokenUponFailedPatternMatch()
        {
            Token token;

            upper.TryMatch(abcDEF, out token).ShouldBeFalse();
            token.ShouldBeNull();
        }

        [Test]
        public void ProducesTokenUponSuccessfulPatternMatch()
        {
            Token token;

            lower.TryMatch(abcDEF, out token).ShouldBeTrue();
            token.ShouldBe(lower, "abc", 1, 1);

            upper.TryMatch(abcDEF.Advance(3), out token).ShouldBeTrue();
            token.ShouldBe(upper, "DEF", 1, 4);
        }

        [Test]
        public void HasDescriptiveName()
        {
            lower.Name.ShouldEqual("Lowercase");
            upper.Name.ShouldEqual("Uppercase");
        }

        [Test]
        public void ProvidesConvenienceSubclassForDefiningKeywords()
        {
            Token token;
            var foo = new Keyword("foo");

            foo.Name.ShouldEqual("foo");

            foo.TryMatch(new Text("bar"), out token).ShouldBeFalse();
            token.ShouldBeNull();

            foo.TryMatch(new Text("foo"), out token).ShouldBeTrue();
            token.ShouldBe(foo, "foo", 1, 1);

            foo.TryMatch(new Text("foo bar"), out token).ShouldBeTrue();
            token.ShouldBe(foo, "foo", 1, 1);

            foo.TryMatch(new Text("foobar"), out token).ShouldBeFalse();
            token.ShouldBeNull();

            Action notJustLetters = () => new Keyword(" oops ");
            notJustLetters.ShouldThrow<ArgumentException>("Keywords may only contain letters.\r\nParameter name: word");
        }

        [Test]
        public void ProvidesConvenienceSubclassForDefiningOperators()
        {
            Token token;
            var star = new Operator("*");
            var doubleStar = new Operator("**");

            star.Name.ShouldEqual("*");

            star.TryMatch(new Text("a"), out token).ShouldBeFalse();
            token.ShouldBeNull();

            star.TryMatch(new Text("*"), out token).ShouldBeTrue();
            token.ShouldBe(star, "*", 1, 1);

            star.TryMatch(new Text("* *"), out token).ShouldBeTrue();
            token.ShouldBe(star, "*", 1, 1);

            star.TryMatch(new Text("**"), out token).ShouldBeTrue();
            token.ShouldBe(star, "*", 1, 1);

            doubleStar.Name.ShouldEqual("**");

            doubleStar.TryMatch(new Text("a"), out token).ShouldBeFalse();
            token.ShouldBeNull();

            doubleStar.TryMatch(new Text("*"), out token).ShouldBeFalse();
            token.ShouldBeNull();

            doubleStar.TryMatch(new Text("* *"), out token).ShouldBeFalse();
            token.ShouldBeNull();

            doubleStar.TryMatch(new Text("**"), out token).ShouldBeTrue();
            token.ShouldBe(doubleStar, "**", 1, 1);
        }
    }
}
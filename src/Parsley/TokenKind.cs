﻿using System;
using System.Linq;

namespace Parsley
{
    public abstract class TokenKind
    {
        private readonly string name;
        private readonly bool skippable;

        protected TokenKind(string name, bool skippable = false)
        {
            this.name = name;
            this.skippable = skippable;
        }

        public bool TryMatch(Text text, out Token token)
        {
            var match = Match(text);

            if (match.Success)
            {
                token = new Token(this, text.Position, match.Value);
                return true;
            }

            token = null;
            return false;
        }

        protected abstract MatchResult Match(Text text);

        public string Name
        {
            get { return name; }
        }

        public bool Skippable
        {
            get { return skippable;}
        }

        public override string ToString()
        {
            return name;
        }
    }

    public class RegexTokenKind : TokenKind
    {
        private readonly Pattern pattern;

        public RegexTokenKind(string name, string pattern, bool skippable = false)
            : base(name, skippable)
        {
            this.pattern = new Pattern(pattern);
        }

        protected override MatchResult Match(Text text)
        {
            return text.Match(pattern);
        }
    }

    public class Keyword : RegexTokenKind
    {
        public Keyword(string word)
            : base(word, word + @"\b")
        {
            if (word.Any(ch => !Char.IsLetter(ch)))
                throw new ArgumentException("Keywords may only contain letters.", "word");
        }
    }

    public class Operator : TokenKind
    {
        private readonly string symbol;

        public Operator(string symbol)
            : base(symbol)
        {
            this.symbol = symbol;
        }

        protected override MatchResult Match(Text text)
        {
            var peek = text.Peek(symbol.Length);

            if (peek == symbol)
                return MatchResult.Succeed(peek);

            return MatchResult.Fail();
        }
    }
}
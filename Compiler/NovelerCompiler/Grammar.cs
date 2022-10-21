using Noveler.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace NovelerCompiler
{
    interface IPattern
    {
        bool MatchesSequence(ReadOnlySpan<Token> sequence, out int readTokens);

        public static ExactPattern Exact(params IPattern[] patterns) =>
            new ExactPattern(patterns);

        public static ExactPattern Exact(params TokenType[] sequence) =>
            new ExactPattern(sequence);

        public static TokenPattern Tokens(params TokenType[] sequence) =>
            new TokenPattern(sequence);

        public static OnceOrManyPattern OnceOrMany(params IPattern[] pattern) =>
            new OnceOrManyPattern(pattern);

        public static OnceOrManyPattern OnceOrMany(params TokenType[] patternSequnce) =>
            new OnceOrManyPattern(patternSequnce);

        public static OptionalPattern Optional(IPattern pattern) =>
            new OptionalPattern(pattern);

        public static OptionalPattern Optional(params TokenType[] patternSequnce) =>
            new OptionalPattern(patternSequnce);

        public static ZeroOrManyPattern ZeroOrMany(params IPattern[] pattern) =>
            new ZeroOrManyPattern(pattern);

        public static ZeroOrManyPattern ZeroOrMany(params TokenType[] patternSequnce) =>
            new ZeroOrManyPattern(patternSequnce);

        public static AnyOfPattern Any(params IPattern[] patternSequence) =>
            new AnyOfPattern(patternSequence);
        public static AnyOfPattern Any(params TokenType[] tokenOptions) =>
            new AnyOfPattern(tokenOptions);

        public static NoneOfPattern None(params IPattern[] patterns) =>
            new NoneOfPattern(patterns);

        public static NoneOfPattern None(params TokenType[] tokenOptions) =>
            new NoneOfPattern(tokenOptions);
    }

    internal sealed class Grammar : IPattern
    {
        IPattern[]? _patternSequence;

        [MemberNotNull(nameof(_patternSequence))]
        public void SetGrammar(params IPattern[] patternSequence)
        {
            if (_patternSequence != null)
                throw new InvalidOperationException("Cannot redefine a set grammar.");

            _patternSequence = patternSequence;
        }

        public bool MatchesSequence(ReadOnlySpan<Token> sequence, out int readTokens)
        {
            if (_patternSequence == null || _patternSequence.Length == 0)
                throw new InvalidOperationException("There is no set grammar for this rule.");

            readTokens = 0;

            var sequenceSlice = sequence;

            for (int i = 0; i < _patternSequence.Length; i++)
            {
                if (!_patternSequence[i].MatchesSequence(sequenceSlice, out int readCount))
                {
                    readTokens = 0;
                    return false;
                }

                sequenceSlice = sequenceSlice[readCount..];
                readTokens += readCount;
            }

            return true;
        }
    }

    internal sealed class TokenPattern : IPattern
    {
        readonly TokenType[] _matchSequence;

        public TokenPattern(params TokenType[] sequence)
        {
            _matchSequence = sequence;
        }

        public bool MatchesSequence(ReadOnlySpan<Token> sequence, out int readTokens)
        {
            readTokens = default;
            if (sequence.Length < _matchSequence.Length)
                return false;

            // If a token type does not match return false early
            for (int i = 0; i < _matchSequence.Length; i++)
            {
                if (sequence[i].Type != _matchSequence[i])
                    return false;
            }

            readTokens = _matchSequence.Length;
            return true;
        }
    }

    internal sealed class ExactPattern : IPattern
    {
        IPattern[] _patterns;

        public ExactPattern(params IPattern[] pattern)
        {
            _patterns = pattern;
        }

        public ExactPattern(params TokenType[] patternSequnce)
        {
            _patterns = new IPattern[1];
            _patterns[0] = new TokenPattern(patternSequnce);
        }

        public bool MatchesSequence(ReadOnlySpan<Token> sequence, out int readTokens)
        {
            readTokens = 0;
            var sequenceSlice = sequence;

            for (int i = 0; i < _patterns.Length; i++)
            {
                if (!_patterns[i].MatchesSequence(sequenceSlice, out int readCount))
                {
                    readTokens = 0;
                    return false;
                }

                sequenceSlice = sequenceSlice[readCount..];
                readTokens += readCount;
            }

            return true;
        }
    }

    internal sealed class OnceOrManyPattern : IPattern
    {
        readonly IPattern _pattern;

        public OnceOrManyPattern(params IPattern[] pattern)
        {
            _pattern = new ExactPattern(pattern);
        }

        public OnceOrManyPattern(params TokenType[] patternSequnce)
        {
            _pattern = new TokenPattern(patternSequnce);
        }

        public bool MatchesSequence(ReadOnlySpan<Token> sequence, out int readTokens)
        {
            readTokens = 0;

            var sequenceSlice = sequence;

            while (_pattern.MatchesSequence(sequenceSlice, out int readCount))
            {
                sequenceSlice = sequenceSlice[readCount..];
                readTokens += readCount;
            }

            return readTokens > 0;
        }
    }

    internal sealed class OptionalPattern : IPattern
    {
        readonly IPattern _pattern;

        public OptionalPattern(IPattern pattern)
        {
            _pattern = pattern;
        }

        public OptionalPattern(params TokenType[] patternSequnce)
        {
            _pattern = new TokenPattern(patternSequnce);
        }

        public bool MatchesSequence(ReadOnlySpan<Token> sequence, out int readTokens)
        {
            readTokens = _pattern.MatchesSequence(sequence, out var readCount) ? readCount : 0;
            return true;
        }
    }

    internal sealed class ZeroOrManyPattern : IPattern
    {
        readonly IPattern _pattern;

        public ZeroOrManyPattern(params IPattern[] pattern)
        {
            _pattern = new OptionalPattern(new OnceOrManyPattern(pattern));
        }

        public ZeroOrManyPattern(params TokenType[] patternSequnce)
        {
            _pattern = new OptionalPattern(new OnceOrManyPattern(patternSequnce));
        }

        public bool MatchesSequence(ReadOnlySpan<Token> sequence, out int readTokens)
        {
            return _pattern.MatchesSequence(sequence, out readTokens);
        }
    }

    internal sealed class AnyOfPattern : IPattern
    {
        readonly IPattern[] _patternSequence;

        public AnyOfPattern(params IPattern[] patternSequence)
        {
            _patternSequence = patternSequence;
        }

        public AnyOfPattern(params TokenType[] tokenOptions)
        {
            _patternSequence = new IPattern[tokenOptions.Length];

            for (int i = 0; i < tokenOptions.Length; i++)
            {
                _patternSequence[i] = new TokenPattern(tokenOptions[i]);
            }
        }

        public bool MatchesSequence(ReadOnlySpan<Token> sequence, out int readTokens)
        {
            for (int i = 0; i < _patternSequence.Length; i++)
            {
                if (_patternSequence[i].MatchesSequence(sequence, out readTokens))
                    return true;
            }

            readTokens = 0;
            return false;
        }
    }

    internal sealed class NoneOfPattern : IPattern
    {
        readonly IPattern[] _patternSequence;

        public NoneOfPattern(params IPattern[] patternSequence)
        {
            _patternSequence = patternSequence;
        }

        public NoneOfPattern(params TokenType[] tokenOptions)
        {
            _patternSequence = new IPattern[tokenOptions.Length];

            for (int i = 0; i < tokenOptions.Length; i++)
            {
                _patternSequence[i] = new TokenPattern(tokenOptions[i]);
            }
        }

        public bool MatchesSequence(ReadOnlySpan<Token> sequence, out int readTokens)
        {
            for (int i = 0; i < _patternSequence.Length; i++)
            {
                if (_patternSequence[i].MatchesSequence(sequence, out _))
                {
                    readTokens = 0;
                    return true;
                }
            }

            readTokens = 1;
            return false;
        }
    }
}

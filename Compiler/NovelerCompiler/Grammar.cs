using Noveler.Compiler;

namespace NovelerCompiler
{
    interface IPattern
    {
        bool MatchesSequence(ReadOnlySpan<Token> sequence, out int readTokens);
    }

    internal sealed class Grammar : IPattern
    {
        readonly IPattern[] _patternSequence;

        public Grammar(params IPattern[] patternSequence)
        {
            _patternSequence = patternSequence;
        }

        public bool MatchesSequence(ReadOnlySpan<Token> sequence, out int readTokens)
        {
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

    internal sealed class ExactPattern : IPattern
    {
        readonly TokenType[] _matchSequence;

        public ExactPattern(params TokenType[] sequence)
        {
            _matchSequence = sequence;
        }

        public bool MatchesSequence(ReadOnlySpan<Token> sequence, out int readTokens)
        {
            readTokens = default;
            if (sequence.Length < _matchSequence.Length)
                return false;

            for (int i = 0; i < _matchSequence.Length; i++)
            {
                if (sequence[i].Type == _matchSequence[i])
                    return false;
            }

            readTokens = _matchSequence.Length;
            return true;
        }
    }

    internal sealed class OnceOrManyPattern : IPattern
    {
        readonly IPattern _pattern;

        public OnceOrManyPattern(IPattern pattern)
        {
            _pattern = pattern;
        }

        public OnceOrManyPattern(params TokenType[] patternSequnce)
        {
            _pattern = new ExactPattern(patternSequnce);
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
            _pattern = new ExactPattern(patternSequnce);
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

        public ZeroOrManyPattern(IPattern pattern)
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
}

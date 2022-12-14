using Noveler.Compiler;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NovelerCompiler
{
    interface IPattern
    {
        bool MatchesSequence(ReadOnlySpan<Token> sequence, out int readTokens, [NotNullWhen(true)] out ParseTreeNode? treeState);

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

        public static OptionalPattern ZeroOrMany(params IPattern[] pattern) =>
            new OptionalPattern(new OnceOrManyPattern(pattern));

        public static OptionalPattern ZeroOrMany(params TokenType[] patternSequnce) =>
            new OptionalPattern(new OnceOrManyPattern(patternSequnce));

        public static AnyOfPattern Any(params IPattern[] patternSequence) =>
            new AnyOfPattern(patternSequence);
        public static AnyOfPattern Any(params TokenType[] tokenOptions) =>
            new AnyOfPattern(tokenOptions);

        public static NoneOfTokensPattern None(params TokenType[] tokenOptions) =>
            new NoneOfTokensPattern(tokenOptions);
    }

    [DebuggerDisplay("Grammar ({Kind})")]
    internal sealed class Grammar : IPattern
    {
        public GrammarKind Kind { get; }

        IPattern[]? _patternSequence;

        public Grammar(GrammarKind kind)
        {
            Kind = kind;
        }

        [MemberNotNull(nameof(_patternSequence))]
        public void SetGrammar(params IPattern[] patternSequence)
        {
            if (_patternSequence != null)
                throw new InvalidOperationException("Cannot redefine a set grammar.");

            _patternSequence = patternSequence;
        }

        public bool MatchesSequence(ReadOnlySpan<Token> sequence, out int readTokens, [NotNullWhen(true)] out ParseTreeNode? treeState)
        {
            if (_patternSequence == null || _patternSequence.Length == 0)
                throw new InvalidOperationException("There is no set grammar for this rule.");

            readTokens = 0;

            var sequenceSlice = sequence;
            treeState = new ParseTreeNode(ParseTreeNode.NodeKind.Grammar, Kind);

            for (int i = 0; i < _patternSequence.Length; i++)
            {
                if (!_patternSequence[i].MatchesSequence(sequenceSlice, out int readCount, out ParseTreeNode? node))
                {
                    readTokens = 0;
                    treeState = null;
                    return false;
                }

                treeState.AddNodeOrAdoptChildren(node);
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

        public bool MatchesSequence(ReadOnlySpan<Token> sequence, out int readTokens, [NotNullWhen(true)] out ParseTreeNode? treeState)
        {
            readTokens = default;
            if (sequence.Length < _matchSequence.Length)
            {
                treeState = null;
                return false;
            }

            treeState = new ParseTreeNode(ParseTreeNode.NodeKind.Sequence);

            // If a token type does not match return false early
            for (int i = 0; i < _matchSequence.Length; i++)
            {
                if (sequence[i].Type != _matchSequence[i])
                {
                    treeState = null;
                    return false;
                }

                var tokenNode = new ParseTreeNode(ParseTreeNode.NodeKind.Token)
                {
                    Token = sequence[i]
                };

                treeState.Children!.Add(tokenNode);
            }

            readTokens = _matchSequence.Length;
            return true;
        }
    }

    internal sealed class ExactPattern : IPattern
    {
        readonly IPattern[] _patterns;

        public ExactPattern(params IPattern[] pattern)
        {
            _patterns = pattern;
        }

        public ExactPattern(params TokenType[] patternSequnce)
        {
            _patterns = new IPattern[1];
            _patterns[0] = new TokenPattern(patternSequnce);
        }

        public bool MatchesSequence(ReadOnlySpan<Token> sequence, out int readTokens, [NotNullWhen(true)] out ParseTreeNode? treeState)
        {
            readTokens = 0;
            var sequenceSlice = sequence;

            treeState = new ParseTreeNode(ParseTreeNode.NodeKind.Sequence);

            for (int i = 0; i < _patterns.Length; i++)
            {
                if (!_patterns[i].MatchesSequence(sequenceSlice, out int readCount, out ParseTreeNode? node))
                {
                    readTokens = 0;
                    treeState = null;
                    return false;
                }

                treeState.AddNodeOrAdoptChildren(node);

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

        public bool MatchesSequence(ReadOnlySpan<Token> sequence, out int readTokens, [NotNullWhen(true)] out ParseTreeNode? treeState)
        {
            readTokens = 0;

            var sequenceSlice = sequence;
            treeState = new ParseTreeNode(ParseTreeNode.NodeKind.Sequence);

            while (_pattern.MatchesSequence(sequenceSlice, out int readCount, out ParseTreeNode? node))
            {
                treeState.AddNodeOrAdoptChildren(node);
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

        public bool MatchesSequence(ReadOnlySpan<Token> sequence, out int readTokens, [NotNullWhen(true)] out ParseTreeNode? treeState)
        {
            treeState = new ParseTreeNode(ParseTreeNode.NodeKind.Sequence);

            if (_pattern.MatchesSequence(sequence, out var readCount, out ParseTreeNode? node))
            {
                treeState.AddNodeOrAdoptChildren(node);
                readTokens = readCount;
            }
            else
            {
                readTokens = 0;
            }

            return true;
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

        public bool MatchesSequence(ReadOnlySpan<Token> sequence, out int readTokens, [NotNullWhen(true)] out ParseTreeNode? treeState)
        {
            for (int i = 0; i < _patternSequence.Length; i++)
            {
                if (_patternSequence[i].MatchesSequence(sequence, out readTokens, out treeState))
                    return true;
            }

            treeState = null;
            readTokens = 0;
            return false;
        }
    }

    internal sealed class NoneOfTokensPattern : IPattern
    {
        readonly IPattern[] _patternSequence;

        public NoneOfTokensPattern(params TokenType[] tokenOptions)
        {
            _patternSequence = new IPattern[tokenOptions.Length];

            for (int i = 0; i < tokenOptions.Length; i++)
            {
                _patternSequence[i] = new TokenPattern(tokenOptions[i]);
            }
        }

        public bool MatchesSequence(ReadOnlySpan<Token> sequence, out int readTokens, [NotNullWhen(true)] out ParseTreeNode? treeState)
        {
            readTokens = 0;
            treeState = new ParseTreeNode(ParseTreeNode.NodeKind.Sequence);

            for (int i = 0; i < _patternSequence.Length; i++)
            {
                if (_patternSequence[i].MatchesSequence(sequence, out _, out _))
                {
                    return false;
                }
            }

            return true;
        }
    }
}

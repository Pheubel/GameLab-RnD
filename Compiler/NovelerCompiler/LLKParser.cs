using System;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovelerCompiler
{
    class ParserContext
    {
        List<LLKParser> _invalidParsers = new List<LLKParser>();
    }

    internal class LLKStack
    {
        private List<LLKParser> _parsers = new List<LLKParser>();

        public bool Push(LLKParser parser)
        {
            if (_parsers.Contains(parser))
            {
                _parsers.Add(parser);
                return true;
            }
            return false;
        }

        public LLKParser Pop()
        {
            var parser = _parsers[^1];

            _parsers.RemoveAt(_parsers.Count - 1);

            return parser;
        }

        public LLKParser Peek() => _parsers[^1];
    }

    internal class LLKParser
    {
        public GrammarKind Kind { get; }
    }
}

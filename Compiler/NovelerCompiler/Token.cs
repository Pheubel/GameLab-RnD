using System.Diagnostics;

namespace Noveler.Compiler
{
    [DebuggerDisplay("Token ({Type})")]
    public class Token
    {
        public TokenType Type { get; set; }
        public TypeId ValueType { get; set; } = TypeId.Undeclared;
        public string ValueString { get; }
        public Range PositionOnLine { get; }
        public int LineNumber { get; }
        public string? SourceFile { get; }


        internal Token(TokenType type, ref ReadingContext contex, int tokenLength, string? valueString = null)
        {
            Type = type;
            ValueString = valueString ?? string.Empty;
            PositionOnLine = new Range(contex.CharacterOnLine, contex.CharacterOnLine + tokenLength);
            LineNumber = contex.LineNumber;
            SourceFile = contex.CurrentFile;
        }

        public Token(TokenType type, Range positionOnLine = default, int lineNumber = default, string? sourceFile = null, string? valueString = null)
        {
            Type = type;
            ValueString = valueString ?? string.Empty;
            PositionOnLine = positionOnLine;
            LineNumber = lineNumber;
            SourceFile = sourceFile;
        }

        public static Token InvalidToken { get; } = new Token(TokenType.InvalidToken);
    }
}
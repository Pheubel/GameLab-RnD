namespace Noveler.Compiler
{
    class Token
    {
        public TokenType Type;
        public string ValueString;

        public Token(TokenType type, string? valueString = null)
        {
            Type = type;
            ValueString = valueString ?? string.Empty;
        }

        public static Token InvalidToken { get; } = new Token(TokenType.InvalidToken);
    }
}
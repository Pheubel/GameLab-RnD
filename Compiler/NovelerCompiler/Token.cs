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
    }
}
﻿using System.Diagnostics;

namespace Noveler.Compiler
{
    [DebuggerDisplay("Token ({Type})")]
    public class Token
    {
        public TokenType Type;
        public string ValueString;

        public InternalType ValueType { get; set; } = InternalType.Undeclared;

        public Token(TokenType type, string? valueString = null)
        {
            Type = type;
            ValueString = valueString ?? string.Empty;
        }

        public static Token InvalidToken { get; } = new Token(TokenType.InvalidToken);
    }
}
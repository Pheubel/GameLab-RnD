﻿using NovelerCompiler;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Noveler.Compiler
{
    internal static class Utilities
    {
        public static readonly IReadOnlyDictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>()
        {

        };

        public static readonly IReadOnlySet<string> ReservedKeywords = new HashSet<string>()
        {
            "signed",
            "unsigned",
            "tiny",
            "small",
            "byte",
            "sbyte",
            "short",
            "ushort",
            "uint",
            "ulong",
            "text",
            "string",
            "null",
            "none",
            "ref",
            "char",

            "and",
            "or",
            "not",
            "is",
            "xor",
            "lesser",
            "greater",
            "than",
            "big",
            "whole",
            "number",
            "flag",
            "set",
            "int",
            "long",
            "float",
            "double",
            "include",
            "true",
            "false",
            "equal",
            "equals",
            "to",
            "function",
            "choice",
            "do",
            "while",
            "for",
            "each",
            "return",
            "bool",
            "boolean",
            "event"
        };

        public static int SkipSpace(ReaderWrapper input)
        {
            int skipped = default;
            while (input.Peek() == ' ')
            {
                input.Read();
                skipped++;
            }

            return skipped;
        }

        public static bool MatchCharacter(this ReaderWrapper reader, char character)
        {
            if (reader.Peek() == character)
            {
                reader.Read();
                return true;
            }

            return false;
        }

        public static bool MatchDigit(this ReaderWrapper reader, out char digit)
        {
            digit = (char)reader.Peek();
            if (digit >= '0' && digit <= '9')
            {
                reader.Read();
                return true;
            }

            return false;
        }

        public static bool IsAlpha(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
        }

        public static bool IsNumeric(char c)
        {
            return c >= '0' && c <= '9';
        }

        public static bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsNumeric(c);
        }

        public static bool IsTerminator(char c)
        {
            return c == '\n' || c == ';';
        }

        public static TypeId GetTargetType(TypeId leftHandSide, TypeId rightHandSide)
        {
            if (leftHandSide.IsCustomType() || rightHandSide.IsCustomType())
                throw new InvalidOperationException();

            return (TypeId)Math.Max((int)leftHandSide, (int)rightHandSide);
        }

        public static bool IsValueToken(this TokenType token)
        {
            return ValueTokens.Contains(token);
        }

        public static bool IsOperationToken(this TokenType token)
        {
            return OperationTokens.Contains(token);
        }

        public static bool IsExpressionToken(this TokenType token)
        {
            return ExpressionTokens.Contains(token);
        }

        public static bool IsFactorToken(this TokenType token)
        {
            return FactorTokens.Contains(token);
        }

        readonly static TokenType[] ValueTokens =
        {
            TokenType.IntLiteral,
            TokenType.LongLiteral,
            TokenType.FloatLiteral,
            TokenType.DoubleLiteral,
            TokenType.ValueVariable
        };

        readonly static TokenType[] OperationTokens =
        {
            TokenType.Add,
            TokenType.Subtract,
            TokenType.Multiply,
            TokenType.Divide,
            TokenType.Negate
        };

        readonly static TokenType[] ExpressionTokens =
        {
            TokenType.Add,
            TokenType.Subtract
        };

        readonly static TokenType[] FactorTokens =
        {
            TokenType.Multiply,
            TokenType.Divide
        };
    }
}
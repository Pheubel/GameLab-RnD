using System.Runtime.CompilerServices;

namespace Noveler.Compiler
{
    internal static class Utilities
    {
        /// <summary>
        /// Moves the start of the span over to the first non-space character.
        /// </summary>
        /// <param name="input"> The span holding characters to check.</param>
        /// <returns> The number of characters skipped.</returns>
        public static int SkipSpace(ref ReadOnlySpan<char> input)
        {
            int cursor;
            for (cursor = 0; cursor < input.Length; cursor++)
            {
                if (input[cursor] != ' ')
                {
                    break;
                }
            }

            input = input[cursor..];
            return cursor;
        }

        public static bool IsValueToken(this TokenType token)
        {
            return ValueTokens.Contains(token);
        }

        public static bool IsOperationToken(this TokenType token)
        {
            return OperationTokens.Contains(token);
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
            TokenType.Plus,
            TokenType.Minus,
            TokenType.Multiply,
            TokenType.Divide
        };
    }
}
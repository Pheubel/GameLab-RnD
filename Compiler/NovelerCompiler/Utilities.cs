namespace Noveler.Compiler
{
    internal static class Utilities
    {
        /// <summary>
        /// Moves the start of the span over to the first non-space character.
        /// </summary>
        /// <param name="input"> The span holding characters to check.</param>
        public static void SkipSpace(ref ReadOnlySpan<char> input)
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
        }

        public static bool IsValueToken(this TokenType token)
        {
            return true;
        }
    }
}
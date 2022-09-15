using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Noveler.Compiler
{
    public class Compiler
    {
        // TODO use TextReader to use streams instead in the future?
        public static bool Compile(ReadOnlySpan<char> script, out List<byte> result, out IReadOnlyList<CompilerMessage> messages)
        {
            var outMessages = new List<CompilerMessage>();
            messages = outMessages;
            StringBuilder stringBuilder = new StringBuilder();
            bool isValid = true;

            ReadOnlySpan<char> untokenizedInput = script;

            List<Token> tokens = new List<Token>();

            // tokenize the script
            while (!untokenizedInput.IsWhiteSpace())
            {
                var token = ReadToken(ref untokenizedInput);

                // if the token is an end of file, wrap up tokenizing. 
                if (token.Type == TokenType.EndOfFile)
                    break;

                if (token.Type == TokenType.InvalidToken)
                {
                    isValid = false;
                    outMessages.Add(new CompilerMessage($"Found invalid token: {token.ValueString}", CompilerMessage.MessageCode.InvalidToken, "idk")); // TODO add sauce
                    break;
                }

                tokens.Add(token);
            }

            if (!isValid)
            {
                result = new List<byte>(0);
                return false;
            }

            result = new List<byte>(2048);



            return isValid;
        }


        private static Token ReadToken(ref ReadOnlySpan<char> input)
        {
            Utilities.SkipSpace(ref input);

            // for now only parse a single character as a token. TODO change this
            Token token = input[0] switch
            {
                >= '0' and <= '9' => new Token(TokenType.IntLiteral, input[0].ToString()),
                '+' => new Token(TokenType.Plus),
                '-' => new Token(TokenType.Minus),
                _ => new Token(TokenType.InvalidToken, input[0].ToString())
            };

            return token;
        }
    }
}
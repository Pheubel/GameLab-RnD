using Noveler.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NovelerCompiler
{
    public static class Lexer
    {
        /// <summary>
        /// Analyze the raw script and turn it into a lexical stream of tokens.
        /// </summary>
        /// <param name="untokenizedInput"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static List<Token> Lex(ReaderWrapper untokenizedInput, ref ReadingContext context)
        {
            List<Token> tokens = new List<Token>(512);

            // tokenize the script
            while (true)
            {
                var token = ReadToken(untokenizedInput, ref context);

                if (token.Type == TokenType.InvalidToken)
                {
                    context.AddErrorMessage(new CompilerMessage($"Found invalid token: {token.ValueString}", CompilerMessage.MessageCode.InvalidToken, ref context));
                    continue;
                }

                tokens.Add(token);

                // if the token is an end of file, wrap up tokenizing. 
                if (token.Type == TokenType.EndOfFile)
                    break;
            }

            return tokens;
        }

        private static Token ReadToken(ReaderWrapper input, ref ReadingContext context)
        {
            context.CharacterOnLine += Utilities.SkipSpace(input);

            LexIntoToken(input, ref context, out Token token);

            return token;
        }

        private static void LexIntoToken(ReaderWrapper input, ref ReadingContext context, [NotNull] out Token? token)
        {
            token = default;
            char firstChar = input.PeekChar();
            switch (firstChar)
            {
                // determine if we are dealing with a number
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    if (TryHandleNumber(input, ref context, out var numberToken))
                    {
                        token = numberToken;
                        return;
                    };
                    break;

                case '+':
                    input.Read();
                    if (input.MatchCharacter('+'))
                    {
                        context.CharacterOnLine += 2;
                        token = new Token(TokenType.Increment);
                    }
                    else if (input.MatchCharacter('='))
                    {
                        context.CharacterOnLine += 2;
                        token = new Token(TokenType.AddAssign);
                    }
                    else
                    {
                        context.CharacterOnLine += 1;
                        token = new Token(TokenType.Add);
                    }
                    return;

                case '-':
                    input.Read();
                    if (input.MatchCharacter('-'))
                    {
                        context.CharacterOnLine += 2;
                        token = new Token(TokenType.Decrement);
                    }
                    else if (input.MatchCharacter('='))
                    {
                        context.CharacterOnLine += 2;
                        token = new Token(TokenType.SubtractAssign);
                    }
                    else
                    {
                        context.CharacterOnLine += 1;
                        token = new Token(TokenType.Subtract);
                    }
                    return;

                case '*':
                    input.Read();
                    if (input.MatchCharacter('='))
                    {
                        context.CharacterOnLine += 2;
                        token = new Token(TokenType.MultiplyAssign);
                    }
                    else
                    {
                        context.CharacterOnLine += 1;
                        token = new Token(TokenType.Multiply);
                    }
                    return;

                case '/':
                    input.Read();
                    if (input.MatchCharacter('='))
                    {
                        context.CharacterOnLine += 2;
                        token = new Token(TokenType.DivideAssign);
                    }
                    else
                    {
                        context.CharacterOnLine += 1;
                        token = new Token(TokenType.Divide);
                    }
                    return;

                case '(':
                    input.Read();
                    context.CharacterOnLine++;
                    token = new Token(TokenType.LeftParenthesis);
                    return;

                case ')':
                    input.Read();
                    context.CharacterOnLine++;
                    token = new Token(TokenType.RightParenthesis);
                    return;

                case '\r':
                    input.Read();
                    if (input.MatchCharacter('\n'))
                    {
                        context.AdvanceLine();
                        token = new Token(TokenType.NewLine);
                    }
                    break;

                case '\n':
                    input.Read();
                    context.AdvanceLine();
                    token = new Token(TokenType.NewLine);
                    break;

                case ';':
                    input.Read();
                    context.CharacterOnLine++;
                    token = new Token(TokenType.SemiColon);
                    break;

                case ':':
                    input.Read();
                    context.CharacterOnLine++;
                    token = new Token(TokenType.Colon);
                    break;

                case ',':
                    input.Read();
                    context.CharacterOnLine++;
                    token = new Token(TokenType.Comma);
                    break;

                case unchecked((char)-1):
                    token = new Token(TokenType.EndOfFile);
                    return;

                default:
                    break;
            }

            // handle symbols and keywords
            if (Utilities.IsAlpha(firstChar))
            {
                if (TryHandleSymbolOrKeyword(input, ref context, out token))
                {
                    return;
                }
            }

            // handle unmatched tokens
            token ??= Token.InvalidToken;
        }

        private static bool TryHandleSymbolOrKeyword(ReaderWrapper input, ref ReadingContext context, [NotNullWhen(true)] out Token? token)
        {
            using var charBuffer = PooledList<char>.Rent(512);

            charBuffer.Add(input.ReadChar());

            while (true)
            {
                char c = input.PeekChar();

                if (Utilities.IsAlphaNumeric(c))
                {
                    input.Read();

                    charBuffer.Add(c);
                }
                else
                {
                    break;
                }
            }

            context.CharacterOnLine += charBuffer.Count;
            string symbolString = charBuffer.AsString();

            // handle keywords
            if (TryHandleKeyword(symbolString, out token))
                return true;

            // handle reserved keywords
            if (Utilities.ReservedKeywords.Contains(symbolString))
            {
                context.AddErrorMessage($"\"{symbolString}\" is a reserved keyword that is currently not implemented.", CompilerMessage.MessageCode.ReservedKeyword);
                return false;
            }

            if (TryHandleSymbols(ref context, symbolString, out token))
                return true;

            token = new Token(TokenType.UndefinedSymbol, symbolString);
            return false;
        }

        private static bool TryHandleKeyword(string symbolString, [NotNullWhen(true)] out Token? token)
        {
            bool result = Utilities.Keywords.TryGetValue(symbolString, out var keywordToken);
            token = result ? new Token(keywordToken, symbolString) : Token.InvalidToken;
            return result;
        }

        private static bool TryHandleSymbols(ref ReadingContext context, string symbolString, [NotNullWhen(true)] out Token? token)
        {
            if (!context.SymbolTable.TryGetValue(symbolString, out var symbolEntry))
            {
                symbolEntry = new SymbolTableEntry();
                context.SymbolTable.Add(symbolString, symbolEntry);
            }

            token = new Token(TokenType.Symbol, symbolString);
            symbolEntry.Appearances.Add(token);

            return true;
        }

        private static bool TryHandleNumber(ReaderWrapper input, ref ReadingContext context, [NotNullWhen(true)] out Token? token)
        {
            using var charBuffer = PooledList<char>.Rent(512);

            int numberBase = 10;
            int digitCount = 0;

            // check if the literal is hexadecimal
            if (input.MatchCharacter('0'))
            {
                charBuffer.Add('0');

                if (input.MatchCharacter('x'))
                {
                    numberBase = 16;
                    charBuffer.Add('x');
                }
                else
                    digitCount = 1;
            }

            while (true)
            {
                char c = (char)input.Peek();

                // if character is possibly valid, continue reading
                if ((c >= '0' && c <= '9') ||
                    (c >= 'a' && c <= 'f') ||
                    (c >= 'A' && c <= 'F'))
                {
                    charBuffer.Add(c);
                }
                else
                {
                    break;
                }

                input.Read();
                digitCount++;
            }

            // If the was no number to lex
            if (numberBase == 10 && digitCount == 0)
            {
                token = null;
                context.CharacterOnLine += charBuffer.Count;
                return false;
            }

            if (Utilities.IsAlphaNumeric((char)input.Peek()))
            {
                context.AddErrorMessage(new CompilerMessage($"Invalid literal structure, contains invalid character.", CompilerMessage.MessageCode.InvalidLiteral, ref context));
            }

            // there number was missing after defining a different base
            if (digitCount == 0)
            {
                context.AddErrorMessage(new CompilerMessage($"Invalid literal structure, missing digits.", CompilerMessage.MessageCode.InvalidLiteral, ref context));
            }

            // switch to floating point number handling when dealing with decimal
            if (numberBase == 10 && input.MatchCharacter('.'))
            {
                charBuffer.Add('.');
                return TryHandleFloatingPoint(charBuffer, input, ref context, out token);
            }

            Span<char> bufferSpan;
            NumberStyles numberStyles;

            if (numberBase == 10)
            {
                numberStyles = default;
                bufferSpan = charBuffer.AsSpan();
            }
            else
            {
                numberStyles = NumberStyles.AllowHexSpecifier;
                bufferSpan = charBuffer.AsSpan()[2..];
            }

            if (int.TryParse(bufferSpan, numberStyles, null, out var intResult) && intResult >= 0)
            {
                token = new Token(TokenType.IntLiteral, intResult.ToString()) { ValueType = TypeId.Int32 };
            }
            else if (long.TryParse(bufferSpan, numberStyles, null, out var longResult) && longResult >= 0)
            {
                token = new Token(TokenType.LongLiteral, longResult.ToString()) { ValueType = TypeId.Int64 };
            }
            else
            {
                context.AddErrorMessage($"Invalid literal \"{bufferSpan}\".", CompilerMessage.MessageCode.InvalidLiteral);
                context.CharacterOnLine += charBuffer.Count;
                token = null;
                return false;
            }

            context.CharacterOnLine += charBuffer.Count;
            return true;
        }

        private static bool TryHandleFloatingPoint(PooledList<char> charBuffer, ReaderWrapper input, ref ReadingContext context, out Token? token)
        {
            while (true)
            {
                char c = input.PeekChar();

                if (c >= '0' && c <= '9')
                {
                    charBuffer.Add(c);
                }
                else
                {
                    break;
                }

                input.Read();
            }

            var bufferSpan = charBuffer.AsSpan();

            if (float.TryParse(bufferSpan, out float floatResult))
            {
                token = new Token(TokenType.FloatLiteral, floatResult.ToString()) { ValueType = TypeId.Float32 };
            }
            else if (double.TryParse(bufferSpan, out double doubleResult))
            {
                token = new Token(TokenType.FloatLiteral, doubleResult.ToString()) { ValueType = TypeId.Float32 };
            }
            else
            {
                context.AddErrorMessage($"Invalid literal \"{bufferSpan}\".", CompilerMessage.MessageCode.InvalidLiteral);
                context.CharacterOnLine += charBuffer.Count;
                token = null;
                return false;
            }

            context.CharacterOnLine += charBuffer.Count;
            return true;
        }
    }
}

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

                // strip comments out before they are put into the token stream
                if (token.Type == TokenType.SingleLineComment)
                    continue;

                Token? lastToken = tokens.LastOrDefault();

                //// skip if excessive terminating tokens
                //if (lastToken != null && lastToken.Type.IsTerminatingToken() && token.Type.IsTerminatingToken())
                //    continue;

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
                        token = new Token(TokenType.Increment, ref context, 2);
                        context.CharacterOnLine += 2;
                    }
                    else if (input.MatchCharacter('='))
                    {
                        token = new Token(TokenType.AddAssign, ref context, 2);
                        context.CharacterOnLine += 2;
                    }
                    else
                    {
                        token = new Token(TokenType.Add, ref context, 1);
                        context.CharacterOnLine += 1;
                    }
                    return;

                case '-':
                    input.Read();
                    if (input.MatchCharacter('-'))
                    {
                        token = new Token(TokenType.Decrement, ref context, 2);
                        context.CharacterOnLine += 2;
                    }
                    else if (input.MatchCharacter('='))
                    {
                        token = new Token(TokenType.SubtractAssign, ref context, 2);
                        context.CharacterOnLine += 2;
                    }
                    else
                    {
                        token = new Token(TokenType.Subtract, ref context, 1);
                        context.CharacterOnLine += 1;
                    }
                    return;

                case '*':
                    input.Read();
                    if (input.MatchCharacter('='))
                    {
                        token = new Token(TokenType.MultiplyAssign, ref context, 2);
                        context.CharacterOnLine += 2;
                    }
                    else
                    {
                        token = new Token(TokenType.Multiply, ref context, 2);
                        context.CharacterOnLine += 1;
                    }
                    return;

                case '/':
                    input.Read();
                    if (input.MatchCharacter('='))
                    {
                        token = new Token(TokenType.DivideAssign, ref context, 2);
                        context.CharacterOnLine += 2;
                    }
                    else if (input.MatchCharacter('/'))
                    {
                        int commentLength = 2;

                        while (!input.PeekMatchNewLine())
                        {
                            input.Read();
                            commentLength++;
                        }
                        token = new Token(TokenType.SingleLineComment, ref context, commentLength);
                        context.CharacterOnLine += commentLength;
                    }
                    else
                    {
                        token = new Token(TokenType.Divide, ref context, 1);
                        context.CharacterOnLine += 1;
                    }
                    return;

                case '<':
                    input.Read();
                    if (input.MatchCharacter('<'))
                    {
                        if (input.MatchCharacter('='))
                        {
                            token = new Token(TokenType.LeftShiftAssign, ref context, 3);
                            context.CharacterOnLine += 3;
                        }
                        else
                        {
                            token = new Token(TokenType.LeftShift, ref context, 2);
                            context.CharacterOnLine += 2;
                        }
                    }
                    else if (input.MatchCharacter('='))
                    {
                        token = new Token(TokenType.LessThanOrEqual, ref context, 2);
                        context.CharacterOnLine += 2;
                    }
                    else
                    {
                        token = new Token(TokenType.LessThan, ref context, 1);
                        context.CharacterOnLine++;
                    }
                    return;

                case '>':
                    input.Read();
                    if (input.MatchCharacter('>'))
                    {
                        if (input.MatchCharacter('='))
                        {
                            token = new Token(TokenType.RightShiftAssign, ref context, 3);
                            context.CharacterOnLine += 3;
                        }
                        else
                        {
                            token = new Token(TokenType.RightShift, ref context, 2);
                            context.CharacterOnLine += 2;
                        }
                    }
                    else if (input.MatchCharacter('='))
                    {
                        token = new Token(TokenType.GreaterThanOrEqual, ref context, 2);
                        context.CharacterOnLine += 2;
                    }
                    else
                    {
                        token = new Token(TokenType.GreaterThan, ref context, 1);
                        context.CharacterOnLine++;
                    }
                    return;

                case '&':
                    input.Read();
                    if (input.MatchCharacter('&'))
                    {
                        token = new Token(TokenType.ConditionalAnd, ref context, 2);
                        context.CharacterOnLine += 2;
                    }
                    else if (input.MatchCharacter('='))
                    {
                        token = new Token(TokenType.AndAssign, ref context, 2);
                        context.CharacterOnLine += 2;
                    }
                    else
                    {
                        token = new Token(TokenType.And, ref context, 1);
                        context.CharacterOnLine++;
                    }
                    return;

                case '|':
                    input.Read();
                    if (input.MatchCharacter('|'))
                    {
                        token = new Token(TokenType.ConditionalOr, ref context, 2);
                        context.CharacterOnLine += 2;
                    }
                    else if (input.MatchCharacter('='))
                    {
                        token = new Token(TokenType.OrAssign, ref context, 2);
                        context.CharacterOnLine += 2;
                    }
                    else
                    {
                        token = new Token(TokenType.Or, ref context, 1);
                        context.CharacterOnLine++;
                    }
                    return;

                case '^':
                    input.Read();
                    if (input.MatchCharacter('='))
                    {
                        token = new Token(TokenType.XOrAssign, ref context, 2);
                        context.CharacterOnLine += 2;
                    }
                    else
                    {
                        token = new Token(TokenType.XOr, ref context, 2);
                        context.CharacterOnLine++;
                    }
                    return;

                case '(':
                    input.Read();
                    token = new Token(TokenType.LeftParenthesis, ref context, 1);
                    context.CharacterOnLine++;
                    return;

                case ')':
                    input.Read();
                    token = new Token(TokenType.RightParenthesis, ref context, 1);
                    context.CharacterOnLine++;
                    return;

                case '{':
                    input.Read();
                    token = new Token(TokenType.LeftCurlyBracket, ref context, 1);
                    context.CharacterOnLine++;
                    return;

                case '}':
                    input.Read();
                    token = new Token(TokenType.RightCurlyBacket, ref context, 1);
                    context.CharacterOnLine++;
                    return;

                case '=':
                    input.Read();
                    if (input.MatchCharacter('='))
                    {
                        token = new Token(TokenType.EqualsTo, ref context, 2);
                        context.CharacterOnLine += 2;
                    }
                    else
                    {
                        token = new Token(TokenType.Assign, ref context, 1);
                        context.CharacterOnLine++;
                    }
                    return;

                case '\r':
                    input.Read();
                    if (input.MatchCharacter('\n'))
                    {
                        token = new Token(TokenType.NewLine, ref context, 2);
                        context.AdvanceLine();
                    }
                    break;

                case '\n':
                    input.Read();
                    token = new Token(TokenType.NewLine, ref context, 1);
                    context.AdvanceLine();
                    return;

                case ';':
                    input.Read();
                    token = new Token(TokenType.SemiColon, ref context, 1);
                    context.CharacterOnLine++;
                    return;

                case ':':
                    input.Read();
                    token = new Token(TokenType.Colon, ref context, 1);
                    context.CharacterOnLine++;
                    return;

                case '!':
                    input.Read();
                    if (input.MatchCharacter('='))
                    {
                        token = new Token(TokenType.NotEqualsTo, ref context, 2);
                        context.CharacterOnLine += 2;
                    }
                    else
                    {
                        token = new Token(TokenType.ExclamationMark, ref context, 1);
                        context.CharacterOnLine++;
                    }
                    return;

                case '.':
                    input.Read();
                    token = new Token(TokenType.Period, ref context, 1);
                    context.CharacterOnLine++;
                    return;

                case ',':
                    input.Read();
                    token = new Token(TokenType.Comma, ref context, 1);
                    context.CharacterOnLine++;
                    return;

                case '\\':
                    input.Read();
                    if (input.MatchCharacter('\n'))
                    {
                        token = new Token(TokenType.EscapedNewLine, ref context, 2);
                        context.CharacterOnLine += 2;
                    }
                    else if (input.MatchCharacter('\\'))
                    {
                        token = new Token(TokenType.EscapedBackslash, ref context, 2);
                        context.CharacterOnLine += 2;
                    }
                    else if (input.MatchCharacter('|'))
                    {
                        token = new Token(TokenType.EscapedPipe, ref context, 2);
                        context.CharacterOnLine += 2;
                    }
                    else if (input.MatchCharacter('@'))
                    {
                        token = new Token(TokenType.EscapedAtSign, ref context, 2);
                        context.CharacterOnLine += 2;
                    }
                    else
                    {
                        token = new Token(TokenType.InvalidEscapedCharacter, ref context, input.ReadChar());
                        context.CharacterOnLine += 2;
                    }
                    return;

                case '"':
                    TryHandleStringLiteral(input, ref context, out token);
                    return;

                case '@':
                    input.Read();
                    token = new Token(TokenType.AtSign, ref context, 1);
                    context.CharacterOnLine++;
                    return;

                case Utilities.EndOfFile:
                    token = new Token(TokenType.EndOfFile, ref context, 1);
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

            // treat remaining characters as raw text

            using var charBuffer = PooledList<char>.Rent(512);
            charBuffer.Add(input.ReadChar());

            while (true)
            {
                char c = input.PeekChar();

                if (c == '\\' || c == '@' || c == ' ' || (c == '\n' || (c == '\n' && input.PeekSecondChar() == '\n')))
                    break;

                input.Read();
                charBuffer.Add(c);
            }

            token = new Token(TokenType.RawText, ref context, charBuffer.Count, charBuffer.AsString());
            context.CharacterOnLine += charBuffer.Count;

            //// handle unmatched tokens
            //input.Read();
            //token ??= new Token(TokenType.InvalidToken, ref context, 1);
            //context.CharacterOnLine++;
        }

        private static bool TryHandleStringLiteral(ReaderWrapper input, ref ReadingContext context, out Token token)
        {
            using var charBuffer = PooledList<char>.Rent(512);

            input.Read();

            int passedTokens = 1;
            while (true)
            {
                char c = input.PeekChar();
                passedTokens++;

                if (c == '"')
                    break;

                if (c == '\\')
                {
                    if (input.MatchCharacter('\\'))
                    {
                        charBuffer.Add('\\');
                        passedTokens++;
                    }
                    else if (input.MatchCharacter('n'))
                    {
                        charBuffer.Add('\n');
                        passedTokens++;
                    }
                    else if (input.MatchCharacter('"'))
                    {
                        charBuffer.Add('"');
                        passedTokens++;
                    }
                    else
                    {
                        token = new Token(TokenType.MalformedStringLiteral, ref context, passedTokens);
                        return false;
                    }
                }

                // make sure string is properly closed on a single line
                if (c == '\n' || (c == '\r' && input.PeekSecondChar() == '\n'))
                {
                    token = new Token(TokenType.MalformedStringLiteral, ref context, passedTokens);
                    return false;
                }

                charBuffer.Add(c);
                input.Read();
            }

            input.Read();

            token = new Token(TokenType.StringLiteral, ref context, passedTokens, charBuffer.AsString());

            context.CharacterOnLine += passedTokens;
            return true;
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

            string symbolString = charBuffer.AsString();

            // handle keywords
            if (TryHandleKeyword(symbolString, ref context, out token))
            {
                context.CharacterOnLine += charBuffer.Count;
                return true;
            }

            // handle reserved keywords
            if (Utilities.ReservedKeywords.Contains(symbolString))
            {
                context.AddErrorMessage($"\"{symbolString}\" is a reserved keyword that is currently not implemented.", CompilerMessage.MessageCode.ReservedKeyword);
                context.CharacterOnLine += charBuffer.Count;
                return false;
            }

            if (TryHandleSymbols(ref context, symbolString, out token))
            {
                context.CharacterOnLine += charBuffer.Count;
                return true;
            }

            token = new Token(TokenType.UndefinedSymbol, ref context, symbolString.Length, symbolString);
            context.CharacterOnLine += charBuffer.Count;
            return false;
        }

        private static bool TryHandleKeyword(string symbolString, ref ReadingContext context, [NotNullWhen(true)] out Token? token)
        {
            bool result = Utilities.Keywords.TryGetValue(symbolString, out var keywordToken);
            token = result ? new Token(keywordToken, ref context, symbolString.Length, symbolString) : null;
            return result;
        }

        private static bool TryHandleSymbols(ref ReadingContext context, string symbolString, [NotNullWhen(true)] out Token? token)
        {
            // TODO: remove symbol table operations out of the lexer

            if (!context.SymbolTable.TryGetValue(symbolString, out var symbolEntry))
            {
                symbolEntry = new SymbolTableEntry();
                context.SymbolTable.Add(symbolString, symbolEntry);
            }

            token = new Token(TokenType.Symbol, ref context, symbolString.Length, symbolString);
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
                token = new Token(TokenType.InvalidToken, ref context, charBuffer.Count);
                context.CharacterOnLine += charBuffer.Count;
                return false;
            }

            if (Utilities.IsAlphaNumeric((char)input.Peek()))
            {
                token = new Token(TokenType.InvalidToken, ref context, charBuffer.Count);
                context.AddErrorMessage(new CompilerMessage($"Invalid literal structure, contains invalid character.", CompilerMessage.MessageCode.InvalidLiteral, token));
                context.CharacterOnLine += charBuffer.Count;
                return false;
            }

            // there number was missing after defining a different base
            if (digitCount == 0)
            {
                token = new Token(TokenType.InvalidToken, ref context, charBuffer.Count);
                context.AddErrorMessage(new CompilerMessage($"Invalid literal structure, missing digits.", CompilerMessage.MessageCode.InvalidLiteral, token));
                context.CharacterOnLine += charBuffer.Count;
                return false;
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

            // TODO: handle postfixes to force specific literal types

            if (int.TryParse(bufferSpan, numberStyles, null, out var intResult) && intResult >= 0)
            {
                token = new Token(TokenType.Int32Literal, ref context, bufferSpan.Length, intResult.ToString()) { ValueType = TypeId.Int32 };
            }
            else if (long.TryParse(bufferSpan, numberStyles, null, out var longResult) && longResult >= 0)
            {
                token = new Token(TokenType.Int64Literal, ref context, bufferSpan.Length, longResult.ToString()) { ValueType = TypeId.Int64 };
            }
            else
            {
                token = new Token(TokenType.InvalidToken, ref context, charBuffer.Count);
                context.AddErrorMessage($"Invalid literal \"{bufferSpan}\".", CompilerMessage.MessageCode.InvalidLiteral, token);
                context.CharacterOnLine += charBuffer.Count;
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

            // TODO: float suffix should be handled

            if (float.TryParse(bufferSpan, out float floatResult))
            {
                token = new Token(TokenType.FloatLiteral, ref context, bufferSpan.Length, floatResult.ToString()) { ValueType = TypeId.Float32 };
            }
            else if (double.TryParse(bufferSpan, out double doubleResult))
            {
                token = new Token(TokenType.FloatLiteral, ref context, bufferSpan.Length, doubleResult.ToString()) { ValueType = TypeId.Float32 };
            }
            else
            {
                token = new Token(TokenType.InvalidToken, ref context, charBuffer.Count);
                context.AddErrorMessage($"Invalid literal \"{bufferSpan}\".", CompilerMessage.MessageCode.InvalidLiteral, token);
                context.CharacterOnLine += charBuffer.Count;
                return false;
            }

            context.CharacterOnLine += charBuffer.Count;
            return true;
        }
    }
}

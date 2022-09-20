using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Globalization;

namespace Noveler.Compiler
{
    public class Compiler
    {
        // TODO use TextReader to use streams instead in the future?
        public static bool Compile(TextReader script, out List<byte> result, out IReadOnlyList<CompilerMessage> messages)
        {
            var outMessages = new List<CompilerMessage>();
            messages = outMessages;

            var tree = Lex(script, outMessages);

            result = Emit(tree, outMessages);

            return true;
        }

        /// <summary>
        /// Analyze the raw script and turn it into a lexical structure of tokens and relations.
        /// </summary>
        /// <param name="untokenizedInput"></param>
        /// <param name="outMessages"></param>
        /// <returns></returns>
        private static SyntaxTree Lex(TextReader untokenizedInput, List<CompilerMessage> outMessages)
        {
            SyntaxTree tree = new SyntaxTree(
                new TreeNode(new Token(TokenType.Root))
                );

            TreeNode currentNode = tree.Root;
            Dictionary<string, VariableTableEntry> variableTable = new Dictionary<string, VariableTableEntry>();
            ReadingContext context = new ReadingContext(variableTable, 1, 1, null, 0);


            // tokenize the script
            while (true)
            {
                var token = ReadToken(untokenizedInput, ref context, outMessages);

                // if the token is an end of file, wrap up tokenizing. 
                if (token.Type == TokenType.EndOfFile)
                    break;

                if (token.Type == TokenType.InvalidToken)
                {
                    outMessages.Add(new CompilerMessage($"Found invalid token: {token.ValueString}", CompilerMessage.MessageCode.InvalidToken, ref context));
                    continue;
                }

                var node = new TreeNode(token);

                if (token.Type == TokenType.OpenEvaluationScope)
                {
                    context.NodeStack.Push(currentNode);

                    currentNode = TreeNode.CreateInvalidNode();
                }

                else if (token.Type == TokenType.CloseEvaluationScope)
                {
                    if (context.NodeStack.TryPop(out var poppedNode))
                    {
                        if (currentNode.Parent != null)
                        {
                            currentNode.ReplaceBy(poppedNode);
                        }
                        else
                        {
                            currentNode.InsertParent(poppedNode);
                        }
                    }
                    else
                    {
                        outMessages.Add(new CompilerMessage($"Unbalanced parentheses, extra \')\' found.", CompilerMessage.MessageCode.InvalidToken, ref context));
                    }
                }

                else if (token.Type.IsValueToken())
                {
                    currentNode.InsertChild(node);

                    currentNode = node;
                }



                if (token.Type.IsOperationToken())
                {
                    currentNode.InsertParent(node);

                    // read next token to make sure the operation is in order
                    var value = ReadToken(untokenizedInput, ref context, outMessages);
                    if (value.Type.IsValueToken())
                    {
                        node.InsertChild(new TreeNode(value));
                        currentNode = node;
                    }
                    else
                    {
                        outMessages.Add(new CompilerMessage($"Expected value, found: {value.ValueString}", CompilerMessage.MessageCode.InvalidToken, ref context));
                    }

                    // TODO constant folding here?


                }



            }


            return tree;
        }

        private static List<byte> Emit(SyntaxTree tree, List<CompilerMessage> outMessages)
        {
            List<byte> result;

            if (!tree.IsValid())
            {
                return new List<byte>(0);
            }

            result = new List<byte>(2048);

            // plunge the tree until the end has been reached.


            return result;
        }



        private static Token ReadToken(TextReader input, ref ReadingContext context, List<CompilerMessage> outMessages)
        {
            context.CharacterOnLine += Utilities.SkipSpace(input);

            //// for now only parse a single character as a token. TODO change this
            //Token token = input[0] switch
            //{
            //    >= '0' and <= '9' => new Token(TokenType.IntLiteral, input[0].ToString()),
            //    '+' => new Token(TokenType.Plus),
            //    '-' => new Token(TokenType.Minus),
            //    _ => new Token(TokenType.InvalidToken, input[0].ToString())
            //};



            ////todo get correct token length
            //tokenLength = 1;

            LexIntoToken(input, ref context, outMessages, out Token token);

            return token;
        }

        private static void LexIntoToken(TextReader input, ref ReadingContext context, List<CompilerMessage> outMessages, [NotNull] out Token? token)
        {
            token = default;
            char firstChar = (char)input.Peek();
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
                    if (TryHandleNumber(input, ref context, outMessages, out var numberToken))
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
                        token = new Token(TokenType.AssignAdd);
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
                        token = new Token(TokenType.AssignSubtract);
                    }
                    else
                    {
                        context.CharacterOnLine += 1;
                        token = new Token(TokenType.Subtract);
                    }
                    return;

                case unchecked((char)-1):
                    token = new Token(TokenType.EndOfFile);
                    return;

                default:
                    break;
            }

            // handle symbols and keywords
            if (Utilities.IsAlpha(firstChar))
            {

            }

            // handle unmatched tokens
            token ??= Token.InvalidToken;
        }

        private static bool TryHandleNumber(TextReader input, ref ReadingContext context, List<CompilerMessage> outMessages, [NotNullWhen(true)] out Token? token)
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

            if (Utilities.IsAplhaNumeric((char)input.Peek()))
            {
                outMessages.Add(new CompilerMessage($"Invalid literal structure, contains invalid character.", CompilerMessage.MessageCode.InvalidLiteral, ref context));
            }

            // there number was missing after defining a different base
            if (digitCount == 0)
            {
                outMessages.Add(new CompilerMessage($"Invalid literal structure, missing digits.", CompilerMessage.MessageCode.InvalidLiteral, ref context));
            }

            // switch to floating point number handling when dealing with decimal
            if (numberBase == 10 && input.MatchCharacter('.'))
            {
                return TryHandleFloatingPoint(charBuffer, input, ref context, outMessages, out token);
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
                token = new Token(TokenType.IntLiteral, intResult.ToString());
            }
            else if (long.TryParse(bufferSpan, numberStyles, null, out var longResult) && longResult >= 0)
            {
                token = new Token(TokenType.LongLiteral, longResult.ToString());
            }
            else
            {
                outMessages.Add(new CompilerMessage($"Invalid literal \"{bufferSpan}\", outside of valid range.", CompilerMessage.MessageCode.InvalidLiteral, ref context));
                context.CharacterOnLine += charBuffer.Count;
                token = null;
                return false;
            }

            context.CharacterOnLine += charBuffer.Count;
            return true;
        }

        private static bool TryHandleFloatingPoint(PooledList<char> charBuffer, TextReader input, ref ReadingContext context, List<CompilerMessage> outMessages, out Token? token)
        {
            // TODO: implement
            token = null;
            return false;
        }
    }

    enum ReadState
    {
        Story,
        Code,
        EmbeddedCode
    }
}
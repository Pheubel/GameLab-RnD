using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Globalization;
using NovelerCompiler;
using System.CommandLine.Parsing;

namespace Noveler.Compiler
{
    public class Compiler
    {
        // TODO use TextReader to use streams instead in the future?
        public static bool Compile(TextReader script, out List<byte> result, out IReadOnlyList<CompilerMessage> messages)
        {
            var reader = new ReaderWrapper(script);

            var outMessages = new List<CompilerMessage>();
            messages = outMessages;

            var tree = Lex(reader, outMessages);

            result = EmitTree(tree, outMessages);

            return true;
        }

        /// <summary>
        /// Analyze the raw script and turn it into a lexical structure of tokens and relations.
        /// </summary>
        /// <param name="untokenizedInput"></param>
        /// <param name="outMessages"></param>
        /// <returns></returns>
        private static SyntaxTree Lex(ReaderWrapper untokenizedInput, List<CompilerMessage> outMessages)
        {
            SyntaxTree tree = new SyntaxTree(
                new TreeNode(new Token(TokenType.Root))
                );

            TreeNode currentNode = tree.Root;
            Dictionary<string, VariableTableEntry> variableTable = new Dictionary<string, VariableTableEntry>();
            ReadingContext context = new ReadingContext(variableTable, 1, 1, null, 0, outMessages);


            // tokenize the script
            while (true)
            {
                var token = ReadToken(untokenizedInput, ref context);

                // if the token is an end of file, wrap up tokenizing. 
                if (token.Type == TokenType.EndOfFile)
                    break;

                if (token.Type == TokenType.InvalidToken)
                {

                    context.AddErrorMessage(new CompilerMessage($"Found invalid token: {token.ValueString}", CompilerMessage.MessageCode.InvalidToken, ref context));
                    continue;
                }

                var node = new TreeNode(token);

                // check if the +/- token should be transformed
                if (!currentNode.Token.Type.IsValueToken() && !currentNode.Token.Type.IsOperationToken())
                {
                    // the token can be skipped as it does not cause a transform
                    if (token.Type == TokenType.Add)
                        continue;
                    if (token.Type == TokenType.Subtract)
                    {
                        token.Type = TokenType.Negate;

                        // read next token to make sure the operation is in order
                        var value = ReadToken(untokenizedInput, ref context);
                        if (value.Type.IsValueToken())
                        {
                            node.AddChild(new TreeNode(value));
                            currentNode = node;
                        }
                        else if (value.Type == TokenType.OpenEvaluationScope)
                        {
                            context.NodeStack.Push(node);

                            currentNode = TreeNode.CreateInvalidNode();
                        }
                        else
                        {
                            context.AddErrorMessage(new CompilerMessage($"Cannot negate value, found: {value.ValueString}", CompilerMessage.MessageCode.InvalidToken, ref context));
                        }
                    }
                }

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
                            currentNode.ReplaceParent(poppedNode);
                        }
                        else
                        {
                            currentNode.InsertParent(poppedNode);
                        }
                        poppedNode.Token.ValueType = Utilities.GetTargetType(currentNode.Token.ValueType, poppedNode.Token.ValueType);
                    }
                    else
                    {
                        context.AddErrorMessage(new CompilerMessage($"Unbalanced parentheses, extra \')\' found.", CompilerMessage.MessageCode.InvalidToken, ref context));
                    }
                }

                else if (token.Type.IsValueToken())
                {
                    currentNode.AddChild(node);

                    currentNode = node;
                }

                if (token.Type.IsExpressionToken())
                {
                    currentNode.InsertParent(node);

                    // read next token to make sure the operation is in order
                    var value = ReadToken(untokenizedInput, ref context);
                    if (value.Type.IsValueToken())
                    {
                        token.ValueType = Utilities.GetTargetType(currentNode.Token.ValueType, value.ValueType);
                        node.AddChild(new TreeNode(value));
                        currentNode = node;
                    }
                    else if (value.Type == TokenType.OpenEvaluationScope)
                    {
                        node.Token.ValueType = currentNode.Token.ValueType;
                        context.NodeStack.Push(node);

                        currentNode = TreeNode.CreateInvalidNode();
                    }
                    else
                    {
                        context.AddErrorMessage(new CompilerMessage($"Expected value, found: {value.ValueString}", CompilerMessage.MessageCode.InvalidToken, ref context));
                    }
                }

                if (token.Type.IsFactorToken())
                {
                    // todo test this
                    if (currentNode.Token.Type.IsExpressionToken())
                    {
                        currentNode.ReplaceInParent(node);
                        node.AddChild(currentNode);
                    }
                    else
                    {
                        currentNode.InsertParent(node);
                    }

                    // read next token to make sure the operation is in order
                    var value = ReadToken(untokenizedInput, ref context);
                    if (value.Type.IsValueToken())
                    {
                        token.ValueType = Utilities.GetTargetType(currentNode.Token.ValueType, value.ValueType);
                        node.AddChild(new TreeNode(value));
                        currentNode = node;
                    }
                    else if (value.Type == TokenType.OpenEvaluationScope)
                    {
                        node.Token.ValueType = currentNode.Token.ValueType;
                        context.NodeStack.Push(node);

                        currentNode = TreeNode.CreateInvalidNode();
                    }
                    else
                    {
                        context.AddErrorMessage(new CompilerMessage($"Expected value, found: {value.ValueString}", CompilerMessage.MessageCode.InvalidToken, ref context));
                    }
                }



            }


            return tree;
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
                    token = new Token(TokenType.OpenEvaluationScope);
                    return;

                case ')':
                    input.Read();
                    context.CharacterOnLine++;
                    token = new Token(TokenType.CloseEvaluationScope);
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

                break;
            }

            context.CharacterOnLine += charBuffer.Count;
            string symbolString = charBuffer.AsString();

            // determine if this is a function declaration
            if (input.MatchCharacter('('))
            {
                context.CharacterOnLine++;
                //TODO: handle function start declaration
                token = new Token(TokenType.FunctionDeclaration, symbolString);
                return false;
            }

            // handle keywords
            if (TryHandleKeyword(symbolString, out token))
                return true;

            // handle reserved keywords
            if (Utilities.ReservedKeywords.Contains(symbolString))
            {
                context.AddErrorMessage($"\"{symbolString}\" is a reserved keyword that is currently not implemented.", CompilerMessage.MessageCode.ReservedKeyword);
                return false;
            }

            // TODO: allow multiple forward declaration?
            if (TryHandleSymbols(input, ref context, symbolString, out token))
                return true;

            //context.AddErrorMessage(new CompilerMessage("Cannot "));
            token = new Token(TokenType.UndefinedSymbol, symbolString);
            return false;
        }

        private static bool TryHandleKeyword(string symbolString, [NotNullWhen(true)] out Token? token)
        {
            bool result = Utilities.Keywords.TryGetValue(symbolString, out var keywordToken);
            token = result ? new Token(keywordToken, symbolString) : Token.InvalidToken;
            return result;
        }

        private static bool TryHandleSymbols(ReaderWrapper input, ref ReadingContext context, string symbolString, [NotNullWhen(true)] out Token? token)
        {
            context.CharacterOnLine += Utilities.SkipSpace(input);

            // determine if the variable should be declared
            if (Utilities.MatchCharacter(input, ':'))
            {
                context.CharacterOnLine++;

                // handle if variable already exists
                if (!context.VariableTable.TryAdd(symbolString, new VariableTableEntry()))
                {
                    context.AddErrorMessage("Cannot re-declare a variable.", CompilerMessage.MessageCode.RedeclaredVariable);
                    token = new Token(TokenType.InvalidToken, $"Reclared variable ({symbolString}).");
                    return false;
                }

                // variable will get it's type from future keywords
                token = new Token(TokenType.ValueVariable, symbolString);

                return true;
            }

            // the variable should already be declared
            if (!context.VariableTable.TryGetValue(symbolString, out VariableTableEntry? tableEntry))
            {
                context.AddErrorMessage($"\"{symbolString}\" has not been declared yet. Make sure you declare", CompilerMessage.MessageCode.UndefinedVariable);
                token = new Token(TokenType.UndeclaredVariable, symbolString);
                return false;
            }

            var valueType = (InternalType)tableEntry.GetTypeId();
            token = valueType switch
            {
                InternalType.Int32 => new Token(TokenType.IntValue),
                InternalType.Int64 => new Token(TokenType.LongLiteral),
                InternalType.Float32 => new Token(TokenType.FloatValue),
                InternalType.Float64 => new Token(TokenType.DoubleValue),

                // should not happen
                InternalType.Undeclared => throw new NotImplementedException(),

                // the type is not a language standard type
                _ => new Token(TokenType.CustomType)
            };

            token.ValueType = valueType;

            tableEntry.Appearances.Add(token);

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
                token = new Token(TokenType.IntLiteral, intResult.ToString()) { ValueType = InternalType.Int32 };
            }
            else if (long.TryParse(bufferSpan, numberStyles, null, out var longResult) && longResult >= 0)
            {
                token = new Token(TokenType.LongLiteral, longResult.ToString()) { ValueType = InternalType.Int64 };
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
                token = new Token(TokenType.FloatLiteral, floatResult.ToString()) { ValueType = InternalType.Float32 };
            }
            else if (double.TryParse(bufferSpan, out double doubleResult))
            {
                token = new Token(TokenType.FloatLiteral, doubleResult.ToString()) { ValueType = InternalType.Float32 };
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

        private static List<byte> EmitTree(SyntaxTree tree, List<CompilerMessage> outMessages)
        {
            List<byte> result;

            if (!tree.IsValid())
            {
                return new List<byte>(0);
            }

            result = new List<byte>(2048);

            EmitCode(tree.Root);

            return result;
        }

        private static byte EmitCode(TreeNode node)
        {
            switch (node.Token.Type)
            {
                case TokenType.InvalidToken:
                    break;
                case TokenType.IntLiteral:
                    Console.WriteLine($"LoadConst32 R0 {node.Token.ValueString:X}");
                    break;
                case TokenType.LongLiteral:
                    Console.WriteLine($"LoadConst64 R0 {node.Token.ValueString:X}");
                    break;
                case TokenType.FloatLiteral:
                    break;
                case TokenType.DoubleLiteral:
                    break;

                case TokenType.Add:
                    EmitCode(node.Children[0]);
                    Console.WriteLine("PUSH R1");
                    EmitCode(node.Children[1]);
                    Console.WriteLine("MOVE R2 R1");
                    Console.WriteLine("POP R1");
                    Console.WriteLine("ADD R1 R2");
                    break;

                case TokenType.Subtract:
                    EmitCode(node.Children[0]);
                    Console.WriteLine("PUSH R1");
                    EmitCode(node.Children[1]);
                    Console.WriteLine("MOVE R2 R1");
                    Console.WriteLine("POP R1");
                    Console.WriteLine("SUBTRACT R1 R2");
                    break;

                case TokenType.Multiply:
                    EmitCode(node.Children[0]);
                    Console.WriteLine("PUSH R1");
                    EmitCode(node.Children[2]);
                    Console.WriteLine("MULTIPLY R1 R2");
                    break;

                case TokenType.Divide:
                    EmitCode(node.Children[0]);
                    Console.WriteLine("PUSH R1");
                    EmitCode(node.Children[2]);
                    Console.WriteLine("DIVREM R1 R2");
                    break;

                case TokenType.Assign:
                    break;
                case TokenType.AssignAdd:
                    break;
                case TokenType.AssignSubtract:
                    break;
                case TokenType.Compare:
                    break;
                case TokenType.Increment:
                    break;
                case TokenType.Decrement:
                    break;
                case TokenType.FunctionName:
                    break;
                case TokenType.OpenFunction:
                    break;
                case TokenType.CloseFunction:
                    break;
                case TokenType.SemiColon:
                    break;
                case TokenType.ClosingCurlyBracket:
                    break;
                case TokenType.ValueVariable:
                    break;
                case TokenType.Root:
                    EmitCode(node.Children[0]);
                    break;
                case TokenType.EndOfLine:
                    break;
                case TokenType.EndOfFile:
                    break;
                case TokenType.Negate:
                    Console.WriteLine("NEGATE R1");
                    break;
                default:
                    break;
            }

            Console.WriteLine(node.Token.ValueType);

            return default;
        }
    }

    enum ReadState
    {
        Story,
        Code,
        EmbeddedCode
    }
}
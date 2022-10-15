using Noveler.Compiler;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovelerCompiler
{
    public static class Lexer
    {
        /// <summary>
        /// Analyze the raw script and turn it into a lexical structure of tokens and relations.
        /// </summary>
        /// <param name="untokenizedInput"></param>
        /// <param name="outMessages"></param>
        /// <returns></returns>
        public static SyntaxTree Lex(TextReader untokenizedInput, List<CompilerMessage> outMessages) =>
            Lex(new ReaderWrapper(untokenizedInput), outMessages);

        internal static SyntaxTree Lex(ReaderWrapper untokenizedInput, List<CompilerMessage> outMessages)
        {
            SyntaxTree tree = new SyntaxTree(
                new TreeNode(new Token(TokenType.Root))
                );

            TreeNode currentNode = tree.Root;
            Dictionary<string, SymbolTableEntry> variableTable = new Dictionary<string, SymbolTableEntry>();
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


                if (token.Type.IsTerminatingToken())
                {
                    // if the current token is also an expression termination, then it can be skipped
                    if (currentNode.Token.Type.IsTerminatingToken())
                        continue;

                    // TODO: handle when a expression is completed

                    currentNode = node;
                }

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
                            currentNode.AddChild(node);

                            currentNode = node;
                        }
                        else if (value.Type == TokenType.OpenEvaluationScope)
                        {
                            currentNode.AddChild(node);
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
                    token = new Token(TokenType.OpenEvaluationScope);
                    return;

                case ')':
                    input.Read();
                    context.CharacterOnLine++;
                    token = new Token(TokenType.CloseEvaluationScope);
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
                    token = new Token(TokenType.SemiColon);
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
                if (!context.SymbolTable.TryAdd(symbolString, new SymbolTableEntry() { Kind = SymbolKind.Variable }))
                {
                    context.AddErrorMessage("Cannot re-declare a variable.", CompilerMessage.MessageCode.RedeclaredVariable);
                    token = new Token(TokenType.InvalidToken, $"Reclared variable ({symbolString}).");
                    return false;
                }

                // determine the type for the variable

                token = new Token(TokenType.ValueVariable, symbolString);
                NumberModifier setFlags = 0;

                using PooledList<char> typeBuffer = PooledList<char>.Rent(128);
                while (true)
                {
                    typeBuffer.Clear();

                    context.CharacterOnLine += Utilities.SkipSpace(input);

                    // read in characters for type information
                    while (true)
                    {
                        char c = input.PeekChar();

                        if (Utilities.IsAlphaNumeric(c))
                        {
                            input.Read();

                            typeBuffer.Add(c);
                        }
                        else
                        {
                            break;
                        }
                    }

                    context.CharacterOnLine += typeBuffer.Count;

                    var typeContextString = typeBuffer.AsString();

                    if (!Utilities.Keywords.TryGetValue(typeContextString, out TokenType tokenType))
                    {
                        if (string.IsNullOrEmpty(typeContextString))
                        {
                            context.AddErrorMessage("Missing type when declaring variable", CompilerMessage.MessageCode.MissingType);
                            return false;
                        }

                        // TODO: remove this error when custom types are supported
                        token.ValueType = TypeId.Undeclared;
                        context.AddErrorMessage("Custom types are currently not supported.", CompilerMessage.MessageCode.InvalidToken);
                        break;

                        if (context.SymbolTable.TryGetValue(typeContextString, out var symbolEntry))
                        {
                            if (symbolEntry.Kind != SymbolKind.Unknown && symbolEntry.Kind != SymbolKind.Type)
                            {
                                // TODO: handle invalid symbol kinds
                            }
                        }
                        else
                        {
                            symbolEntry = new SymbolTableEntry();
                            context.SymbolTable.Add(typeContextString, symbolEntry);
                        }

                        symbolEntry.Appearances.Add(token);

                        break;
                    }

                    #region Modifier Determination

                    // determine the size modifier
                    if (tokenType == TokenType.KeywordBig)
                    {
                        // TODO: handle conflicting size modifier
                        if (setFlags.HasFlag(NumberModifier.HasDefinedSize))
                        {

                        }

                        setFlags |= NumberModifier.Is64Bit;
                        setFlags |= NumberModifier.HasDefinedSize;
                    }
                    else if (tokenType == TokenType.KeywordSmall)
                    {
                        // TODO: handle conflicting size modifier
                        if (setFlags.HasFlag(NumberModifier.HasDefinedSize))
                        {

                        }

                        setFlags |= NumberModifier.Is16Bit;
                        setFlags |= NumberModifier.HasDefinedSize;
                    }
                    else if (tokenType == TokenType.KeywordTiny)
                    {
                        // TODO: handle conflicting size modifier
                        if (setFlags.HasFlag(NumberModifier.HasDefinedSize))
                        {

                        }

                        setFlags |= NumberModifier.Is8Bit;
                        setFlags |= NumberModifier.HasDefinedSize;
                    }

                    // determine the type modifier
                    else if (tokenType == TokenType.KeywordWhole)
                    {
                        // TODO: handle conflicting integer modifier
                        if (setFlags.HasFlag(NumberModifier.HasDefinedFloatInt))
                        {

                        }

                        setFlags |= NumberModifier.IsInteger;
                        setFlags |= NumberModifier.HasDefinedFloatInt;
                    }

                    // determine the sign modifier
                    else if (tokenType == TokenType.KeywordSigned)
                    {
                        // TODO: handle conflicting integer modifier
                        if (setFlags.HasFlag(NumberModifier.HasDefinedSigned))
                        {

                        }

                        setFlags |= NumberModifier.IsSigned;
                        setFlags |= NumberModifier.HasDefinedSigned;
                    }
                    else if (tokenType == TokenType.KeywordUnsigned)
                    {
                        // TODO: handle conflicting integer modifier
                        if (setFlags.HasFlag(NumberModifier.HasDefinedSigned))
                        {

                        }

                        setFlags |= NumberModifier.IsUnsigned;
                        setFlags |= NumberModifier.HasDefinedSigned;
                    }

                    #endregion

                    // take a look at the set context for the number
                    else if (tokenType == TokenType.KeywordNumber)
                    {
                        // determine if the number is an integer
                        if (setFlags.HasFlag(NumberModifier.IsInteger))
                        {
                            // hack to get the right integer type quickly
                            token.ValueType = (TypeId)(((uint)(setFlags & NumberModifier.SizeMask) >> 1) + (setFlags.HasFlag(NumberModifier.IsUnsigned) ? 2 : 1));
                        }
                        else
                        {
                            // TODO: floats should not have signed modifiers
                            if (setFlags.HasFlag(NumberModifier.IsUnsigned))
                            {
                                token.ValueType = TypeId.Undeclared;
                            }

                            // TODO: float should not be smaller than 32 bit
                            if (setFlags.HasFlag(NumberModifier.Is16Bit))
                            {
                                token.ValueType = TypeId.Undeclared;
                            }

                            token.ValueType = setFlags.HasFlag(NumberModifier.Is64Bit) ? TypeId.Float64 : TypeId.Float32;
                        }

                        break;
                    }

                    // invalid keyword found
                    else
                    {
                        // TODO: handle case
                    }
                }

                return true;
            }

            // the variable should already be declared
            if (!context.SymbolTable.TryGetValue(symbolString, out SymbolTableEntry? tableEntry))
            {
                context.AddErrorMessage($"\"{symbolString}\" has not been declared yet. Make sure you declare", CompilerMessage.MessageCode.UndefinedVariable);
                token = new Token(TokenType.UndeclaredVariable, symbolString);
                return false;
            }

            var valueType = (TypeId)tableEntry.GetTypeId();

            // this should not happen
            if (valueType == TypeId.Undeclared)
                throw new InvalidOperationException();

            token = new Token(TokenType.ValueVariable)
            {
                ValueType = valueType
            };

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

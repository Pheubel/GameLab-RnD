using Noveler.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NovelerCompiler
{
    internal static partial class SyntaxAnalyzer
    {

        public static void Test(List<Token> tokenStream)
        {
            var tokenStreamSpan = CollectionsMarshal.AsSpan(tokenStream);
            //var result = DeclarationStatementGrammar.MatchesSequence(tokenStreamSpan, out var readCount, out ParseTreeNode? parseTree);

            if(StoryEmbeddedIfStatementGrammar.MatchesSequence(tokenStreamSpan, out var readCount, out ParseTreeNode? parseTree))
            {
                Console.WriteLine(parseTree.VisualizeTree());
                Console.WriteLine();
                Console.WriteLine($"read tokens: {readCount}");
            }
            else
            {
                Console.WriteLine("No tokens read");
            }
        }















        // TODO: move to syntax analyzer
        //// determine if this is a function declaration
        //if (input.MatchCharacter('('))
        //{
        //    context.CharacterOnLine++;
        //    //TODO: handle function start declaration
        //    token = new Token(TokenType.FunctionDeclaration, symbolString);
        //    return false;
        //}

        /*
        public static SyntaxTree Analyze(List<Token> tokenStream, ref ReadingContext context)
        {
            while (true)
            {
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
        }
        */
    }
}

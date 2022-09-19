using System.ComponentModel;
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

            var tree = Lex(script, outMessages);

            var emitedBytes = Emit(tree, outMessages);

            return isValid;
        }

        /// <summary>
        /// Analyze the raw script and turn it into a lexical structure of tokens and relations.
        /// </summary>
        /// <param name="untokenizedInput"></param>
        /// <param name="outMessages"></param>
        /// <returns></returns>
        private static SyntaxTree Lex(ReadOnlySpan<char> untokenizedInput, List<CompilerMessage> outMessages)
        {
            SyntaxTree tree = new SyntaxTree(
                new TreeNode(new Token(TokenType.Root))
                );

            TreeNode currentNode = tree.Root;
            Dictionary<string, VariableTableEntry> variableTable = new Dictionary<string, VariableTableEntry>();
            ReadingContext context = new ReadingContext(variableTable, 1, 1, null, 0);

            // tokenize the script
            while (!untokenizedInput.IsWhiteSpace())
            {
                var token = ReadToken(ref untokenizedInput, context, out var tokenLength);

                // if the token is an end of file, wrap up tokenizing. 
                if (token.Type == TokenType.EndOfFile)
                    break;

                if (token.Type == TokenType.InvalidToken)
                {
                    outMessages.Add(new CompilerMessage($"Found invalid token: {token.ValueString}", CompilerMessage.MessageCode.InvalidToken, context));
                    continue;
                }

                var node = new TreeNode(token);

                if(token.Type == TokenType.OpenEvaluationScope)
                {

                }

                else if (token.Type.IsValueToken())
                {
                    currentNode.AddChild(node);

                    currentNode = node;
                }






                if (token.Type.IsOperationToken())
                {
                    currentNode.AddParent(node);

                    // read next token to make sure the operation is in order
                    var value = ReadToken(ref untokenizedInput, context, out var valueTokenLength);
                    if (value.Type.IsValueToken())
                    {
                        node.AddChild(new TreeNode(value));
                        currentNode = node;
                    }
                    else
                    {
                        outMessages.Add(new CompilerMessage($"Expected value, found: {value.ValueString}", CompilerMessage.MessageCode.InvalidToken, context));
                    }

                    // TODO constant folding here?


                }



            }


            return tree;
        }

        private static IReadOnlyList<byte> Emit(SyntaxTree tree, List<CompilerMessage> outMessages)
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



        private static Token ReadToken(ref ReadOnlySpan<char> input, ReadingContext context, out int tokenLength)
        {
            context.LineNumber += Utilities.SkipSpace(ref input);

            // for now only parse a single character as a token. TODO change this
            Token token = input[0] switch
            {
                >= '0' and <= '9' => new Token(TokenType.IntLiteral, input[0].ToString()),
                '+' => new Token(TokenType.Plus),
                '-' => new Token(TokenType.Minus),
                _ => new Token(TokenType.InvalidToken, input[0].ToString())
            };

            //todo get correct token length
            tokenLength = 1;

            return token;
        }
    }

    internal class SyntaxTree
    {
        public TreeNode Root { get; }

        public SyntaxTree(TreeNode root)
        {
            Root = root;
        }

        public bool IsValid()
        {
            // TODO validate
            return true;
        }
    }

    internal class TreeNode
    {
        public Token Token { get; set; }
        public List<TreeNode> Children { get; }
        public TreeNode? Parent { get; private set; }

        public TreeNode(Token token)
        {
            Token = token;
            Children = new List<TreeNode>();
        }

        public static TreeNode InvalidNode() =>
            new TreeNode(new Token(TokenType.InvalidToken));

        /// <summary>
        /// Insert a node as the rightmost child.
        /// </summary>
        /// <param name="childNode"></param>
        public void AddChild(TreeNode childNode)
        {
            childNode.Parent = this;
            Children.Add(childNode);
        }

        public void AddParent(TreeNode parentNode)
        {
            var oldParent = this.Parent;
            if (oldParent != null)
            {
                oldParent.Children.Remove(this);
                oldParent.AddChild(parentNode);
            }

            parentNode.AddChild(this);
        }

        public void PlungeChildren()
        {

        }
    }
}
using Noveler.Compiler;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NovelerCompiler
{
    [DebuggerDisplay("Node ({Kind}: {GrammarKind})")]
    internal class ParseTreeNode
    {
        public NodeKind Kind { get; }
        public GrammarKind GrammarKind { get; }
        public List<ParseTreeNode>? Children { get; }
        [DisallowNull]
        public Token? Token { get; set; }


        public ParseTreeNode(NodeKind kind, GrammarKind grammarKind = GrammarKind.NotAValidGrammar)
        {
            Kind = kind;
            GrammarKind = grammarKind;

            if (kind != NodeKind.Token)
                Children = new List<ParseTreeNode>();
            else
            {
                GrammarKind = GrammarKind.Token;
            }
        }

        /// <summary>
        /// Determine if the given node should be added to the children or if its children should be addopted by this node.
        /// </summary>
        /// <param name="node"> The node to check.</param>
        public void AddNodeOrAdoptChildren(ParseTreeNode node)
        {
            if (Kind == NodeKind.Token)
                throw new InvalidOperationException("Cannot add children to token nodes");

            switch (node.Kind)
            {
                case NodeKind.Grammar:
                case NodeKind.Token:
                    Children!.Add(node);
                    break;
                case NodeKind.Sequence:
                    Children!.AddRange(node.Children!);
                    break;
            }
        }

        public string VisualizeTree()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{GrammarKind}{(Token != null ? $": {Token.Type}" : string.Empty)}");

            int childCount = Children?.Count ?? 0;
            for (var i = 0; i < childCount; i++)
            {
                Children![i].VisualizeTree(sb, string.Empty, (i == (childCount - 1)));
            }

            return sb.ToString();
        }

        private void VisualizeTree(StringBuilder output, string indent, bool isLast)
        {
            output.Append(indent);

            if (isLast)
            {
                output.Append("└─");
                indent += "  ";
            }
            else
            {
                output.Append("├─");
                indent += "│ ";
            }

            output.AppendLine($"{GrammarKind}{(Token != null ? $": {Token.Type}" : string.Empty)}");

            var numberOfChildren = Children?.Count ?? 0;
            for (var i = 0; i < numberOfChildren; i++)
            {
                Children![i].VisualizeTree(output, indent, (i == (numberOfChildren - 1)));
            }
        }

        public enum NodeKind
        {
            Grammar,
            Sequence,
            Token
        }
    }
}

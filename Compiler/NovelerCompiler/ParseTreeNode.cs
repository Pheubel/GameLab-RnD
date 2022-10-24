using Microsoft.VisualBasic;
using Noveler.Compiler;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NovelerCompiler
{
    [DebuggerDisplay("Node ({Kind})")]
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

        public enum NodeKind
        {
            Grammar,
            Sequence,
            Token
        }
    }

    public enum GrammarKind
    {
        NotAValidGrammar
    }
}

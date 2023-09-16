namespace Noveler.Compiler.SyntaxTree
{
    internal interface IOperationNode : ISyntaxTreeNode
    {
        ISyntaxTreeNode Target { get; }

        bool ISyntaxTreeNode.IsLeaf => false;
    }
}

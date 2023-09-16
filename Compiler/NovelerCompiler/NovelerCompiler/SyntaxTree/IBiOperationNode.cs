namespace Noveler.Compiler.SyntaxTree
{
    internal interface IBiOperationNode : IOperationNode
    {
        ISyntaxTreeNode LeftSide { get; }
        ISyntaxTreeNode RightSide { get; }

        ISyntaxTreeNode IOperationNode.Target => LeftSide;
    }
}

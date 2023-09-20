namespace Noveler.Compiler.SyntaxTree
{
    internal interface ISyntaxTreeNode
    {
        bool IsLeaf { get; }

        void EmitCode(List<byte> output);
    }
}

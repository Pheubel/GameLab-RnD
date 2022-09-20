namespace Noveler.Compiler
{
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
}
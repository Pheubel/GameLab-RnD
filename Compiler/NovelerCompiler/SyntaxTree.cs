namespace Noveler.Compiler
{
    public class SyntaxTree
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
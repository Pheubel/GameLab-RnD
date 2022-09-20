namespace Noveler.Compiler
{
    internal class TreeNode
    {
        public Token Token { get; set; }
        public List<TreeNode> Children { get; private set; }
        public TreeNode? Parent { get; private set; }

        public TreeNode(Token token)
        {
            Token = token;
            Children = new List<TreeNode>();
        }

        public static TreeNode CreateInvalidNode() =>
            new TreeNode(new Token(TokenType.InvalidToken));

        /// <summary>
        /// Insert a node as the rightmost child.
        /// </summary>
        /// <param name="childNode"></param>
        public void InsertChild(TreeNode childNode)
        {
            childNode.Parent = this;
            Children.Add(childNode);
        }

        public void InsertParent(TreeNode parentNode)
        {
            var oldParent = this.Parent;
            if (oldParent != null)
            {
                oldParent.Children.Remove(this);
                oldParent.InsertChild(parentNode);
            }

            parentNode.InsertChild(this);
        }

        public void ReplaceBy(TreeNode replacement)
        {
            if (this.Parent != null)
            {
                var index = this.Parent.Children.IndexOf(this);
                this.Parent.Children[index] = replacement;
            }

            foreach (var child in Children)
            {
                child.Parent = replacement;
                replacement.Children = this.Children;
            }
        }
    }
}
using System.Diagnostics;

namespace Noveler.Compiler
{
    [DebuggerDisplay("Node (Token:{Token.Type})")]
    public class TreeNode
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
        public void AddChild(TreeNode childNode)
        {
            childNode.Parent = this;
            Children.Add(childNode);
        }

        public void AddChildLeft(TreeNode childNode)
        {
            childNode.Parent = this;
            Children.Insert(0, childNode);
        }

        public void InsertParent(TreeNode parentNode)
        {
            var oldParent = this.Parent;
            if (oldParent != null)
            {
                int index = oldParent.Children.IndexOf(this);

                oldParent.Children[index] = parentNode;
                parentNode.Parent = oldParent;
            }

            parentNode.AddChild(this);
        }

        public void ReplaceInParent(TreeNode replacement)
        {
            if (this.Parent != null)
            {
                var index = this.Parent.Children.IndexOf(this);
                this.Parent.Children[index] = replacement;
                replacement.Parent = this.Parent;
                this.Parent = null;
            }
        }

        public void ReplaceParent(TreeNode replacement)
        {
            this.Parent?.Children.Remove(this);
            replacement.AddChild(this);
        }
    }
}
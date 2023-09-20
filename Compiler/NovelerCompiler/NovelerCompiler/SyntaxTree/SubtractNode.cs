using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noveler.Compiler.SyntaxTree
{
    internal class SubtractNode : IBiOperationNode
    {
        public required ISyntaxTreeNode LeftSide { get; set; }

        public required ISyntaxTreeNode RightSide { get; set; }
    }
}

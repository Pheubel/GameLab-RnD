using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Noveler.Compiler.SyntaxTree
{
    internal interface IConstantValue<T> : ISyntaxTreeNode
        where T : struct
    {
        public T Value { get; }
        public TypeDefinition Type { get; }

        bool ISyntaxTreeNode.IsLeaf => true;
    }
}

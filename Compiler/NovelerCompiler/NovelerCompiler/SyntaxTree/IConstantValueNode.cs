using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Noveler.Compiler.SyntaxTree
{
    internal interface IValueNode : ISyntaxTreeNode
    {
        object Value { get; }
        TypeDefinition Type { get; }
    }

    internal interface IValueNode<T> : IValueNode where T : notnull
    {
        new T Value { get; }

        object IValueNode.Value => Value;
    }

    internal interface IConstantValueNode<T> : IValueNode<T>
        where T : struct
    {
        bool ISyntaxTreeNode.IsLeaf => true;
    }
}

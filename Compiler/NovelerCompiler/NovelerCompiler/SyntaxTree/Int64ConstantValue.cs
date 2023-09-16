using System.Reflection.Metadata;

namespace Noveler.Compiler.SyntaxTree
{
    internal class Int64ConstantValue : IConstantValueNode<long>
    {
        public Int64ConstantValue(long value, TypeDefinition type)
        {
            Value = value;
            Type = type;
        }

        public long Value { get; }
        public TypeDefinition Type { get; }
    }
}

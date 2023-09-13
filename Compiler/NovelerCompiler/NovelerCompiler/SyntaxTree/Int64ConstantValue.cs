using System.Reflection.Metadata;

namespace Noveler.Compiler.SyntaxTree
{
    internal readonly struct Int64ConstantValue : IConstantValue<long>
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

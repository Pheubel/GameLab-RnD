using System.Reflection.Metadata;

namespace Noveler.Compiler.SyntaxTree
{
    internal readonly struct Int16ConstantValue : IConstantValue<short>
    {
        public Int16ConstantValue(short value, TypeDefinition type)
        {
            Value = value;
            Type = type;
        }

        public short Value { get; }
        public TypeDefinition Type { get; }
    }
}

using System.Reflection.Metadata;

namespace Noveler.Compiler.SyntaxTree
{
    internal readonly struct Int8ConstantValue : IConstantValue<sbyte>
    {
        public Int8ConstantValue(sbyte value, TypeDefinition type)
        {
            Value = value;
            Type = type;
        }

        public sbyte Value { get; }
        public TypeDefinition Type { get; }
    }
}

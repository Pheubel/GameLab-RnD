using System.Reflection.Metadata;

namespace Noveler.Compiler.SyntaxTree
{
    internal readonly struct UInt8ConstantValue : IConstantValue<byte>
    {
        public UInt8ConstantValue(byte value, TypeDefinition type)
        {
            Value = value;
            Type = type;
        }

        public byte Value { get; }
        public TypeDefinition Type { get; }
    }
}

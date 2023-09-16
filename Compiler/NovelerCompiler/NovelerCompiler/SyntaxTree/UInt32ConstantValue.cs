using System.Reflection.Metadata;

namespace Noveler.Compiler.SyntaxTree
{
    internal class UInt32ConstantValue : IConstantValueNode<uint>
    {
        public UInt32ConstantValue(uint value, TypeDefinition type)
        {
            Value = value;
            Type = type;
        }

        public uint Value { get; }
        public TypeDefinition Type { get; }
    }}

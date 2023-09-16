using System.Reflection.Metadata;

namespace Noveler.Compiler.SyntaxTree
{
    internal class UInt16ConstantValue : IConstantValueNode<ushort>
    {
        public UInt16ConstantValue(ushort value, TypeDefinition type)
        {
            Value = value;
            Type = type;
        }

        public ushort Value { get; }
        public TypeDefinition Type { get; }
    }}

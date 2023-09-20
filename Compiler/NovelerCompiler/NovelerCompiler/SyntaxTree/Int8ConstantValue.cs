using System.Reflection.Metadata;

namespace Noveler.Compiler.SyntaxTree
{
    internal class Int8ConstantValue : IConstantValueNode<sbyte>
    {
        public Int8ConstantValue(sbyte value, TypeDefinition type)
        {
            Value = value;
            Type = type;
        }

        public sbyte Value { get; }
        public TypeDefinition Type { get; }

        public void EmitCode(List<byte> output)
        {
            output.Add((byte)Value);
        }
    }
}

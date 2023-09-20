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

        public void EmitCode(List<byte> output)
        {
            Span<byte> buffer = stackalloc byte[sizeof(ushort)];
            BitConverter.TryWriteBytes(buffer, Value);
            ListUtil.AddRange(output, buffer);
        }
    }
}

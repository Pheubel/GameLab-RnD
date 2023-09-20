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

        public void EmitCode(List<byte> output)
        {
            Span<byte> buffer = stackalloc byte[sizeof(uint)];
            BitConverter.TryWriteBytes(buffer, Value);
            ListUtil.AddRange(output, buffer);
        }
    }
}

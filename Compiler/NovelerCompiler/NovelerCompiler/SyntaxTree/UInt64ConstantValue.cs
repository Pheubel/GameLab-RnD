using System.Reflection.Metadata;

namespace Noveler.Compiler.SyntaxTree
{
    internal class UInt64ConstantValue : IConstantValueNode<ulong>
    {
        public UInt64ConstantValue(ulong value, TypeDefinition type)
        {
            Value = value;
            Type = type;
        }

        public ulong Value { get; }
        public TypeDefinition Type { get; }

        public void EmitCode(List<byte> output)
        {
            Span<byte> buffer = stackalloc byte[sizeof(ulong)];
            BitConverter.TryWriteBytes(buffer, Value);
            ListUtil.AddRange(output, buffer);
        }
    }
}

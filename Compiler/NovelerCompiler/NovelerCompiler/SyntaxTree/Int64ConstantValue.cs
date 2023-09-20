using System.Reflection.Metadata;

namespace Noveler.Compiler.SyntaxTree
{
    internal class Int64ConstantValue : IConstantValueNode<long>
    {
        public Int64ConstantValue(long value, TypeDefinition type)
        {
            Value = value;
            Type = type;
        }

        public long Value { get; }
        public TypeDefinition Type { get; }

        public void EmitCode(List<byte> output)
        {
            Span<byte> buffer = stackalloc byte[sizeof(long)];
            BitConverter.TryWriteBytes(buffer, Value);
            ListUtil.AddRange(output, buffer);
        }
    }
}

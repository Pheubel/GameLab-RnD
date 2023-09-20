using System.Reflection.Metadata;

namespace Noveler.Compiler.SyntaxTree
{
    internal readonly struct Float64ConstantValue : IConstantValueNode<double>
    {
        public Float64ConstantValue(double value, TypeDefinition type)
        {
            Value = value;
            Type = type;
        }

        public double Value { get; }
        public TypeDefinition Type { get; }

        public void EmitCode(List<byte> output)
        {
            Span<byte> buffer = stackalloc byte[sizeof(double)];
            BitConverter.TryWriteBytes(buffer, Value);
            ListUtil.AddRange(output, buffer);
        }
    }
}

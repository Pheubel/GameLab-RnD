using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace Noveler.Compiler.SyntaxTree
{
    internal class Float32ConstantValue : IConstantValueNode<float>
    {
        public Float32ConstantValue(float value, TypeDefinition type)
        {
            Value = value;
            Type = type;
        }

        public float Value { get; }
        public TypeDefinition Type { get; }

        public void EmitCode(List<byte> output)
        {
            Span<byte> buffer = stackalloc byte[sizeof(float)];
            BitConverter.TryWriteBytes(buffer,Value);
            ListUtil.AddRange(output, buffer);
        }
    }
}

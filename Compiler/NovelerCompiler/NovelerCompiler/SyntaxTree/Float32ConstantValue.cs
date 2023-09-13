using System.Reflection.Metadata;

namespace Noveler.Compiler.SyntaxTree
{
    internal readonly struct Float32ConstantValue : IConstantValue<float>
    {
        public Float32ConstantValue(float value, TypeDefinition type)
        {
            Value = value;
            Type = type;
        }

        public float Value { get; }
        public TypeDefinition Type { get; }
    }}

﻿using System.Reflection.Metadata;

namespace Noveler.Compiler.SyntaxTree
{
    internal class Int16ConstantValue : IConstantValueNode<short>
    {
        public Int16ConstantValue(short value, TypeDefinition type)
        {
            Value = value;
            Type = type;
        }

        public short Value { get; }
        public TypeDefinition Type { get; }

        public void EmitCode(List<byte> output)
        {
            Span<byte> buffer = stackalloc byte[sizeof(short)];
            BitConverter.TryWriteBytes(buffer, Value);
            ListUtil.AddRange(output, buffer);
        }
    }
}

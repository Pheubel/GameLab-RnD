﻿using System.Reflection.Metadata;

namespace Noveler.Compiler.SyntaxTree
{
    internal class Int32ConstantValue : IConstantValueNode<int>
    {
        public Int32ConstantValue(int value, TypeDefinition type)
        {
            Value = value;
            Type = type;
        }

        public int Value { get; }
        public TypeDefinition Type { get; }
    }
}

using Noveler.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace NovelerCompiler
{
    [Flags]
    [SuppressMessage("Design", "CA1069:Enums values should not be duplicated", Justification = "Defined masks of interest.")]
    internal enum NumberModifier : byte
    {
        IsFloat = 0,
        IsInteger = 1,

        FloatIntMask = 1,

        IsSigned = 0,
        IsUnsigned = 1 << 1,

        SignedUnsignedMask = 1 << 1,

        Is32Bit = 0 << 2,
        Is64Bit = 1 << 2,
        Is16Bit = 2 << 2,
        Is8Bit = 3 << 2,

        SizeMask = 3 << 2,

        HasDefinedFloatInt = 1 << 5,
        HasDefinedSigned = 1 << 6,
        HasDefinedSize = 1 << 7
    }
}

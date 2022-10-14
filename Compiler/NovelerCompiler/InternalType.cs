using System.Runtime.CompilerServices;

namespace Noveler.Compiler
{
    public enum TypeId
    {
        Undeclared,
        Int32,
        Int64,
        Float32,
        Float64
    }

    public static class InternalTypeExtensions
    {
        public static bool IsCustomType(this TypeId typeId) =>
            (uint)typeId > (uint)TypeId.Float64;
    }
}
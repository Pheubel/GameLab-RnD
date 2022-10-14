namespace Noveler.Compiler
{
    public enum TypeId
    {
        Undeclared,
        Int32,
        UnsignedInt32,
        Int64,
        UnsignedInt64,
        Int8,
        Boolean = Int8,
        UnsignedInt8,
        Int16,
        UnsignedInt16,
        Float32,
        Float64
    }

    internal static class InternalTypeExtensions
    {
        public static bool IsCustomType(this TypeId typeId) =>
            (uint)typeId > (uint)TypeId.Float64;
    }
}
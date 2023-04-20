using Microsoft.CodeAnalysis;

namespace Noveler.Compiler.SymbolTypes
{
    internal sealed record StructureInfo(string Name, int SizeInBytes, Optional<FieldInfo[]> FieldInfo);
}

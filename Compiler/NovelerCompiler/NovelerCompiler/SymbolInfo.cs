using Noveler.Compiler.SymbolTypes;
using OneOf;
using OneOf.Types;

namespace Noveler.Compiler
{
	internal sealed class SymbolInfo
	{
		public string Name { get; }
		public OneOf<StructureInfoReference, StructureInfo, FieldInfo, FunctionInfo> Type { get; set; }
		public string? Value { get; set; }

		public SymbolInfo(string name)
		{
			Name = name;
		}
	}
}

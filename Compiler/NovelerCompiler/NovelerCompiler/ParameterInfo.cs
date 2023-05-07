using Noveler.Compiler.SymbolTypes;
using OneOf;

namespace Noveler.Compiler
{
    internal sealed record ParameterInfo(string Name, string TypeName)
	{
		public OneOf<StructureInfoReference, StructureInfo> TypeInfo { get; set; }
	}
}

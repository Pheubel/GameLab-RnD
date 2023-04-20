using Noveler.Compiler.SymbolTypes;
using OneOf;

namespace Noveler.Compiler
{
    internal sealed record ParameterInfo(string Name, string TypeName)
	{
		public OneOf<UnknownInfo, StructureInfo> TypeInfo { get; set; }
	}
}

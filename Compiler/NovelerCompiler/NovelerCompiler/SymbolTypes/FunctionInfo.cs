using OneOf;

namespace Noveler.Compiler.SymbolTypes
{
	internal sealed record FunctionInfo(string Name, ParameterInfo[] Parameters)
	{
		public OneOf<StructureInfoReference, StructureInfo> ReturnType { get; set; }
		public int ParameterCount => Parameters.Length;
	}
}

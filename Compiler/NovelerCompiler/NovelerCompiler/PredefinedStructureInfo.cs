using Noveler.Compiler.SymbolTypes;

namespace Noveler.Compiler
{
	internal static class PredefinedStructureInfo
	{
		public static readonly StructureInfo Int8 = new StructureInfo("Int8", 1);
		public static readonly StructureInfo Int16 = new StructureInfo("Int16", 2);
		public static readonly StructureInfo Int32 = new StructureInfo("Int32", 4);
		public static readonly StructureInfo Int64 = new StructureInfo("Int64", 8);

		public static readonly StructureInfo UInt8 = new StructureInfo("UInt8", 1);
		public static readonly StructureInfo UInt16 = new StructureInfo("UInt16", 2);
		public static readonly StructureInfo UInt32 = new StructureInfo("UInt32", 4);
		public static readonly StructureInfo UInt64 = new StructureInfo("UInt64", 8);

		public static readonly StructureInfo Float32 = new StructureInfo("Float32", 4);
		public static readonly StructureInfo Float64 = new StructureInfo("Float64", 8);
	}
}

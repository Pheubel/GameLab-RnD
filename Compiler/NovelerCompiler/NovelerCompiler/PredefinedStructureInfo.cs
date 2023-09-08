using Noveler.Compiler.CodeDomainObjectModel;

namespace Noveler.Compiler
{
	internal static class PredefinedTypeDefinitions
	{
		public static readonly TypeDefinition Int8 = new TypeDefinition("Int8", 1);
		public static readonly TypeDefinition Int16 = new TypeDefinition("Int16", 2);
		public static readonly TypeDefinition Int32 = new TypeDefinition("Int32", 4);
		public static readonly TypeDefinition Int64 = new TypeDefinition("Int64", 8);

		public static readonly TypeDefinition UInt8 = new TypeDefinition("UInt8", 1);
		public static readonly TypeDefinition UInt16 = new TypeDefinition("UInt16", 2);
		public static readonly TypeDefinition UInt32 = new TypeDefinition("UInt32", 4);
		public static readonly TypeDefinition UInt64 = new TypeDefinition("UInt64", 8);

		public static readonly TypeDefinition Float32 = new TypeDefinition("Float32", 4);
		public static readonly TypeDefinition Float64 = new TypeDefinition("Float64", 8);
	}
}

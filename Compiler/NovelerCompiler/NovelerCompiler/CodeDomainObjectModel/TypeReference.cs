namespace Noveler.Compiler.CodeDomainObjectModel
{
    internal sealed record TypeReference(string Name, TypeReferenceCollection GenericTypeArgument) : DomainObject
	{
		// TODO look at https://learn.microsoft.com/en-us/dotnet/api/system.codedom.codetypereference?view=dotnet-plat-ext-7.0#properties


		public TypeReference(string Name) : this(Name, new()) { }

		public override IReadOnlyList<DomainObject> GetChildren() => Array.Empty<DomainObject>();
	}
	internal static class BuiltIns
	{
		public static class TypeReferences
		{
			public static readonly TypeReference Boolean = new TypeReference("Boolean");
			public static readonly TypeReference Int8 = new TypeReference("Int8");
			public static readonly TypeReference Int16 = new TypeReference("Int16");
			public static readonly TypeReference Int32 = new TypeReference("Int32");
			public static readonly TypeReference Int64 = new TypeReference("Int64");
			public static readonly TypeReference UInt8 = new TypeReference("UInt8");
			public static readonly TypeReference UInt16 = new TypeReference("UInt16");
			public static readonly TypeReference UInt32 = new TypeReference("UInt32");
			public static readonly TypeReference UInt64 = new TypeReference("UInt64");
			public static readonly TypeReference Float32 = new TypeReference("Float32");
			public static readonly TypeReference Float64 = new TypeReference("Float64");
		}
	}
}
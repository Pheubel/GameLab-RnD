namespace Noveler.Compiler.CodeDomainObjectModel
{
	internal sealed record TypeReference(string Name, TypeReferenceCollection GenericTypeArgument) : DomainObject
	{
		// TODO look at https://learn.microsoft.com/en-us/dotnet/api/system.codedom.codetypereference?view=dotnet-plat-ext-7.0#properties


		public TypeReference(string Name) : this(Name, new()) { }
	};
}

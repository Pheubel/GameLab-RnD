namespace Noveler.Compiler.CodeDomainObjectModel
{
	internal sealed record ParameterDeclarationExpression(string Name, TypeReference ParameterType) : DomainObject
	{
		// TODO: consider taking by ref as well

		public ParameterDeclarationExpression(string Name, string ParameterTypeName) : this(Name, new TypeReference(ParameterTypeName)) { }

		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				ParameterType
			};

			return children;
		}
	}
}

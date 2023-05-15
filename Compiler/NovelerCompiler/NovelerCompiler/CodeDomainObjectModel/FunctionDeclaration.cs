using Noveler.Compiler.CodeDomainObjectModel.Statements;

namespace Noveler.Compiler.CodeDomainObjectModel
{
	internal sealed record FunctionDeclaration(string Name, TypeReference ReturnType) : DomainObject
	{
		public FunctionDeclaration(string Name, string ReturnTypeName) : this(Name, new TypeReference(ReturnTypeName)) { }

		public TypeParameterCollection GenericTypeParameters { get; } = new TypeParameterCollection();

		public ParameterDeclarationExpressionCollection Parameters { get; } = new ParameterDeclarationExpressionCollection();

		public StatementCollection Statements { get; } = new StatementCollection();

		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				ReturnType,
				GenericTypeParameters,
				Parameters ,
				Statements
			};

			return children;
		}
	}
}

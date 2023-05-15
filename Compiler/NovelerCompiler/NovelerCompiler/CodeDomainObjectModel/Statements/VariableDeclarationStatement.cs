namespace Noveler.Compiler.CodeDomainObjectModel.Statements
{
	/// <summary>
	/// Represents a variable declaration statement.
	/// </summary>
	/// <param name="VariableName"> The name of the variable to be declared.</param>
	/// <param name="TypeReference"> The type of the variable.</param>
	/// <example>a : b</example>
	internal sealed record VariableDeclarationStatement(string VariableName, TypeReference TypeReference) : Statement
	{
		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				TypeReference
			};

			return children;
		}
	}
}

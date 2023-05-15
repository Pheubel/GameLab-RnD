namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// A reference to a variable.
	/// </summary>
	/// <param name="Name"> The name of the variable being referenced.</param>
	internal sealed record VariableReferenceExpression(string Name) : Expression
	{
		public override IReadOnlyList<DomainObject> GetChildren() => Array.Empty<DomainObject>();
	}
}

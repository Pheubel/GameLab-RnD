namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents a negation expression.
	/// </summary>
	/// <param name="ValueExpression"> The expression whose sign will be negated.</param>
	/// <example>-a</example>
	internal sealed record NegateExpression(Expression ValueExpression) : Expression
	{
		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				ValueExpression
			};

			return children;
		}
	}
}

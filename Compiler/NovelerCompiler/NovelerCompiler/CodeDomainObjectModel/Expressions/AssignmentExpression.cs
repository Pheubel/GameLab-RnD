namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents an assignmen expression
	/// </summary>
	/// <param name="TargetExpression"> The expression representing the target of the assignemnt.</param>
	/// <param name="RightHandExpression"> The expression indicating the value to assign to the target.</param>
	/// <example>a = b</example>
	internal sealed record AssignmentExpression(Expression TargetExpression, Expression RightHandExpression) : Expression
	{
		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				TargetExpression,
				RightHandExpression
			};

			return children;
		}
	}
}

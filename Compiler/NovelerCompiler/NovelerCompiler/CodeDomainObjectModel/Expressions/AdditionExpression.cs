namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents an addition expression.
	/// </summary>
	/// <param name="LeftHandExpression"> The left hand side expression of the addition expression.</param>
	/// <param name="RightHandExpression"> The right hand side expression of the addition expression.</param>
	/// <example>a + b</example>
	internal sealed record AdditionExpression(Expression LeftHandExpression, Expression RightHandExpression) : Expression
	{
		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				LeftHandExpression,
				RightHandExpression
			};

			return children;
		}
	}
}

namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents a greater than expression.
	/// </summary>
	/// <param name="LeftHandExpression"> The left hand side expression of the equality expression.</param>
	/// <param name="RightHandExpression"> The right hand side expression of the equality expression.</param>
	/// <example>a <![CDATA[>]]> b</example>
	internal sealed record GreaterThanExpression(Expression LeftHandExpression, Expression RightHandExpression) : Expression
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

		/// <summary>
		/// Inverts the expression to be a less than expression.
		/// </summary>
		/// <returns> The inverted expression.</returns>
		/// <example> a <![CDATA[>]]> b will become b <![CDATA[<]]> a.</example>
		public LessThanExpression Invert()
		{
			return new LessThanExpression(RightHandExpression, LeftHandExpression);
		}
	}

}

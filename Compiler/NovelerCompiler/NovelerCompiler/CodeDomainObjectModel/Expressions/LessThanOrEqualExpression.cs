namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents a less than or equal expression.
	/// </summary>
	/// <param name="LeftHandExpression"> The left hand side expression of the equality expression.</param>
	/// <param name="RightHandExpression"> The right hand side expression of the equality expression.</param>
	/// <example>a <![CDATA[<]]> b</example>
	internal sealed record LessThanOrEqualExpression(Expression LeftHandExpression, Expression RightHandExpression) : Expression
	{
		/// <summary>
		/// Inverts the expression to be a greater than or equal expression.
		/// </summary>
		/// <returns> The inverted expression.</returns>
		/// <example> a <![CDATA[<]]>= b will become b <![CDATA[>]]>= a.</example>
		public GreaterThanOrEqualExpression Invert()
		{
			return new GreaterThanOrEqualExpression(RightHandExpression, LeftHandExpression);
		}
	};
}

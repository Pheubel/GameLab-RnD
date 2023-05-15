namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents a less than expression.
	/// </summary>
	/// <param name="LeftHandExpression"> The left hand side expression of the equality expression.</param>
	/// <param name="RightHandExpression"> The right hand side expression of the equality expression.</param>
	/// <example>a <![CDATA[<]]> b</example>
	internal sealed record LessThanExpression(Expression LeftHandExpression, Expression RightHandExpression) : Expression
	{
		/// <summary>
		/// Inverts the expression to be a less than expression.
		/// </summary>
		/// <returns> The inverted expression.</returns>
		/// <example> a <![CDATA[<]]> b will become b <![CDATA[>]]> a.</example>
		public GreaterThanExpression Invert()
		{
			return new GreaterThanExpression(RightHandExpression, LeftHandExpression);
		}
	};
}

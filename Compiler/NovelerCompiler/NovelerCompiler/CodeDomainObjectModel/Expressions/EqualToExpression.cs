namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents an equals to expression.
	/// </summary>
	/// <param name="LeftHandExpression"> The left hand side expression of the equality expression.</param>
	/// <param name="RightHandExpression"> The right hand side expression of the equality expression.</param>
	/// <example>a == b</example>
	internal sealed record EqualToExpression(Expression LeftHandExpression, Expression RightHandExpression) : Expression;
}

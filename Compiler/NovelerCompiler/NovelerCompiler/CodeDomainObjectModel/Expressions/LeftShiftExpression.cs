namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents a left shift expression.
	/// </summary>
	/// <param name="LeftHandExpression"> The left hand side expression of the shift expression.</param>
	/// <param name="RightHandExpression"> The right hand side expression of the shift expression.</param>
	/// <example>a << b</example>
	internal sealed record LeftShiftExpression(Expression LeftHandExpression, Expression RightHandExpression) : Expression;
}

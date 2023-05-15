namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents an exclusive or expression, also known as bitwise xor.
	/// </summary>
	/// <param name="LeftHandExpression"> The left hand side expression of the exclusive or expression.</param>
	/// <param name="RightHandExpression"> The right hand side expression of the exclusive or expression.</param>
	/// <example>a ^ b</example>
	internal sealed record ExclusiveOrExpression(Expression LeftHandExpression, Expression RightHandExpression) : Expression;
}

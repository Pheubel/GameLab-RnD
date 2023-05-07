namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents a binary expression.
	/// </summary>
	/// <param name="Left"> The expression on the left side of the binary expression.</param>
	/// <param name="Operator"> The operator that is used in the expression.</param>
	/// <param name="Right"> The expression on the right side of the binary expression.</param>
	/// <example>2 * 3</example>
	internal sealed record BinaryOperatorExpression(Expression Left, BinaryOperatorType Operator, Expression Right) : Expression;
}

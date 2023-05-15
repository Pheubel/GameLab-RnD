namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents a conditional not expression
	/// </summary>
	/// <remarks>
	/// Used in boolean expressions to invert the boolean value.
	/// </remarks>
	/// <param name="ValueExpression"> the expression whose value's truthness gets inverted.</param>
	/// <example>!a</example>
	internal sealed record ConditionalNotExpression(Expression ValueExpression) : Expression;
}

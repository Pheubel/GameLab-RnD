namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents a bitwise not expression
	/// </summary>
	/// <param name="ValueExpression"> The expression whose value bits will get inverted.</param>
	/// <example>~a</example>
	internal sealed record BitwiseNotExpression(Expression ValueExpression) : Expression;
}

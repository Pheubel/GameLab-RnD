namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents a post-decrement expression.
	/// </summary>
	/// <param name="ValueExpression"> The value expression that gets incremented.</param>
	internal sealed record PostDecrementExpression(Expression ValueExpression) : Expression;
}

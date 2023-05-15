namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents a post-increment expression.
	/// </summary>
	/// <param name="ValueExpression"> The value expression that gets incremented.</param>
	internal sealed record PostIncrementExpression(Expression ValueExpression) : Expression;
}

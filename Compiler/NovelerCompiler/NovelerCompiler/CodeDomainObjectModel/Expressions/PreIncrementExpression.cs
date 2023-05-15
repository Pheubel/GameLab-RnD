namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents a pre-increment expression.
	/// </summary>
	/// <param name="ValueExpression"> The value expression that gets incremented.</param>
	internal sealed record PreIncrementExpression(Expression ValueExpression) : Expression;
}

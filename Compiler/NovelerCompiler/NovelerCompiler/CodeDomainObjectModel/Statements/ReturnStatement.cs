using Noveler.Compiler.CodeDomainObjectModel.Expressions;

namespace Noveler.Compiler.CodeDomainObjectModel.Statements
{
	/// <summary>
	/// Represents a return statement for a function.
	/// </summary>
	/// <param name="ReturnExpression"> The expression to evaluate, whose result gets returned.</param>
	internal sealed record ReturnStatement(Expression? ReturnExpression) : Statement;
}

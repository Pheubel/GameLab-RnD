namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents an object being accessed through an index.
	/// </summary>
	/// <param name="ObjectExpression"> The expression for retrieving the object to be accessed</param>
	/// <param name="IndexExpression"> The expression for retrieving the index.</param>
	/// <example>foo[2]</example>
	internal sealed record IndexerExpression(Expression ObjectExpression, Expression IndexExpression) : Expression;
}

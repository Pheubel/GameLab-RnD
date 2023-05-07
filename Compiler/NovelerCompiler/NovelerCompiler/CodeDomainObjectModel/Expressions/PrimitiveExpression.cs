namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents a literal value.
	/// </summary>
	/// <param name="Value"> The string representation of the literal.</param>
	internal sealed record PrimitiveExpression(string Value) : Expression;
}

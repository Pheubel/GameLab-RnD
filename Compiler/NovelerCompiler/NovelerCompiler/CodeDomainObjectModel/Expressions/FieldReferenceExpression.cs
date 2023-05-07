namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// A reference to a type field.
	/// </summary>
	/// <param name="ObjectExpression"> The expression for getting the object instance to access the field.</param>
	/// <param name="Name"> The name of the field to be referenced.</param>
	internal sealed record FieldReferenceExpression(Expression ObjectExpression, string Name);
}

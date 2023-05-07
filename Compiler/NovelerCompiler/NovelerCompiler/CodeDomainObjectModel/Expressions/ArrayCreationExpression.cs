namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents the creation of an array.
	/// </summary>
	/// <param name="ArrayType"> The type of the array to be constructed.</param>
	/// <param name="SizeExpression"> The expression determining the size of the array.</param>
	/// <example>the right side of: foo = bar[10]</example>
	internal sealed record ArrayCreationExpression(TypeReference ArrayType, Expression SizeExpression) : Expression
	{
		public ArrayCreationExpression(string ArrayTypeName, Expression SizeExpression) : this(new TypeReference(ArrayTypeName), SizeExpression) { }
	};
}

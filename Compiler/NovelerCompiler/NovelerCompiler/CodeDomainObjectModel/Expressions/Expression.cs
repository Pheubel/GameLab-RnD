namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	internal abstract record Expression : DomainObject;

	internal sealed record SimpleAccessExpression : Expression
	{
		// TODO
	};

	internal sealed record ObjectCreateExpression(TypeReference Type, ExpressionCollection Parameters) : Expression
	{

	}

	internal sealed record ExpressionCollection : DomainObjectCollection<Expression>;
}

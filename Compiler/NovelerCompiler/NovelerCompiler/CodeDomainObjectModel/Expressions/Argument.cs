namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	internal sealed record Argument(Expression ArgumentExpression) : DomainObject
	{
		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				ArgumentExpression
			};

			return children;
		}
	}
}

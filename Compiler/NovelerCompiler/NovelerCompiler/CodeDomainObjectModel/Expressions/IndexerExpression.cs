namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents an object being accessed through an index.
	/// </summary>
	/// <example>foo[2]</example>
	internal sealed record IndexerExpression(Expression Source, ArgumentCollection Arguments) : Expression
	{
		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				Source,
				Arguments
			};

			return children;
		}
	}
}

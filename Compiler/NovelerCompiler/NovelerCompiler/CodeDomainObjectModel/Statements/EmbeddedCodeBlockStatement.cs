namespace Noveler.Compiler.CodeDomainObjectModel.Statements
{
	/// <summary>
	/// Represents a a code block statement embedded in the story.
	/// </summary>
	internal sealed record EmbeddedCodeBlockStatement : EmbeddedStatement
	{
		public StatementCollection Statements { get; } = new();

		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				Statements
			};

			return children;
		}
	}
}

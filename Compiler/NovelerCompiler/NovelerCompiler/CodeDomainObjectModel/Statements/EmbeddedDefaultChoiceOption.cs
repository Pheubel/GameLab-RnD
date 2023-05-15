namespace Noveler.Compiler.CodeDomainObjectModel.Statements
{
	/// <summary>
	/// Represents a default option that shows up after you have no more other option left.
	/// </summary>
	/// <param name="OptionText"> The text statement depicting the choice you can make.</param>
	internal sealed record EmbeddedDefaultChoiceOption(TextStatement OptionText) : EmbeddedStatement
	{
		public StatementCollection OptionStatements { get; } = new();

		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				OptionText,
				OptionStatements
			};

			return children;
		}
	}
}

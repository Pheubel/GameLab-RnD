namespace Noveler.Compiler.CodeDomainObjectModel.Statements
{
	/// <summary>
	/// Represents a choice option inside of a choice block.
	/// </summary>
	/// <param name="OptionText"> The text statement depicting the choice you can make.</param>
	internal sealed record EmbeddedChoiceOption(TextStatement OptionText) : EmbeddedStatement
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

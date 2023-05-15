namespace Noveler.Compiler.CodeDomainObjectModel.Statements
{
	/// <summary>
	/// Represents a choice block with options a player can select from.
	/// </summary>
	internal sealed record EmbeddedChoiceBlockStatement : EmbeddedStatement
	{
		public EmbeddedChoiceOptionCollection ChoiceOptions { get; } = new();
		public EmbeddedDefaultChoiceOptionCollection DefaultOption { get; } = new();

		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				ChoiceOptions,
				DefaultOption
			};

			return children;
		}
	}
}

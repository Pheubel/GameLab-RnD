using Noveler.Compiler.CodeDomainObjectModel.Expressions;

namespace Noveler.Compiler.CodeDomainObjectModel.Statements
{
	/// <summary>
	/// Represents an if else construct with a true and false branch.
	/// </summary>
	/// <param name="Condition"> The expression that gets executed to determine if the true or false branch should be chosen.</param>
	internal sealed record ConditionStatement(Expression Condition) : Statement
	{
		/// <summary>
		/// The statements that get executed when the condition is true.
		/// </summary>
		public StatementCollection TrueStatements { get; } = new StatementCollection();

		/// <summary>
		/// The statements that get executed when the condition is true.
		/// </summary>
		public StatementCollection FalseStatements { get; } = new StatementCollection();

		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				Condition,
				TrueStatements,
				FalseStatements
			};

			return children;
		}
	}
}

using Noveler.Compiler.CodeDomainObjectModel.Expressions;

namespace Noveler.Compiler.CodeDomainObjectModel.Statements
{
	/// <summary>
	/// Represents a conditional statement embedded in the story
	/// </summary>
	/// <param name="Condition"> The condition expression to evaluate.</param>
	/// <example>@if (a) { b() } else { c++ }</example>
	internal sealed record EmbeddedConditionalStatement(Expression Condition) : EmbeddedStatement
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

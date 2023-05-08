using Noveler.Compiler.CodeDomainObjectModel.Expressions;

namespace Noveler.Compiler.CodeDomainObjectModel.Statements
{
	internal abstract record Statement : DomainObject;

	/// <summary>
	/// Represents a assign statement for coopying one value to another.
	/// </summary>
	/// <example>a = 4</example>
	internal sealed record AssignStatement : Statement
	{
		public Expression LeftSide { get; set; }
		public Expression RightSide { get; set; }
	};

	internal sealed record VariableDeclarationStatement(string VariableName, TypeReference TypeReference) : Statement;

	internal sealed record VariableDeclarationAssignmentStatement(VariableDeclarationStatement VariableDeclaration, Expression InitializationExpression) : Statement;

	internal sealed record EmptyStatement() : Statement;

	internal abstract record EmbeddedStatement : Statement;

	//internal sealed record EmbeddedVariableDeclaration(string VariableName) : EmbeddedStatement
	//{
	//	public Expression? InitializationExpression { get; set; }
	//};

	internal sealed record EmbeddedCodeBlockStatement : EmbeddedStatement
	{
		public StatementCollection Statements { get; } = new();
	};

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
	};

	internal sealed record EmbeddedChoiceBlock : EmbeddedStatement
	{
		public EmbeddedChoiceOptionCollection ChoiceOptions { get; } = new();
	};

	internal sealed record EmbeddedChoiceOption(TextStatement OptionText) : EmbeddedStatement
	{
		public StatementCollection OptionStatements { get; } = new();
	}

	internal sealed record EmbeddedChoiceOptionCollection : DomainObjectCollection<EmbeddedChoiceOption>;
}

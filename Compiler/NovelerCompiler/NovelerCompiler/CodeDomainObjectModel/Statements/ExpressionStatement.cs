using Noveler.Compiler.CodeDomainObjectModel.Expressions;

namespace Noveler.Compiler.CodeDomainObjectModel.Statements
{
	/// <summary>
	/// Represents a statement with an expression.
	/// </summary>
	/// <param name="Expression"> The expression that will be executed.</param>
	internal sealed record ExpressionStatement(Expression Expression) : Statement
	{
		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				Expression
			};

			return children;
		}
	}
}

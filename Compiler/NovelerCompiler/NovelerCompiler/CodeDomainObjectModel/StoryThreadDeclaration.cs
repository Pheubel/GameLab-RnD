using Noveler.Compiler.CodeDomainObjectModel.Statements;

namespace Noveler.Compiler.CodeDomainObjectModel
{
	/// <summary>
	/// Represents a thread in a story that you can go to at any point.
	/// </summary>
	/// <param name="Name"> The name of the thread.</param>
	internal sealed record StoryThreadDeclaration(string Name) : DomainObject
	{
		public ParameterDeclarationExpressionCollection Parameters { get; } = new ParameterDeclarationExpressionCollection();
		public StatementCollection Statements { get; } = new();

		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				Parameters,
				Statements
			};

			return children;
		}
	}
}

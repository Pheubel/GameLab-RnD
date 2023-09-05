using Noveler.Compiler.CodeDomainObjectModel.Statements;
using System.Text;

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

        public override string ToString()
        {
			StringBuilder sb = new();

			sb.AppendLine($"Thread \"{Name}\":");

			sb.AppendLine($"\tParameters ({Parameters.Count}):");
			foreach ( var parameter in Parameters )
			{
                sb.AppendLine($"{parameter.Name} : {parameter.ParameterType.Name}");
            }

			int statementCount = Statements.Count;
            sb.AppendLine($"\tStatements ({statementCount}):");
			for (int i = 0; i < statementCount; i++)
			{
				sb.AppendLine($"{i + 1}:\t{Statements[i]}");
			}


            return sb.ToString();
        }
    }
}

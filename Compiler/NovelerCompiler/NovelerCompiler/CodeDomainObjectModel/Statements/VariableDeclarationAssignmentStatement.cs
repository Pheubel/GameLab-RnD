using Noveler.Compiler.CodeDomainObjectModel.Expressions;

namespace Noveler.Compiler.CodeDomainObjectModel.Statements
{
    /// <summary>
    /// Represents a variable declaration followed by an assignment.
    /// </summary>
    /// <param name="VariableDeclaration"> The variable declaration.</param>
    /// <param name="InitializationExpression"> The expression that gets evaluated for the initial value.</param>
    /// <example>a : b = c</example>
    internal sealed record VariableDeclarationAssignmentStatement(VariableDeclarationStatement VariableDeclaration, Expression InitializationExpression) : Statement, IVariableDeclarationStatement
    {
		public bool HasInitializationExpression => true;

        public string VariableName => VariableDeclaration.VariableName;

        public TypeReference TypeReference => VariableDeclaration.TypeReference;

        public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				VariableDeclaration,
				InitializationExpression
			};

			return children;
		}
	}
}

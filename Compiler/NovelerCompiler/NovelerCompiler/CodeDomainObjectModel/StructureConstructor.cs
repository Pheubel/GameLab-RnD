using Noveler.Compiler.CodeDomainObjectModel.Statements;
using System.Text;

namespace Noveler.Compiler.CodeDomainObjectModel
{
    internal sealed record StructureConstructor(TypeReference StructureType) : StructureMember
    {
        public StructureConstructor(string StructureType) : this(new TypeReference(StructureType))
        { }

        public ParameterDeclarationExpressionCollection Parameters { get; } = new ParameterDeclarationExpressionCollection();

        public StatementCollection Statements { get; } = new StatementCollection();

        public override IReadOnlyList<DomainObject> GetChildren()
        {
            return new DomainObject[]
            {
                Parameters,
                Statements
            };
        }

        public override string ToString()
        {
            StringBuilder sb = new();

            sb.AppendLine($"\tParameters ({Parameters.Count})");
            foreach (ParameterDeclarationExpression parameter in Parameters)
            {
                sb.AppendLine($"{parameter.Name} : {parameter.ParameterType.Name}");
            }
            sb.AppendLine();

            sb.AppendLine($"\tStatements ({Statements.Count}):");
            foreach (Statement statement in Statements)
            {
                sb.AppendLine(statement.ToString());
            }

            return sb.ToString();
        }
    }
}
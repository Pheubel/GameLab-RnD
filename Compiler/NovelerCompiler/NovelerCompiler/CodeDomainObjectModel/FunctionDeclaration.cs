using Noveler.Compiler.CodeDomainObjectModel.Statements;
using System.Text;

namespace Noveler.Compiler.CodeDomainObjectModel
{
    internal sealed record FunctionDeclaration(string Name, TypeReference ReturnType) : DomainObject
    {
        public bool IsGenericFunction => GenericTypeParameters.Count > 0;

        public FunctionDeclaration(string Name, string ReturnTypeName) : this(Name, new TypeReference(ReturnTypeName)) { }

        public TypeParameterCollection GenericTypeParameters { get; } = new TypeParameterCollection();

        public ParameterDeclarationExpressionCollection Parameters { get; } = new ParameterDeclarationExpressionCollection();

        public StatementCollection Statements { get; } = new StatementCollection();

        public override IReadOnlyList<DomainObject> GetChildren()
        {
            var children = new List<DomainObject>
            {
                ReturnType,
                GenericTypeParameters,
                Parameters ,
                Statements
            };

            return children;
        }

        public override string ToString()
        {
            StringBuilder sb = new();

            sb.AppendLine($"Function \"{Name}\":");

            sb.AppendLine($"\tReturn type: {ReturnType.Name}");

            sb.AppendLine();

            sb.AppendLine($"\tGeneric type arguments ({GenericTypeParameters.Count}):");
            foreach (TypeParameter genericTypeParameter in GenericTypeParameters)
            {
                sb.AppendLine(genericTypeParameter.Name);
            }
            sb.AppendLine();

            sb.AppendLine($"\tParameters ({Parameters.Count})");
            foreach(ParameterDeclarationExpression parameter in Parameters)
            {
                sb.AppendLine($"{parameter.Name} : {parameter.ParameterType.Name}");
            }
            sb.AppendLine();

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

using System.Text;

namespace Noveler.Compiler.CodeDomainObjectModel
{
    internal sealed record FunctionDeclaration(string Name, TypeReference ReturnType, SymbolScope SymbolScope) : DomainObject
    {
        public bool IsGenericFunction => GenericTypeParameters.Count > 0;

        public FunctionDeclaration(string Name, string ReturnTypeName, SymbolScope SymbolScope) : this(Name, new TypeReference(ReturnTypeName), SymbolScope) { }

        public TypeParameterCollection GenericTypeParameters { get; } = new TypeParameterCollection();

        public ParameterDeclarationExpressionCollection Parameters { get; } = new ParameterDeclarationExpressionCollection();

        public FunctionBodyDeclaration FunctionBodyDeclaration { get; set; } = null!;

        public override IReadOnlyList<DomainObject> GetChildren()
        {
            var children = new List<DomainObject>
            {
                ReturnType,
                GenericTypeParameters,
                Parameters ,
                FunctionBodyDeclaration
            };

            return children;
        }
    }
}

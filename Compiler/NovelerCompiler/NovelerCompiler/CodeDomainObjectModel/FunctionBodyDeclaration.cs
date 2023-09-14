using Noveler.Compiler.CodeDomainObjectModel.Statements;

namespace Noveler.Compiler.CodeDomainObjectModel
{
    internal sealed record FunctionBodyDeclaration(SymbolScope SymbolScope) : DomainObject
    {
        public StatementCollection Statements { get; } = new StatementCollection();

        public override IReadOnlyList<DomainObject> GetChildren()
        {
            return new DomainObject[]
            {
                Statements
            };
        }
    }
}

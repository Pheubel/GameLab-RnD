using Noveler.Compiler.CodeDomainObjectModel.Statements;

namespace Noveler.Compiler.CodeDomainObjectModel
{
    internal sealed record TypeHeader(string TypeName) : DomainObject
    {
        public override IReadOnlyList<DomainObject> GetChildren()
        {
            return Array.Empty<DomainObject>();
        }
    }

    internal sealed record TypeBody : DomainObject
    {
        StructureMemberCollection StructureMembers { get; } = new StructureMemberCollection();

        public override IReadOnlyList<DomainObject> GetChildren()
        {
            return new[] { StructureMembers };
        }
    }
    internal sealed record StructureMemberCollection : DomainObjectCollection<StructureMember> { }

    internal abstract record StructureMember : DomainObject;

    internal sealed record StructureMemberField(IVariableDeclarationStatement FieldDeclarationStatement) : StructureMember
    {
        public override IReadOnlyList<DomainObject> GetChildren()
        {
            return new DomainObject[]
            {
                (Statement)FieldDeclarationStatement
            };
        }
    }
    internal sealed record StructureConstructor : StructureMember
    {
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
    }
}
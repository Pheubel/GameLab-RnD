using Noveler.Compiler.CodeDomainObjectModel.Statements;

namespace Noveler.Compiler.CodeDomainObjectModel
{
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
}
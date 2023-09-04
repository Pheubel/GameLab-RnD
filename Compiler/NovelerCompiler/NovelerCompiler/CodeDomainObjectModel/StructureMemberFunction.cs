namespace Noveler.Compiler.CodeDomainObjectModel
{
    internal sealed record StructureMemberFunction(FunctionDeclaration FunctionDeclaration) : StructureMember
    {
        public override IReadOnlyList<DomainObject> GetChildren()
        {
            return new DomainObject[]
            {
                FunctionDeclaration
            };
        }
    }
}
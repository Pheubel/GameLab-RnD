namespace Noveler.Compiler.CodeDomainObjectModel
{
    internal sealed record TypeBody : DomainObject
    {
        StructureMemberCollection StructureMembers { get; } = new StructureMemberCollection();

        public override IReadOnlyList<DomainObject> GetChildren()
        {
            return new[] { StructureMembers };
        }
    }
}
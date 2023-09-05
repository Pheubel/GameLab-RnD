namespace Noveler.Compiler.CodeDomainObjectModel
{
    internal sealed record TypeHeader(string TypeName) : DomainObject
    {
        public override IReadOnlyList<DomainObject> GetChildren()
        {
            return Array.Empty<DomainObject>();
        }
    }
}
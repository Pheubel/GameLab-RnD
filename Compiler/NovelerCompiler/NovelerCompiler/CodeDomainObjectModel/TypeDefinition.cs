namespace Noveler.Compiler.CodeDomainObjectModel
{
    internal sealed record TypeDefinition(string Name, TypeFieldDefinition[] TypeFieldDefinitions)
    {
        public bool IsFullyDefined { get; set; }
        public int SizeInBytes { get; set; }

        public TypeDefinition(string Name, int SizeInBytes) :
            this(Name, Array.Empty<TypeFieldDefinition>())
        {
            this.SizeInBytes = SizeInBytes;
            IsFullyDefined = true;
        }
    }
}

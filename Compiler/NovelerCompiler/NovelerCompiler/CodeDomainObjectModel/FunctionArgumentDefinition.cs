namespace Noveler.Compiler.CodeDomainObjectModel
{
    internal sealed record FunctionArgumentDefinition(string Name, TypeDefinition ArgumentType)
    {
        public int ArgumentIndex { get; set; }
    };
}
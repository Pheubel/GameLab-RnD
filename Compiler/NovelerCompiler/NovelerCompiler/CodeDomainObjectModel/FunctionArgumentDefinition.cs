namespace Noveler.Compiler.CodeDomainObjectModel
{
    internal sealed record FunctionArgumentDefinition(string Name, TypeDefinition ArgumentType)
    {
        int _argumentIndex;
     
        public ref int ArgumentIndex { get => ref _argumentIndex; }
    };
}
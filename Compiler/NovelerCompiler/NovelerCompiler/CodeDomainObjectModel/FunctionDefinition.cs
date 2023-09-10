using System.Text;

namespace Noveler.Compiler.CodeDomainObjectModel
{
    internal sealed record FunctionDefinition(
        string Name,
        NamespaceDefinition Namespace,
        TypeDefinition? ParentType,
        CompilationUnit? OriginalCompilationUnit,
        FunctionDeclaration OriginalDeclaration)
    {
        // TODO: figure this out properly, could use a list or custom type
        public OrderedDictionary<string, FunctionArgumentDefinition> FunctionArguments { get; } = new();

        public bool IsStructureMethod => ParentType != null;
        public bool IsFullyDefined { get; set; }


        public string GetFunctionSignature()
        {
            StringBuilder sb = new();
            sb.Append(Name);
            sb.Append('(');
            sb.Append(')');
        }
    };

    internal sealed record FunctionArgumentDefinition(string Name, TypeDefinition ArgumentType)
    {

    };
}
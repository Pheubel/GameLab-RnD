using System.Text;

namespace Noveler.Compiler.CodeDomainObjectModel
{
    enum StructureType
    {
        UserType,
        Unit,
        Int8,
        Int16,
        Int32,
        Int64,
        UInt8,
        UInt16,
        UInt32,
        UInt64,
        Float32,
        Float64,
    }

    internal sealed record TypeDefinition(string Name, NamespaceDefinition Namespace, CompilationUnit? OriginalCompilationUnit, TypeDeclaration? OriginalDeclaration) : IQualifyable
    {
        public Dictionary<string, TypeFieldDefinition> TypeFieldDefinitions { get; } = new();
        public Dictionary<string, FunctionDefinition> TypeFunctionDefinitions { get; } = new();
        public bool IsFullyDefined { get; set; }
        public int SizeInBytes { get; set; }
        
        // by default, assume the type is a user type.
        public StructureType StructureType { get; init; } = StructureType.UserType; 

        public TypeDefinition(string Name, NamespaceDefinition Namespace, int SizeInBytes) :
            this(Name, Namespace, null, null)
        {
            this.SizeInBytes = SizeInBytes;
            IsFullyDefined = true;
        }

        public string GetFullyQualifiedName()
        {
            StringBuilder sb = new();
            GetFullyQualifiedName(sb);

            return sb.ToString();
        }

        public void GetFullyQualifiedName(StringBuilder stringBuilder)
        {
            // this is tail recursable, but is this the best it can be?
            stringBuilder.Insert(0, $".{Name}");
            Namespace.GetFullyQualifiedName(stringBuilder);
        }

        public void GetFullyQualifiedName(StringBuilder stringBuilder, int index)
        {
            // this is tail recursable, but is this the best it can be?
            stringBuilder.Insert(index, $".{Name}");
            Namespace.GetFullyQualifiedName(stringBuilder, index);
        }
    }
}

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Noveler.Compiler.CodeDomainObjectModel
{
    internal sealed record NamespaceDefinition(string Name, NamespaceDefinition? ParentNamespace)
    {
        [MemberNotNullWhen(true, nameof(ParentNamespace))]
        public bool HasParentNamespace => ParentNamespace != null;
        public Dictionary<string, TypeDefinition> TypeDefinitions = new();
        //public Dictionary<string, FunctionDefinition> FunctionDefinitions = ();

        public string GetFullyQualifiedName()
        {
            StringBuilder sb = new();
            GetFullyQualifiedName(sb);

            return sb.ToString();
        }

        public void GetFullyQualifiedName(StringBuilder stringBuilder)
        {
            // this is tail recursable, but is this the best it can be?
            stringBuilder.Insert(0, Name);

            if (HasParentNamespace)
            {
                stringBuilder.Insert(0, '.');
                ParentNamespace.GetFullyQualifiedName(stringBuilder);
            }
        }
    }
}
using System.Text;

namespace Noveler.Compiler.CodeDomainObjectModel
{
    internal sealed record NameSpace(string Name, SymbolScope SymbolScope) : DomainObject
    {
        public override IReadOnlyList<DomainObject> GetChildren()
        {
            var children = new List<DomainObject>
            {
                Types,
                Functions
            };

            return children;
        }

        /// <summary>
        /// Gets the types included in this namespace.
        /// </summary>
        public TypeDeclarationCollection Types { get; } = new TypeDeclarationCollection();

        /// <summary>
        /// Gets the functions directly included in this namespace.
        /// </summary>
        public FunctionDeclarationCollection Functions { get; } = new FunctionDeclarationCollection();

        public override string ToString()
        {
            StringBuilder sb = new();

            sb.AppendLine($"Namespace name: {Name}\n" +
                $"\tTypes ({Types.Count}):");

            foreach (var type in Types)
            {
                sb.AppendLine(type.ToString());
            }

            sb.AppendLine();

            sb.AppendLine($"\tNamespace functions ({Functions.Count}):");

            foreach (var function in Functions)
            {
                sb.AppendLine(function.ToString());
            }

            return sb.ToString();
        }
    }
}

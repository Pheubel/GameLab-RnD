using Noveler.Compiler.CodeDomainObjectModel;

namespace Noveler.Compiler.CodeDomainObjectModel
{
    internal sealed record CompilationUnit : DomainObject
    {
        /// <summary>
        /// Gets the namespaces used in this compile unit.
        /// </summary>
        public NameSpaceCollection NameSpaces { get; } = new NameSpaceCollection();

        /// <summary>
        /// Gets the threads this compile unit.
        /// </summary>
        public StoryThreadDeclarationCollection Threads { get; } = new StoryThreadDeclarationCollection();

        public UsingNamespaceCollection UsingNamespaces { get; } = new();

        public static readonly CompilationUnit Placeholder = new();

        public override IReadOnlyList<DomainObject> GetChildren()
        {
            var children = new List<DomainObject>
            {
                NameSpaces,
                Threads
            };

            return children;
        }
    }

internal sealed record UsingNamespaceCollection : DomainObjectCollection<UsingNamespace>;

    internal sealed record UsingNamespace(string NamespaceName) : DomainObject
    {
        public override IReadOnlyList<DomainObject> GetChildren()
        {
            return Array.Empty<DomainObject>();
        }
    }
}

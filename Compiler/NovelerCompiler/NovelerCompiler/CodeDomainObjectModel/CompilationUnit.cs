using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noveler.Compiler.CodeDomainObjectModel.Expressions;
using Noveler.Compiler.CodeDomainObjectModel.Statements;

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

        public static CompilationUnit CreateDefaultIncludeUnit()
        {
            var unit = new CompilationUnit();

            var globalNamespace = NameSpace.CreateGlobalNamespaceInstance();

            var a = new TypeDeclaration("Int8");

            globalNamespace.Types.Add(a);

            unit.NameSpaces.Add(globalNamespace);

            return unit;
        }
    }

    internal sealed record TypeDefinition(string Name)
    {
        public bool IsFullyDefined { get; set; }
        public int SizeInBytes { get; set; }

        public TypeDefinition(string Name, int SizeInBytes)
            : this(Name)
        {
            this.SizeInBytes = SizeInBytes;
            IsFullyDefined = true;
        }
    }

    internal sealed record TypeFieldDefinition(TypeReference FieldType, string FieldName, int OffsetInBytes);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noveler.Compiler.CodeDomainObjectModel.Expressions;
using Noveler.Compiler.CodeDomainObjectModel.Statements;

namespace Noveler.Compiler.CodeDomainObjectModel
{
	internal record CompilationUnit : DomainObject
	{
		public NameSpaceCollection NameSpaces { get; } = new NameSpaceCollection();

		public static readonly CompilationUnit Placeholder = new();
	};

	internal sealed record MainCompilationUnit : CompilationUnit
	{
		StatementCollection EntryStatements { get; } = new StatementCollection();
	};

	internal sealed record NameSpaceCollection : DomainObjectCollection<NameSpace>;

	internal sealed record NameSpaceUses : DomainObject;

	internal sealed record NameSpace(string Name) : DomainObject
	{
		public static NameSpace CreateGlobalNamespaceInstance() => new NameSpace("__global__");

		/// <summary>
		/// Gets the types included in this namespace.
		/// </summary>
		public TypeDeclarationCollection Types { get; } = new TypeDeclarationCollection();

		/// <summary>
		/// Gets the functions directly included in this namespace.
		/// </summary>
		public FunctionDeclarationCollection Functions { get; } = new FunctionDeclarationCollection();

		// TODO: thread collection for jumping to specific text threads or even custom starting points
	};

	internal sealed record TypeMemberCollection() : DomainObjectCollection<TypeMember>;

	internal sealed record FunctionDeclarationCollection() : DomainObjectCollection<FunctionDeclaration>;


}

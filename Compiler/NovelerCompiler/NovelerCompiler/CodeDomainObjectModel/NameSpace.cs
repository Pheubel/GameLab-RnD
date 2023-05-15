namespace Noveler.Compiler.CodeDomainObjectModel
{
	internal sealed record NameSpace(string Name) : DomainObject
	{
		public static NameSpace CreateGlobalNamespaceInstance() => new NameSpace("__global__");

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
	}
}

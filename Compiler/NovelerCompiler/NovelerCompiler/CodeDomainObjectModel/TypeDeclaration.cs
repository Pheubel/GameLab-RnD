namespace Noveler.Compiler.CodeDomainObjectModel
{
    /// <summary>
    /// A type declaration domain object conttaining information about a type's declaration.
    /// </summary>
    /// <param name="Name"> The name of the type being declared.</param>
    internal sealed record TypeDeclaration(string Name) : DomainObject
	{
		/// <summary>
		/// The type dictating how this type should be handled at runtime.
		/// </summary>
		public DeclarationType DeclarationType { get; set; }

		/// <summary>
		/// The fields of the type.
		/// </summary>
		public StructureMemberFieldCollection TypeFieldMembers { get; } = new();

		/// <summary>
		/// The functions of the type.
		/// </summary>
		public StructureMemberFunctionCollection TypeFieldFunctions { get; } = new();

		public StructureConstructorCollection TypeConstructors { get; } = new();

		/// <summary>
		/// The generic parameters of the type.
		/// </summary>
		public TypeParameterCollection TypeParameters { get; } = new();

		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				TypeFieldMembers,
				TypeFieldFunctions,
				TypeParameters
			};

			return children;
		}
	}
}
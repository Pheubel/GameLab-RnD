namespace Noveler.Compiler.CodeDomainObjectModel
{
    /// <summary>
    /// A type declaration domain object conttaining information about a type's declaration.
    /// </summary>
    /// <param name="Name"> The name of the type being declared.</param>
    internal sealed record TypeDeclaration(string Name, SymbolScope SymbolScope) : DomainObject
	{
		public bool IsGenericType => TypeParameters.Count > 0;

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
		public StructureMemberFunctionCollection TypeMemberFunctions { get; } = new();

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
				TypeMemberFunctions,
				TypeParameters
			};

			return children;
		}
	}
}
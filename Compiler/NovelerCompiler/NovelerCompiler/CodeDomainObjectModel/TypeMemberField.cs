namespace Noveler.Compiler.CodeDomainObjectModel
{
	/// <summary>
	/// A field member of a type. 
	/// </summary>
	/// <param name="Name"> The field's name.</param>
	/// <param name="FieldType"> The field's type.</param>
	internal sealed record TypeMemberField(string Name, TypeReference FieldType) : TypeMember(Name)
	{
		/// <summary>
		/// A field member of a type. 
		/// </summary>
		/// <param name="FieldName">The field's name.</param>
		/// <param name="FieldTypeName"> The field's type name.</param>
		public TypeMemberField(string FieldName, string FieldTypeName) : this(FieldName, new TypeReference(FieldTypeName)) { }
	};
}

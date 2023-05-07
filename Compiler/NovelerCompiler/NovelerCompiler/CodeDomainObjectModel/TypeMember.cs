namespace Noveler.Compiler.CodeDomainObjectModel
{
	/// <summary>
	/// Represents a member of a type.
	/// </summary>
	/// <param name="Name"></param>
	/// <see cref="TypeMemberField"/>
	/// <see cref="TypeMemberFunction"/>
	internal abstract record TypeMember(string Name) : DomainObject;
}

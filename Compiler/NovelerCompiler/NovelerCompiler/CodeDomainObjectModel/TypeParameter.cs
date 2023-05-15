namespace Noveler.Compiler.CodeDomainObjectModel
{
	/// <summary>
	/// Type parameter used for generic arguments.
	/// </summary>
	/// <param name="Name"> The name of the type parameter</param>
	internal sealed record TypeParameter(string Name) : DomainObject
	{
		public override IReadOnlyList<DomainObject> GetChildren() => Array.Empty<DomainObject>();
	}
}

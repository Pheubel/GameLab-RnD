using System.Numerics;

namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	internal sealed record ArgumentReferenceExpression(string ParameterName) : Expression
	{
		public override IReadOnlyList<DomainObject> GetChildren() => Array.Empty<DomainObject>();
	}
}

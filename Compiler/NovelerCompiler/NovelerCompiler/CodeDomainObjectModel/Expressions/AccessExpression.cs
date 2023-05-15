using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents an expression that accesses a member from a source expression object.
	/// </summary>
	/// <param name="Source"> The expression returning an object to access.</param>
	/// <param name="Target"> The Symbol to access.</param>
	internal sealed record AccessExpression(Expression Source, Symbol Target) : Expression
	{
		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				Source,
				Target
			};

			return children;
		}
	}
}

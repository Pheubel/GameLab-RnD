using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents an and expression, also known as a bitwise and.
	/// </summary>
	/// <param name="LeftHandExpression"> The left hand side expression of the and expression.</param>
	/// <param name="RightHandExpression"> The right hand side expression of the and expression.</param>
	/// <example>a &amp; b</example>
	internal sealed record AndExpression(Expression LeftHandExpression, Expression RightHandExpression) : Expression
	{
		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				LeftHandExpression,
				RightHandExpression
			};

			return children;
		}
	}
}

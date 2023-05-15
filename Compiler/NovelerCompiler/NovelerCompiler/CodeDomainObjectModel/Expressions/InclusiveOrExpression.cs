using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents an inclusive or expression, also known as bitwise or.
	/// </summary>
	/// <param name="LeftHandExpression"> The left hand side expression of the inclusive or expression.</param>
	/// <param name="RightHandExpression"> The right hand side expression of the inclusive or expression.</param>
	/// <example>a | b</example>
	internal sealed record InclusiveOrExpression(Expression LeftHandExpression, Expression RightHandExpression) : Expression
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

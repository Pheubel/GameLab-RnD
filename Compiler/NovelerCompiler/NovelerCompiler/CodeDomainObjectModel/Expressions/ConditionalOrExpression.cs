using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents a conditional or expression.
	/// </summary>
	/// <param name="LeftHandExpression"> The left hand side expression of the conditional or expression.</param>
	/// <param name="RightHandExpression"> The right hand side expression of the conditional or expression.</param>
	/// <example>a || b</example>
	internal sealed record ConditionalOrExpression(Expression LeftHandExpression, Expression RightHandExpression) : Expression;
}

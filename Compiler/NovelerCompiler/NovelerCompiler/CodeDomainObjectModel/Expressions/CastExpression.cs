using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents a cast from one type to another type
	/// </summary>
	/// <param name="TypeReference"> The type to cast to.</param>
	/// <param name="ValueExpression"> The</param>
	/// <example>(float)a</example>
	internal sealed record CastExpression(TypeReference TypeReference, Expression ValueExpression) : Expression;
}

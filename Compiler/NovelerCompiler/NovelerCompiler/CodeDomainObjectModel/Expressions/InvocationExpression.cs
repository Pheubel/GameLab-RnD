using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents the invocation of a function.
	/// </summary>
	/// <example>a(b,c)</example>
	internal sealed record InvocationExpression(Expression Source, ArgumentCollection Arguments) : Expression;
}

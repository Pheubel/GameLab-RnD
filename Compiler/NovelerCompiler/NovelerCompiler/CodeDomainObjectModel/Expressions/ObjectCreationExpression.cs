using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents an object creation.
	/// </summary>
	/// <param name="ObjectType"></param>
	/// <param name="Arguments"></param>
	/// <example>type(a,b)</example>
	internal record ObjectCreationExpression(TypeReference ObjectType, ArgumentCollection Arguments) : Expression;
}

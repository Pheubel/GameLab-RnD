using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents a "a ? b : c" expression where if the condition of a is met, b will be returned, otherwise C
	/// </summary>
	/// <param name="Condition"> The condition expression to be tested for true or false.</param>
	/// <param name="TrueExpression"> The expression to return if the condition was true.</param>
	/// <param name="FalseExpression"> The expression to return if the condition was false.</param>
	internal record TernaryExpression(Expression Condition, Expression TrueExpression, Expression FalseExpression) : Expression
	{
		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				Condition,
				TrueExpression,
				FalseExpression
			};

			return children;
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents a conditional and expression.
	/// </summary>
	/// <param name="LeftHandExpression"> The left hand side expression of the conditional and expression.</param>
	/// <param name="RightHandExpression"> The right hand side expression of the conditional and expression.</param>
	/// <example>a && b</example>
	internal sealed record ConditionalAndExpression(Expression LeftHandExpression, Expression RightHandExpression) : Expression
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

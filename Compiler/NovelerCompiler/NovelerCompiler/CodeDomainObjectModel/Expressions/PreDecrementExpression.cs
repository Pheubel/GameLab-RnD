﻿namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents a pre-decrement expression.
	/// </summary>
	/// <param name="ValueExpression"> The value expression that gets incremented.</param>
	internal sealed record PreDecrementExpression(Expression ValueExpression) : Expression
	{
		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				ValueExpression
			};

			return children;
		}
	}
}

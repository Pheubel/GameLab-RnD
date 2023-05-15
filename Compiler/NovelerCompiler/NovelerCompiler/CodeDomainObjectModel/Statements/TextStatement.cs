using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noveler.Compiler.CodeDomainObjectModel.Expressions;

namespace Noveler.Compiler.CodeDomainObjectModel.Statements
{
	/// <summary>
	/// Represents a text line in a story that can be formatted.
	/// </summary>
	/// <param name="TextLine"> The line of text with formatting parameters where applicable.</param>
	/// <param name="Expressions"> The expressions that get executed and the results inserted into the resulting string.</param>
	internal sealed record TextStatement(string TextLine, ExpressionCollection Expressions) : Statement
	{
		/// <summary>
		/// Determines if the text line should be treated as a format string.
		/// </summary>
		public bool IsFormattedLine => Expressions.Count > 0;

		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				Expressions
			};

			return children;
		}
	}
}

using Noveler.Compiler.Grammar;

namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	internal sealed record BooleanLiteralExpression : Expression
	{
		public BooleanLiteralExpression(string ValueString)
		{
			this.ValueString = ValueString;
			this.Value = ValueString == NovelerLexer.DefaultVocabulary.GetLiteralName(NovelerLexer.TRUE) || (ValueString == NovelerLexer.DefaultVocabulary.GetLiteralName(NovelerLexer.FALSE) ? false :
				throw new ArgumentException("argument is not a valid option", nameof(ValueString)));
		}

		public BooleanLiteralExpression(bool Value)
		{
			this.Value = Value;
			this.ValueString = Value.ToString();
		}

		public bool Value { get; private init; }
		public string ValueString { get; private init; }
	}
}

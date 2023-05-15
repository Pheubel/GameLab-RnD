namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	/// <summary>
	/// Represents a floating point literal in a script
	/// </summary>
	/// <example>21.4</example>
	internal sealed record FloatingPointLiteralExpression : Expression
	{
		public double Value { get; init; }

		public FloatingPointLiteralExpression(string valueString)
		{
			if (!double.TryParse(valueString, System.Globalization.NumberStyles.Float, null, out var value))
				throw new ArgumentException("Could not parse floating point literal", nameof(valueString));

			Value = value;
		}

		public FloatingPointLiteralExpression GetNegated() => this with { Value = this.Value * -1 };

		public double AsDouble() => Value;
		public float AsFloat() => (float)Value;
	}
}

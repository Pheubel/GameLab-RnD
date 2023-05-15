using System.Reflection.Metadata.Ecma335;

namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{

	/// <summary>
	/// Represents an integer literal in a script
	/// </summary>
	/// <example>14</example>
	internal sealed record IntegerLiteralExpression : Expression
	{
		public ulong Value { get; init; }

		private IntegerLiteralExpression()
		{
			Value = 0;
		}

		public static IntegerLiteralExpression FromDecimal(string valueString)
		{
			return ulong.TryParse(valueString, out var value) ?
			 new IntegerLiteralExpression() { Value = value } :
			 throw new ArgumentOutOfRangeException(nameof(valueString), "integer literal is too large.");
		}

		public static IntegerLiteralExpression FromHex(string valueString)
		{
			return ulong.TryParse(valueString, System.Globalization.NumberStyles.HexNumber, null, out var value) ?
				new IntegerLiteralExpression() { Value = value } :
				throw new ArgumentOutOfRangeException(nameof(valueString), "integer literal is too large.");
		}

		public static IntegerLiteralExpression FromBinary(string valueString)
		{
			int cursor = valueString.Length - 1;
			int exponent = 0;
			char currentChar;

			ulong value = 0;

			do
			{
				currentChar = valueString[cursor];
				cursor--;

				if (currentChar == '_')
					continue;

				value |= (ulong)valueString[cursor] << exponent;
				exponent++;

				if (exponent == 64)
					throw new ArgumentOutOfRangeException(nameof(valueString), "integer literal is too large.");
			} while (currentChar != 'b' || currentChar != 'B');

			return new IntegerLiteralExpression()
			{
				Value = value
			};
		}

		public IntegerLiteralExpression GetNegated() => this with { Value = ~this.Value + 1 };

		public ulong AsUInt64()
		{
			return Value;
		}

		public long AsInt64()
		{
			return (long)Value;
		}

		public uint AsUInt32()
		{
			return (uint)Value;
		}

		public int AsInt32()
		{
			return (int)Value;
		}

		public ushort AsUInt16()
		{
			return (ushort)Value;
		}

		public short AsInt16()
		{
			return (short)Value;
		}

		public byte AsUInt8()
		{
			return (byte)Value;
		}

		public sbyte AsInt8()
		{
			return (sbyte)Value;
		}

		public override IReadOnlyList<DomainObject> GetChildren() => Array.Empty<DomainObject>();
	}
}

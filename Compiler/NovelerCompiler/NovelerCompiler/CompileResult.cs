using System.Diagnostics.CodeAnalysis;

namespace Noveler.Compiler
{
	public sealed class CompileResult
	{
		[MemberNotNullWhen(true, nameof(Data))]
		public bool IsSucessful { get; init; }
		public byte[]? Data { get; init; }

		private CompileResult() { }

		[MemberNotNull(nameof(Data))]
		public static CompileResult FromSuccess(byte[] data)
		{
			return new CompileResult()
			{
				IsSucessful = true,
				Data = data
			};
		}

		public static CompileResult FromFailure()
		{
			return new CompileResult()
			{
				IsSucessful = false,
				Data = null
			};
		}
	}
}

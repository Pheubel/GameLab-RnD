using SymbolDefinition = OneOf.OneOf<OneOf.Types.Unknown>;

namespace Noveler.Compiler
{
	internal sealed class SymbolInfo
	{
		public SymbolInfo()
		{
			Definition = new OneOf.Types.Unknown();
		}

		public SymbolDefinition Definition { get; private set; }
		public bool IsDefined => !Definition.IsT0;

		public bool SetDefinition(SymbolDefinition definition)
		{
			if (definition.IsT0)
				throw new ArgumentException("Cannot set definition to unknown.", nameof(definition));

			if (Definition.IsT0)
				return false;

			return true;
		}
	}
}

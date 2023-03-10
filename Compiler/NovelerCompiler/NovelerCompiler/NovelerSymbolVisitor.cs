using Noveler.Compiler.Grammar;

namespace Noveler.Compiler
{
	internal sealed class NovelerSymbolVisitor : NovelerParserBaseVisitor<Dictionary<string, SymbolInfo>?>
	{
		public override Dictionary<string, SymbolInfo> VisitStory(NovelerParser.StoryContext context)
		{
			Dictionary<string, SymbolInfo> symbolTable = new(128);

			var typeVisitor = new TypeVisitor(symbolTable);
			typeVisitor.VisitStory(context);

			return symbolTable;
		}

		private sealed class TypeVisitor : NovelerParserBaseVisitor<nint>
		{
			private readonly Dictionary<string, SymbolInfo> _symbolTable;

			public TypeVisitor(Dictionary<string, SymbolInfo> symbolTable)
			{
				_symbolTable = symbolTable;
			}

			public override nint VisitType_name(NovelerParser.Type_nameContext context)
			{
				var typeName = context.GetText();

				Console.WriteLine($"found type_name: {typeName}");
				_symbolTable.TryAdd(typeName, new SymbolInfo());

				return default;
			}

			public override nint VisitIdentifier(NovelerParser.IdentifierContext context)
			{
				var identifierName = context.GetText();

				Console.WriteLine($"found type_name: {identifierName} ({context.Start.Line}:{context.Start.Column} ... {context.Stop.Line}:{context.Stop.StopIndex})");
				_symbolTable.TryAdd(identifierName, new SymbolInfo());
				return default;
			}
		}
	}
}

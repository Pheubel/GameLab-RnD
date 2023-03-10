using Antlr4.Runtime.Misc;
using Microsoft.CodeAnalysis;
using Noveler.Compiler.Grammar;
using OneOf.Types;

namespace Noveler.Compiler
{
	internal sealed class NovelerFirstPassVisitor : NovelerParserBaseVisitor<FirstPassResult?>
	{
		private readonly Optional<FileInfo> _fileInfo;

		public NovelerFirstPassVisitor(Optional<FileInfo> fileInfo)
		{
			_fileInfo = fileInfo;
		}

		public override FirstPassResult VisitStory(NovelerParser.StoryContext context)
		{
			FirstPassResult result = new();

			var typeVisitor = new SymbolVisitor(result.SymbolTable);
			typeVisitor.VisitStory(context);

			foreach (var importedFile in context.import_statement())
			{
				HandleImport(importedFile, result, _fileInfo, (_) => { });
			}

			return result;
		}

		private static void HandleImport(NovelerParser.Import_statementContext importContext, FirstPassResult result, Optional<FileInfo> fileInfo, Action<Error<int>> onError)
		{
			var filePath = importContext.String_Literal().GetText()[1..^1];
			Optional<FileInfo> importedFileInfo = default;

			// attempt relative import first
			if (fileInfo.HasValue)
			{
				importedFileInfo = new FileInfo(Path.Combine(fileInfo.Value.DirectoryName ?? string.Empty, filePath));
			}

			// if there is no relative match, attempt absolute match
			if (!importedFileInfo.HasValue || !importedFileInfo.Value.Exists)
			{
				importedFileInfo = new FileInfo(filePath);
			}

			result.AddOrGetImport(importedFileInfo.Value, out bool alreadyAdded).Switch(
				import =>
				{
					var importVisitor = new ImportVisitor(importedFileInfo.Value, result);
					importVisitor.VisitImported_file(import);
				},
				error => onError(error)
				);
		}

		private sealed class ImportVisitor : NovelerParserBaseVisitor<nint>
		{
			private readonly FileInfo _file;
			private readonly FirstPassResult _result;

			public ImportVisitor(FileInfo file, FirstPassResult result)
			{
				_file = file;
				_result = result;
			}

			public override nint VisitImport_statement(NovelerParser.Import_statementContext context)
			{
				HandleImport(context, _result, _file, (_) => { });

				return default;
			}
		}

		private sealed class SymbolVisitor : NovelerParserBaseVisitor<nint>
		{
			private readonly Dictionary<string, SymbolInfo> _symbolTable;

			public SymbolVisitor(Dictionary<string, SymbolInfo> symbolTable)
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

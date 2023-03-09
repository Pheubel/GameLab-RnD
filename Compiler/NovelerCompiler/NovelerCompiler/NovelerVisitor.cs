using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Microsoft.CodeAnalysis;
using Noveler.Compiler.Grammar;

namespace Noveler.Compiler
{
	internal sealed class NovelerVisitor : NovelerParserBaseVisitor<StorySyntaxTree?>
	{
		private Optional<FileInfo> _fileInfo;

		private Dictionary<string, object> _symbolTable;
		private Dictionary<string, object> _functionTable;
		private Dictionary<string, object> _typeTable;
		private Dictionary<string, object> _stubTable;

		private List<string> _stringTable;

		private HashSet<string> _visitedFiles;

		public NovelerVisitor(Optional<FileInfo> fileInfo)
		{
			_fileInfo = fileInfo;

			_symbolTable = new();
			_functionTable = new();
			_typeTable = new();
			_stubTable = new();
			_visitedFiles = new();
			_stringTable = new(128);
		}

		private NovelerVisitor(Optional<FileInfo> fileInfo,
							   Dictionary<string, object> symbolTable,
							   Dictionary<string, object> functionTable,
							   Dictionary<string, object> typeTable,
							   Dictionary<string, object> stubTable,
							   HashSet<string> visitedFiles,
							   List<string> stringTable)
		{
			_fileInfo = fileInfo;
			_symbolTable = symbolTable;
			_functionTable = functionTable;
			_typeTable = typeTable;
			_stubTable = stubTable;
			_visitedFiles = visitedFiles;
			_stringTable = stringTable;
		}

		public IReadOnlyDictionary<string, object> SymbolTable => _symbolTable;
		public IReadOnlyDictionary<string, object> FunctionTable => _functionTable;
		public IReadOnlyDictionary<string, object> TypeTable => _typeTable;
		public IReadOnlyDictionary<string, object> StubTable => _stubTable;
		public IReadOnlyList<string> StringTable => _stringTable;

		/// <summary>
		/// Visits the statements importing additional files.
		/// </summary>
		/// <param name="context"> The parse tree.</param>
		/// <returns> Since this does not participate in building the story tree, null is returned.</returns>
		public override StorySyntaxTree? VisitImport_statement(NovelerParser.Import_statementContext context)
		{
			var importedFile = context.String_Literal().GetText()[1..^1];

			Optional<FileInfo> importedFileInfo = default;

			// attempt relative import first
			if (_fileInfo.HasValue)
			{
				importedFileInfo = new FileInfo(Path.Combine(_fileInfo.Value.DirectoryName ?? string.Empty, importedFile));
			}

			// if there is no relative match, attempt absolute match
			if (!importedFileInfo.HasValue || !importedFileInfo.Value.Exists)
			{
				importedFileInfo = new FileInfo(importedFile);
			}

			var info = importedFileInfo.Value;

			// report errors when imported file can't be found
			if (!info.Exists)
			{
				// TODO: signal failure for importing 

				return default;
			}

			// only plunge into the included file once
			if (_visitedFiles.Add(info.FullName))
			{
				using FileStream importedFileStream = info.OpenRead();

				AntlrInputStream inputStream = new AntlrInputStream(importedFileStream);
				NovelerLexer novelerLexer = new NovelerLexer(inputStream);
				CommonTokenStream commonTokenStream = new CommonTokenStream(novelerLexer);
				NovelerParser novelerParser = new NovelerParser(commonTokenStream);
				NovelerParser.Imported_fileContext importContext = novelerParser.imported_file();

				NovelerVisitor visitor = new NovelerVisitor(
					fileInfo: info,
					symbolTable: _symbolTable,
					functionTable: _functionTable,
					typeTable: _typeTable,
					stubTable: _stubTable,
					visitedFiles: _visitedFiles,
					stringTable: _stringTable);

				visitor.Visit(context);
			}

			return default;
		}
	}


	// TODO: implement and move to separate files
	internal abstract class SyntaxTree
	{

	}

	internal sealed class StorySyntaxTree : SyntaxTree
	{

	}

	internal sealed class FunctionSyntaxTree : SyntaxTree
	{

	}
}

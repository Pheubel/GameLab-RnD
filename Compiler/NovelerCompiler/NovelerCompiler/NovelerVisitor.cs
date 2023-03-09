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
	internal sealed class NovelerVisitor : NovelerParserBaseVisitor<object?>
	{
		private Optional<FileInfo> _fileInfo;

		private Dictionary<string, object> _symbolTable;
		private Dictionary<string, object> _functionTable;
		private Dictionary<string, object> _typeTable;
		private Dictionary<string, object> _stubTable;

		private HashSet<string> _visitedFiles;

		public NovelerVisitor(Optional<FileInfo> fileInfo)
		{
			_fileInfo = fileInfo;

			_symbolTable = new();
			_functionTable = new();
			_typeTable = new();
			_stubTable = new();
			_visitedFiles = new();
		}

		private NovelerVisitor(Optional<FileInfo> fileInfo,
							   Dictionary<string, object> symbolTable,
							   Dictionary<string, object> functionTable,
							   Dictionary<string, object> typeTable,
							   Dictionary<string, object> stubTable,
							   HashSet<string> visitedFiles)
		{
			_fileInfo = fileInfo;
			_symbolTable = symbolTable;
			_functionTable = functionTable;
			_typeTable = typeTable;
			_stubTable = stubTable;
			_visitedFiles = visitedFiles;
		}

		public IReadOnlyDictionary<string, object> SymbolTable => _symbolTable;
		public IReadOnlyDictionary<string, object> FunctionTable => _functionTable;
		public IReadOnlyDictionary<string, object> TypeTable => _typeTable;
		public IReadOnlyDictionary<string, object> StubTable => _stubTable;

		public override object VisitImport_statement(NovelerParser.Import_statementContext context)
		{
			var literal = context.String_Literal().GetText()[1..^1];

			return null;
		}
	}
}

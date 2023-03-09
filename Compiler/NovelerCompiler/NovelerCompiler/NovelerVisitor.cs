using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noveler.Compiler.Grammar;

namespace Noveler.Compiler
{
	internal sealed class NovelerVisitor : NovelerParserBaseVisitor<object?>
	{
		private Dictionary<string, object> _symbolTable = new();
		private Dictionary<string, object> _functionTable = new();
		private Dictionary<string, object> _typeTable = new();
		private Dictionary<string, object> _stubTable = new();

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

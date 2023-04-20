using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Microsoft.CodeAnalysis;
using Noveler.Compiler.Grammar;
using Noveler.Compiler.SymbolTypes;
using OneOf.Types;
using System.IO;
using System.Reflection;
using System.Text;

using Unit = System.IntPtr;

namespace Noveler.Compiler
{
	internal sealed class NovelerFirstPassVisitorTakeOne : NovelerParserBaseVisitor<FirstPassResult?>
	{
		private readonly Optional<FileInfo> _fileInfo;

		public NovelerFirstPassVisitorTakeOne(Optional<FileInfo> fileInfo)
		{
			_fileInfo = fileInfo;
		}

		public override FirstPassResult VisitStory(NovelerParser.StoryContext context)
		{
			FirstPassResult result = new();

			var importVisitor = new ImportVisitor(_fileInfo, result);
			foreach (var importedStatement in context.import_statement())
			{
				importVisitor.VisitImport_statement(importedStatement);
			}

			var scriptVisitor = new ScriptVisitor(result);
			foreach (var storySegment in context.story_segment())
			{
				scriptVisitor.VisitStory_segment(storySegment);
			}

			return result;
		}

		private sealed class ImportVisitor : NovelerParserBaseVisitor<Unit>
		{
			private readonly Optional<FileInfo> _file;
			private readonly FirstPassResult _result;

			public ImportVisitor(Optional<FileInfo> file, FirstPassResult result)
			{
				_file = file;
				_result = result;
			}

			public override Unit VisitImport_statement(NovelerParser.Import_statementContext context)
			{
				var filePath = context.String_Literal().GetText()[1..^1];
				Optional<FileInfo> importedFileInfo = default;

				// attempt relative import first
				if (_file.HasValue)
				{
					importedFileInfo = new FileInfo(Path.Combine(_file.Value.DirectoryName ?? string.Empty, filePath));
				}

				// if there is no relative match, attempt absolute match
				if (!importedFileInfo.HasValue || !importedFileInfo.Value.Exists)
				{
					importedFileInfo = new FileInfo(filePath);
				}

				_result.AddOrGetImport(importedFileInfo.Value, out bool alreadyAdded).Switch(
					import =>
					{
						var importVisitor = new ImportVisitor(importedFileInfo.Value, _result);
						importVisitor.VisitImported_file(import);
					},
					error =>
					{
						// TODO: handle errors
					}
					);

				return default;
			}

			public override Unit VisitImported_file(NovelerParser.Imported_fileContext context)
			{
				foreach (var importStatement in context.import_statement())
				{
					VisitImport_statement(importStatement);
				}


				var scriptVisitor = new ScriptVisitor(_result);
				foreach (var importedContent in context.imported_content())
				{
					scriptVisitor.VisitImported_content(importedContent);
				}

				return default;
			}
		}

		private sealed class ScriptVisitor : NovelerParserBaseVisitor<Unit>
		{
			private FirstPassResult _result;

			public SymbolTable SymbolTable => _result.SymbolTable;

			public ScriptVisitor(FirstPassResult result)
			{
				_result = result;
			}

			public override Unit VisitMethod_declaration(NovelerParser.Method_declarationContext context)
			{
				// TODO: fully implement

				SymbolVisitor symbolVisitor = new SymbolVisitor(SymbolTable);
				SymbolInfo functionSymbol = symbolVisitor.VisitMethod_header(context.method_header());

				var functionInfo = functionSymbol.Type.AsT3;

				functionInfo.

				return default;
			}

			public override Unit VisitVariable_declare_assign(NovelerParser.Variable_declare_assignContext context)
			{
				SymbolVisitor symbolVisitor = new SymbolVisitor(SymbolTable);
				var symbol = symbolVisitor.VisitVariable_declare(context.variable_declare());

				// TODO: put the assignment expression into a syntax tree

				var expressionContext = context.GetRuleContext<NovelerParser.ExpressionContext>(0);

				return default;
			}

			public override Unit VisitVariable_declare(NovelerParser.Variable_declareContext context)
			{
				SymbolVisitor symbolVisitor = new SymbolVisitor(SymbolTable);
				symbolVisitor.VisitVariable_declare(context);

				return default;
			}

			public override Unit VisitText_segment(NovelerParser.Text_segmentContext context)
			{
				var textVisitor = new TextVisitor(SymbolTable);
				var text = textVisitor.VisitText_segment(context);

				// TODO: add text to string table and add entry to stub tree

				return default;
			}
		}

		private sealed class SymbolVisitor : NovelerParserBaseVisitor<SymbolInfo>
		{
			readonly SymbolTable _symbolTable;

			public SymbolVisitor(SymbolTable symbolTable)
			{
				_symbolTable = symbolTable;
			}

			public override SymbolInfo VisitVariable_declare(NovelerParser.Variable_declareContext context)
			{
				// TODO: fix this with actual type info
				var typeInfo = new StructureInfo("test", 4, default);

				var symbol = new SymbolInfo(context.Simple_Identifier().GetText())
				{
					Type = typeInfo
				};

				if (!_symbolTable.Insert(symbol))
				{
					// TODO: handle duplicate symbol
				}

				return symbol;
			}

			/// <summary>
			/// Visits the header of a method and extracts the name, return type and arguments.
			/// </summary>
			public override SymbolInfo VisitMethod_header(NovelerParser.Method_headerContext context)
			{
				var symbol = new SymbolInfo(context.identifier().GetText());

				var parameterContexts = context.parameter_list().GetRuleContexts<NovelerParser.ParameterContext>();
				ParameterInfo[] parameterList = new ParameterInfo[parameterContexts.Length];

				for (int i = 0; i < parameterContexts.Length; i++)
				{
					string parameterName = parameterContexts[i].identifier().GetText();
					string parameterTypeName = parameterContexts[i].type().GetText();

					parameterList[i] = new ParameterInfo(parameterName, parameterTypeName);

					var symbolEntry = _symbolTable.LookUp(parameterTypeName);

					if (!symbolEntry.HasValue)
					{
						parameterList[i].TypeInfo = new UnknownInfo(parameterTypeName);
					}
					else
					{
						if (!symbolEntry.Value.Type.IsT1)
						{
							// TODO: signal invalid symbol info type
						}
						else
						{
							parameterList[i].TypeInfo = symbolEntry.Value.Type.AsT1;
						}
					}
				}

				FunctionInfo functionInfo = new FunctionInfo(symbol.Name, parameterList);
				symbol.Type = functionInfo;

				if (!_symbolTable.Insert(symbol))
				{
					// TODO: handle duplicate symbol
				}

				return symbol;
			}
		}

		private sealed class TextVisitor : NovelerParserBaseVisitor<TextLine>
		{
			public SymbolTable SymbolTable { get; init; }

			public TextVisitor(SymbolTable symbolTable)
			{
				SymbolTable = symbolTable;
			}

			public override TextLine VisitText_segment(NovelerParser.Text_segmentContext context)
			{
				StringBuilder sb = new StringBuilder();
				var argumentList = PooledList<SymbolInfo>.Rent(4);

				// handle continued lines
				foreach (var continuedTextLine in context.continued_text_line())
				{
					var lastToken = HandleTextSegments(continuedTextLine.text_line_segment(), sb, argumentList);

					int spacing = continuedTextLine.PIPE().Symbol.StartIndex - (lastToken.StopIndex + 1);

					if (spacing > 0)
						sb.Append(' ', spacing - 1);
				}

				// handle last line
				HandleTextSegments(context.text_line().text_line_segment(), sb, argumentList);

				// clean up polled list
				SymbolInfo[] arguments = argumentList.ToNewArray();
				PooledList<SymbolInfo>.Return(argumentList);

				return new TextLine(sb.ToString(), arguments);
			}

			/// <summary>
			/// Handles the text segments to stich them together into a single string inside the string builder.
			/// </summary>
			/// <param name="segmentContexts"> The segment contexts to be stiched together.</param>
			/// <param name="builder"> The string builder the result will be appended to.</param>
			/// <param name="argumentList"> The list of arguments for a formatted string any arguments will be appended to.</param>
			/// <returns> The final token that was touched by the handler.</returns>
			private IToken HandleTextSegments(NovelerParser.Text_line_segmentContext[] segmentContexts, StringBuilder builder, PooledList<SymbolInfo> argumentList)
			{
				IToken? lastToken = default;

				for (int i = 0; i < segmentContexts.Length; i++)
				{
					var segment = segmentContexts[i];

					if (lastToken is not null)
					{
						var spacingCharacters = segment.Start.InputStream.GetText(new Interval(lastToken.StopIndex + 1, segment.Start.StartIndex - 1));

						builder.Append(spacingCharacters);
					}

					if (segment.escaped_text_segment_character() is not null)
					{
						// TODO: handle escaped characters as some have special functions
					}
					else if (segment.interpolated_value() is not null)
					{
						builder.Append($"{{{argumentList.Count}}}");
						var identifier = segment.interpolated_value().expression();

						var lookup = SymbolTable.LookUp(identifier.GetText());

						if (lookup.HasValue)
						{
							var symbol = lookup.Value;
							argumentList.Add(symbol);
						}
						else
						{
							// TODO: handle symbol not found
						}
					}
					else
					{
						builder.Append(segment.GetText());
					}

					lastToken = segment.Stop;
				}

				// since there will be at least 1 segment context, lastToken will always be set.
				return lastToken!;
			}
		}

		private record TextLine(string Text, SymbolInfo[] Arguments)
		{
			public bool IsFormatted => Arguments.Length > 0;
		}
	}
}

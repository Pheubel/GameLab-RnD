using Noveler.Compiler.Grammar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;
using System.CodeDom.Compiler;
using Antlr4.Runtime.Misc;
using Noveler.Compiler.CodeDomainObjectModel;
using Antlr4.Runtime;
using static Noveler.Compiler.Grammar.NovelerParser;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.Text;
using Noveler.Compiler.CodeDomainObjectModel.Expressions;
using Noveler.Compiler.CodeDomainObjectModel.Statements;

namespace Noveler.Compiler
{
	internal sealed class NovelerVisitor : NovelerParserBaseVisitor<object>
	{
		public NovelerVisitor(string sourcePath)
		{
			SourcePath = sourcePath;
			_nameSpaceScopeStack = new();

		}

		public string SourcePath { get; }
		private Stack<NameSpace> _nameSpaceScopeStack;

		/// <summary>
		/// Visits the main entry point of a story.
		/// </summary>
		/// <param name="context"></param>
		/// <returns> <see cref="Dictionary{string, CompilationUnit}"/></returns>
		public override object VisitStory([NotNull] NovelerParser.StoryContext context)
		{
			CompilationUnit mainCompileUnit = new();

			if (_nameSpaceScopeStack.Count > 0)
			{
				throw new Exception("namespace scope stack is leaking, this is a bug.");
			}

			_nameSpaceScopeStack.Push(NameSpace.CreateGlobalNamespaceInstance());

			Dictionary<string, CompilationUnit> units = new()
			{
				{ SourcePath, mainCompileUnit }
			};

			var imports = context.import_statement();
			HandleImportStatements(imports, units);

			var segments = context.story_segment();

			StatementCollection statements = new();
			HandleStorySegments(segments, statements);

			return units;
		}

		private void HandleStorySegments(Story_segmentContext[] segments, StatementCollection statements)
		{
			for (int i = 0; i < segments.Length; i++)
			{
				var segment = segments[i].GetChild(0);

				switch (segment.Payload)
				{
					case Text_segmentContext textSegmentContext:
						TextStatement statement = (TextStatement)this.VisitText_segment(textSegmentContext);

						statements.Add(statement);
						//TODO: do something with this
						break;

					case Embed_statementContext embedStatementContext:

						//: embedded_variable_declaration
						//| embedded_code_block
						//| embedded_if_statement
						//| choice_block
						//;

						HandleEmbeddedStatement(embedStatementContext, statements);

						break;

					// empty segments can be skipped
					case Empty_segmentContext:
						continue;

					default:
						throw new Exception();
				}

				//: embed_statement
				//| text_segment
				//| empty_segment
			}
		}

		private void HandleEmbeddedStatement(Embed_statementContext embedStatementContext, StatementCollection statements)
		{
			var embeddedStatement = embedStatementContext.GetChild<RuleContext>(0);

			switch (embeddedStatement.Payload)
			{
				case Embedded_variable_declarationContext embeddedVariableDeclaration:
					statements.Add((Statement)VisitEmbedded_variable_declaration(embeddedVariableDeclaration));
					break;

				case Embedded_if_statementContext embeddedIfStatement:
					statements.Add((Statement)Visit(embeddedIfStatement));
					break;

				case Choice_blockContext choiceBlockStatement:
					// TODO
					break;

				case Embedded_code_blockContext embedCodeBlockContext:
					// TODO
					break;

				default:
					throw new Exception($"unexpected embedded statement: rule {embeddedStatement.RuleIndex}");
			}
		}

		public override object VisitEmbedded_if([NotNull] Embedded_ifContext context)
		{
			var expression = (Expression)Visit(context.if_statement_if_segment());
			var condition = new ConditionStatement(expression);
			HandleStorySegments(context.story_segment(), condition.TrueStatements);

			return condition;
		}

		public override object VisitEmbedded_if_else([NotNull] Embedded_if_elseContext context)
		{
			var condition = (ConditionStatement)Visit(context.embedded_if_statement());

			var currentCondition = condition;
			foreach (var elseIfContext in context.embedded_else_if_statement())
			{
				var newCondition = (ConditionStatement)VisitEmbedded_else_if_statement(elseIfContext);
				condition.FalseStatements.Add(newCondition);

				currentCondition = newCondition;
			}

			HandleStorySegments(context.story_segment(), currentCondition.FalseStatements);

			return condition;
		}

		public override object VisitEmbedded_else_if_statement([NotNull] Embedded_else_if_statementContext context)
		{
			var expression = (Expression)Visit(context.if_statement_if_segment());
			var condition = new ConditionStatement(expression);
			HandleStorySegments(context.story_segment(), condition.TrueStatements);

			return condition;
		}

		//public override object VisitEmbedded_if_else([NotNull] Embedded_if_elseContext context)
		//{
		//	var condition = (ConditionStatement)Visit(context.embedded_if_statement());

		//	HandleStorySegments(context.story_segment(), condition.FalseStatements);

		//	return condition;
		//}

		//public override object VisitEmbedded_if_else_if([NotNull] Embedded_if_else_ifContext context)
		//{
		//	var condition = (ConditionStatement)Visit(context.embedded_if_statement());

		//	//var  = context.if_statement_if_segment();


		//	//var falseStatemtent = (ConditionStatement)Visit(context.embedded_if_statement());
		//	//condition.FalseStatements.Add(falseStatemtent);

		//	return condition;
		//}

		public override object VisitBooleanExpression([NotNull] BooleanExpressionContext context)
		{
			return Visit(context.expression());
		}

		/// <summary>
		/// Visits a variable declaration statement embedded in the story scope.
		/// </summary>
		/// <param name="context"></param>
		/// <returns> A variable declaration statement.</returns>
		public override object VisitEmbedded_variable_declaration([NotNull] Embedded_variable_declarationContext context)
		{
			var declarationContext = context.variable_declare();

			if (declarationContext != null)
				return VisitVariable_declare(declarationContext);

			var declarationAssignmentContext = context.variable_declare_assign();
			if (declarationAssignmentContext != null)
				return VisitVariable_declare_assign(declarationAssignmentContext);

			throw new Exception($"Unexpected embedded variable declaration: rule {context.RuleIndex}");
		}

		/// <summary>
		/// Visits a variable declaration statement.
		/// </summary>
		/// <param name="context"></param>
		/// <returns> A variable declaration statement.</returns>
		public override object VisitVariable_declaration([NotNull] Variable_declarationContext context)
		{
			var declarationContext = context.variable_declare();

			if (declarationContext != null)
				return VisitVariable_declare(declarationContext);

			var declarationAssignmentContext = context.variable_declare_assign();
			if (declarationAssignmentContext != null)
				return VisitVariable_declare_assign(declarationAssignmentContext);

			throw new Exception($"Unexpected embedded variable declaration: rule {context.RuleIndex}");
		}

		/// <summary>
		/// Visits the point where a variable gets declared with a type.
		/// </summary>
		/// <param name="context"></param>
		/// <returns> A variable declaration statement.</returns>
		public override object VisitVariable_declare([NotNull] Variable_declareContext context)
		{
			return new VariableDeclarationStatement(
				VariableName: context.Simple_Identifier().GetText(),
				TypeReference: new TypeReference(context.type().GetText())
				);
		}

		/// <summary>
		/// Visits a variable declaration followed by an assignment
		/// </summary>
		/// <param name="context"></param>
		/// <returns> A statement containing the the variable declaration and assignment statement.</returns>
		public override object VisitVariable_declare_assign([NotNull] Variable_declare_assignContext context)
		{
			var variableDeclaration = (VariableDeclarationStatement)VisitVariable_declare(context.variable_declare());
			var expression = (Expression)Visit(context.expression());

			return new VariableDeclarationAssignmentStatement(
				VariableDeclaration: variableDeclaration,
				InitializationExpression: expression
				);
		}

		public override object VisitImported_file([NotNull] Imported_fileContext context)
		{
			if (_nameSpaceScopeStack.Count > 0)
			{
				throw new Exception("namespace scope stack is leaking, this is a bug.");
			}

			_nameSpaceScopeStack.Push(NameSpace.CreateGlobalNamespaceInstance());


			// TODO
			return new CompilationUnit();
		}

		/// <summary>
		/// Visits a text segment and returns a text statement with the text string and possible formatting arguments.
		/// </summary>
		/// <param name="context"> The context in which the text is encountered in.</param>
		/// <returns> A text statement.</returns>
		public override object VisitText_segment([NotNull] Text_segmentContext context)
		{
			StringBuilder sb = new StringBuilder();
			ExpressionCollection expressions = new ExpressionCollection();

			// handle continued lines
			foreach (var continuedTextLine in context.continued_text_line())
			{
				var lastToken = HandleTextSegments(continuedTextLine.text_line_segment(), sb, expressions);

				int spacing = continuedTextLine.PIPE().Symbol.StartIndex - (lastToken.StopIndex + 1);

				if (spacing > 0)
					sb.Append(' ', spacing - 1);
			}

			// handle last line
			HandleTextSegments(context.text_line().text_line_segment(), sb, expressions);


			return new TextStatement(sb.ToString(), expressions);
		}

		/// <summary>
		/// Handles the text segments to stich them together into a single string inside the string builder.
		/// </summary>
		/// <param name="segmentContexts"> The segment contexts to be stiched together.</param>
		/// <param name="builder"> The string builder the result will be appended to.</param>
		/// <param name="argumentList"> The list of arguments for a formatted string any arguments will be appended to.</param>
		/// <returns> The final token that was touched by the handler.</returns>
		private IToken HandleTextSegments(NovelerParser.Text_line_segmentContext[] segmentContexts, StringBuilder builder, ExpressionCollection expressions)
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
					// TODO: turn this into an expression

					builder.Append($"{{{expressions.Count}}}");
					var expressionContext = segment.interpolated_value().expression();

					var expression = (Expression)this.VisitExpression(expressionContext);

					expressions.Add(expression);
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

		/// <summary>
		/// Handles the set up of compilation units when an import is encountered.
		/// </summary>
		/// <param name="imports"> The contexts indicating the import statements containing the files to be imported.</param>
		/// <param name="units"> The collection of compilation units gathered in the compilation process.</param>
		private void HandleImportStatements(Import_statementContext[] imports, Dictionary<string, CompilationUnit> units)
		{
			// TODO: see if this can be done in parallel to speed things up

			foreach (var import in imports)
			{
				var targetFile = import.String_Literal().GetText()[1..^1];

				string targetPath;
				if (Path.IsPathFullyQualified(targetFile))
					targetPath = targetFile;
				else
					targetPath = Path.GetFullPath(targetFile, Path.GetDirectoryName(SourcePath) ?? string.Empty);

				// skip if compilation unit already exists.
				if (units.ContainsKey(targetPath))
					continue;

				var importCompileUnit = CompilationUnit.Placeholder;

				// reserve spot in dictionary to prevent duplication.
				units.Add(targetPath, importCompileUnit);

				var importVisitor = new NovelerVisitor(targetPath);

				using var importFileStream = File.OpenRead(targetPath);

				AntlrInputStream inputStream = new AntlrInputStream(importFileStream);
				NovelerLexer novelerLexer = new NovelerLexer(inputStream);
				CommonTokenStream commonTokenStream = new CommonTokenStream(novelerLexer);
				NovelerParser novelerParser = new NovelerParser(commonTokenStream);

				var importedFileContext = novelerParser.imported_file();

				// overwrite the placeholder with the complete compilation unit
				units[targetPath] = (CompilationUnit)importVisitor.VisitImported_file(importedFileContext);
			}
		}
	}
}

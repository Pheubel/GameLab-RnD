﻿using Noveler.Compiler.Grammar;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

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

		private readonly Stack<NameSpace> _nameSpaceScopeStack;

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
					HandleCodeBlock(embedCodeBlockContext.code_block(), statements);
					break;

				default:
					throw new Exception($"unexpected embedded statement: rule {embeddedStatement.RuleIndex}");
			}
		}

		private void HandleCodeBlock(Code_blockContext codeBlockContext, StatementCollection statements)
		{
			var statementContexts = codeBlockContext.statement();

			foreach (var statementContext in statementContexts)
			{
				var statement = Visit(statementContext);

				if (statement is EmptyStatement)
				{
					continue;
				}
				else if (statement is StatementCollection statementCol)
				{
					statements.AddRange(statementCol);
				}
				else
				{
					statements.Add((Statement)statement);
				}
			}
		}

		public override object VisitStatement_empty([NotNull] Statement_emptyContext context)
		{
			return new EmptyStatement();
		}

		public override object VisitStatement_variable_declaration([NotNull] Statement_variable_declarationContext context)
		{
			return VisitVariable_declaration(context.variable_declaration());
		}

		public override object VisitStatement_method_declaration([NotNull] Statement_method_declarationContext context)
		{
			var currentNamespace = _nameSpaceScopeStack.Peek();

			var function = (FunctionDeclaration)VisitMethod_declaration(context.method_declaration());

			currentNamespace.Functions.Add(function);

			return new EmptyStatement();
		}

		public override object VisitMethod_declaration([NotNull] Method_declarationContext context)
		{
			var function = (FunctionDeclaration)VisitMethod_header(context.method_header());

			//function.Statements

			return function;
		}

		public override object VisitMethod_header([NotNull] Method_headerContext context)
		{
			var function = new FunctionDeclaration(
				context.identifier().GetText(),
				(TypeReference)VisitReturn_type(context.return_type())
				);


			if (context.parameter_list() != null)
			{
				foreach (var parameterContext in context.parameter_list().parameter())
				{
					function.Parameters.Add((ParameterDeclarationExpression)VisitParameter(parameterContext));
				}
			}


			return function;
		}

		public override object VisitParameter_list([NotNull] Parameter_listContext context)
		{
			ParameterDeclarationExpressionCollection parameters = new();

			foreach (var parameterContext in context.parameter())
			{
				parameters.Add((ParameterDeclarationExpression)VisitParameter(parameterContext));
			}

			return parameters;
		}

		public override object VisitParameter([NotNull] ParameterContext context)
		{
			var parameter = new ParameterDeclarationExpression(
				context.identifier().GetText(),
				(TypeReference)VisitType(context.type())
				);

			return parameter;
		}

		public override object VisitReturn_type([NotNull] Return_typeContext context)
		{
			TypeReference typeReference;
			if (context.NOTHING() != null)
			{
				typeReference = new TypeReference("nothing");
			}
			else
			{
				typeReference = (TypeReference)VisitType(context.type());
			}
			return typeReference;
		}

		public override object VisitType([NotNull] TypeContext context)
		{
			// TODO:
			var typeContext = context.GetChild<RuleContext>(0);
			TypeReference typeReference = typeContext switch
			{
				Value_typeContext valueTypeContext => (TypeReference)VisitValue_type(valueTypeContext),
				Type_parameterContext typeParameterContext => new TypeReference(typeParameterContext.GetText()),
				_ => throw new Exception($"Unexpected type: rule {typeContext.RuleIndex}"),
			};

			return typeReference;
		}

		public override object VisitValue_type([NotNull] Value_typeContext context)
		{
			return VisitNon_nullable_value_type(context.non_nullable_value_type());
		}

		public override object VisitNon_nullable_value_type([NotNull] Non_nullable_value_typeContext context)
		{
			return VisitStruct_type(context.struct_type());
		}

		public override object VisitStruct_type([NotNull] Struct_typeContext context)
		{
			var typeContext = context.GetChild<RuleContext>(0);
			TypeReference typeReference = typeContext switch
			{
				Type_nameContext typeNameContext => new TypeReference(typeNameContext.GetText()),
				Simple_typeContext simpleTypeContext => (TypeReference)VisitSimple_type(simpleTypeContext),
				_ => throw new Exception($"Unexpected type: rule {typeContext.RuleIndex}"),
			};
			return typeReference;
		}

		public override object VisitSimple_type([NotNull] Simple_typeContext context)
		{
			if (context.BOOLEAN() != null)
				return BuiltIns.TypeReferences.Boolean;

			return VisitNumeric_type(context.numeric_type());
		}

		public override object VisitNumeric_type([NotNull] Numeric_typeContext context)
		{
			var typeContext = context.GetChild<RuleContext>(0);
			TypeReference typeReference = typeContext switch
			{
				Integer_typeContext integerTypeContext => (TypeReference)VisitInteger_type(integerTypeContext),
				Floating_point_typeContext floatingPointTypeContext => (TypeReference)VisitFloating_point_type(floatingPointTypeContext),
				_ => throw new Exception($"Unexpected type: rule {typeContext.RuleIndex}"),
			};
			return typeReference;
		}

		public override object VisitInteger_type([NotNull] Integer_typeContext context)
		{
			if (context.UNSIGNED() != null)
			{
				if (context.TINY() != null)
				{
					return BuiltIns.TypeReferences.UInt8;
				}
				else if (context.SMALL() != null)
				{
					return BuiltIns.TypeReferences.UInt16;
				}
				else if (context.BIG() != null)
				{
					return BuiltIns.TypeReferences.UInt64;
				}
				else
				{
					return BuiltIns.TypeReferences.UInt32;
				}
			}
			else
			{
				if (context.TINY() != null)
				{
					return BuiltIns.TypeReferences.Int8;
				}
				else if (context.SMALL() != null)
				{
					return BuiltIns.TypeReferences.Int16;
				}
				else if (context.BIG() != null)
				{
					return BuiltIns.TypeReferences.Int64;
				}
				else
				{
					return BuiltIns.TypeReferences.Int32;
				}
			}
		}

		public override object VisitFloating_point_type([NotNull] Floating_point_typeContext context)
		{
			if (context.BIG() != null)
				return BuiltIns.TypeReferences.Float64;
			else
				return BuiltIns.TypeReferences.Float32;
		}

		public override object VisitEmpty_statement([NotNull] Empty_statementContext context)
		{
			return new EmptyStatement();
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

			throw new Exception($"Unexpected variable declaration: rule {context.RuleIndex}");
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
				TypeReference: (TypeReference)VisitType(context.type())
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

using Noveler.Compiler.Grammar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Misc;
using Noveler.Compiler.CodeDomainObjectModel;
using Antlr4.Runtime;
using static Noveler.Compiler.Grammar.NovelerParser;
using System.Runtime.InteropServices;
using Noveler.Compiler.CodeDomainObjectModel.Expressions;
using Noveler.Compiler.CodeDomainObjectModel.Statements;
using Antlr4.Runtime.Tree;

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
						//TODO: make sure to add the text string to the string table
						break;

					case Embed_statementContext embedStatementContext:
						HandleEmbeddedStatement(embedStatementContext, statements);
						break;

					// empty segments can be skipped
					case Empty_segmentContext:
						continue;

					default:
						throw new Exception();
				}
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

				case Choice_blockContext choiceBlockContext:
					statements.Add((Statement)VisitChoice_block(choiceBlockContext));
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

		public override object VisitReturn_statement([NotNull] Return_statementContext context)
		{
			return new ReturnStatement(
					context.expression() != null ? (Expression)VisitExpression(context.expression()) : null
				);
		}

		public override object VisitExpression([NotNull] ExpressionContext context)
		{
			return Visit(context.GetChild(0));
		}

		public override object VisitNon_assignment_expression([NotNull] Non_assignment_expressionContext context)
		{
			return Visit(context.GetChild(0));
		}

		public override object VisitNull_coalescing_expression([NotNull] Null_coalescing_expressionContext context)
		{
			return Visit(context.GetChild(0));
		}

		public override object VisitConditional_expression([NotNull] Conditional_expressionContext context)
		{
			if (context.QUESTION_MARK() != null)
			{
				var conditionExpression = (Expression)VisitNull_coalescing_expression(context.null_coalescing_expression());
				var trueExpression = (Expression)VisitExpression(context.expression(0));
				var falseExpression = (Expression)VisitExpression(context.expression(1));

				return new TernaryExpression(conditionExpression, trueExpression, falseExpression);
			}
			else
			{
				return (Expression)VisitNull_coalescing_expression(context.null_coalescing_expression());
			}
		}

		public override object VisitConditional_or_expression([NotNull] Conditional_or_expressionContext context)
		{
			if (context.CONDITIONAL_OR() != null)
			{
				return new ConditionalOrExpression(
					(Expression)VisitConditional_or_expression(context.conditional_or_expression()),
					(Expression)VisitConditional_and_expression(context.conditional_and_expression())
					);
			}
			else
			{
				return VisitConditional_and_expression(context.conditional_and_expression());
			}
		}

		public override object VisitConditional_and_expression([NotNull] Conditional_and_expressionContext context)
		{
			if (context.CONDITIONAL_AND() != null)
			{
				return new ConditionalAndExpression(
					(Expression)VisitConditional_and_expression(context.conditional_and_expression()),
					(Expression)VisitInclusive_or_expression(context.inclusive_or_expression())
					);
			}
			else
			{
				return VisitInclusive_or_expression(context.inclusive_or_expression());
			}
		}

		public override object VisitInclusive_or_expression([NotNull] Inclusive_or_expressionContext context)
		{
			if (context.PIPE() != null)
			{
				return new InclusiveOrExpression(
					(Expression)VisitInclusive_or_expression(context.inclusive_or_expression()),
					(Expression)VisitExclusive_or_expression(context.exclusive_or_expression())
					);
			}
			else
			{
				return VisitExclusive_or_expression(context.exclusive_or_expression());
			}
		}

		public override object VisitExclusive_or_expression([NotNull] Exclusive_or_expressionContext context)
		{
			if (context.BITWISE_XOR() != null)
			{
				return new ExclusiveOrExpression(
					(Expression)VisitExclusive_or_expression(context.exclusive_or_expression()),
					(Expression)VisitAnd_expression(context.and_expression())
					);
			}
			else
			{
				return VisitAnd_expression(context.and_expression());
			}
		}

		public override object VisitAnd_expression([NotNull] And_expressionContext context)
		{
			if (context.BITWISE_AND() != null)
			{
				return new AndExpression(
					(Expression)VisitAnd_expression(context.and_expression()),
					(Expression)VisitEquality_expression(context.equality_expression())
					);
			}
			else
			{
				return VisitEquality_expression(context.equality_expression());
			}
		}

		public override object VisitEquality_expression([NotNull] Equality_expressionContext context)
		{
			if (context.Equal_To() != null)
			{
				return new EqualToExpression(
					(Expression)VisitEquality_expression(context.equality_expression()),
					(Expression)VisitRelational_expression(context.relational_expression())
					);
			}
			else if (context.Not_Equal_To() != null)
			{
				return new NotEqualToExpression(
					(Expression)VisitEquality_expression(context.equality_expression()),
					(Expression)VisitRelational_expression(context.relational_expression())
					);
			}
			else
			{
				return VisitRelational_expression(context.relational_expression());
			}
		}

		public override object VisitRelational_expression([NotNull] Relational_expressionContext context)
		{
			if (context.LESS_THAN() != null)
			{
				return new LessThanExpression(
						(Expression)VisitRelational_expression(context.relational_expression()),
						(Expression)VisitShift_expression(context.shift_expression())
					);
			}
			else if (context.GREATER_THAN() != null)
			{
				return new GreaterThanExpression(
						(Expression)VisitRelational_expression(context.relational_expression()),
						(Expression)VisitShift_expression(context.shift_expression())
					);
			}
			else if (context.Less_Than_Or_Equal_To() != null)
			{
				return new LessThanOrEqualExpression(
						(Expression)VisitRelational_expression(context.relational_expression()),
						(Expression)VisitShift_expression(context.shift_expression())
					);
			}
			else if (context.Greater_Than_Or_Equal_To() != null)
			{
				return new GreaterThanOrEqualExpression(
						(Expression)VisitRelational_expression(context.relational_expression()),
						(Expression)VisitShift_expression(context.shift_expression())
					);
			}
			else
			{
				return VisitShift_expression(context.shift_expression());
			}
		}

		public override object VisitShift_expression([NotNull] Shift_expressionContext context)
		{
			if (context.LEFT_SHIFT() != null)
			{
				return new LeftShiftExpression(
					(Expression)VisitShift_expression(context.shift_expression()),
					(Expression)VisitAdditive_expression(context.additive_expression())
					);
			}
			else if (context.RIGHT_SHIFT() != null)
			{
				return new RightShiftExpression(
					(Expression)VisitShift_expression(context.shift_expression()),
					(Expression)VisitAdditive_expression(context.additive_expression())
					);
			}
			else
			{
				return VisitAdditive_expression(context.additive_expression());
			}
		}

		public override object VisitAdditive_expression([NotNull] Additive_expressionContext context)
		{
			if (context.PLUS() != null)
			{
				return new AdditionExpression(
					(Expression)VisitAdditive_expression(context.additive_expression()),
					(Expression)VisitMultiplicative_expression(context.multiplicative_expression())
					);
			}
			else if (context.MINUS() != null)
			{
				return new SubtractionExpression(
					(Expression)VisitAdditive_expression(context.additive_expression()),
					(Expression)VisitMultiplicative_expression(context.multiplicative_expression())
					);
			}
			else
			{
				return VisitMultiplicative_expression(context.multiplicative_expression());
			}
		}

		public override object VisitMultiplicative_expression([NotNull] Multiplicative_expressionContext context)
		{
			if (context.ASTERISK() != null)
			{
				return new AdditionExpression(
					(Expression)VisitMultiplicative_expression(context.multiplicative_expression()),
					(Expression)VisitUnary_expression(context.unary_expression())
					);
			}
			else if (context.SLASH() != null)
			{
				return new SubtractionExpression(
					(Expression)VisitMultiplicative_expression(context.multiplicative_expression()),
					(Expression)VisitUnary_expression(context.unary_expression())
					);
			}
			else if (context.REMAINDER() != null)
			{
				return new SubtractionExpression(
					(Expression)VisitMultiplicative_expression(context.multiplicative_expression()),
					(Expression)VisitUnary_expression(context.unary_expression())
					);
			}
			else
			{
				return VisitUnary_expression(context.unary_expression());
			}
		}

		public override object VisitAssignment([NotNull] AssignmentContext context)
		{
			return new AssignmentExpression(
				(Expression)VisitUnary_expression(context.unary_expression()),
				(Expression)VisitExpression(context.expression())
				);
		}

		public override object VisitUnary_expression([NotNull] Unary_expressionContext context)
		{
			if (context.unary_expression() != null)
				return VisitUnary_expression(context.unary_expression());

			if (context.primary_expression() != null)
				return VisitPrimary_expression(context.primary_expression());

			// +a unary expression has no effect on the result of a
			if (context.PLUS() != null)
				return VisitUnary_expression(context.unary_expression());

			if (context.MINUS() != null)
				return new NegateExpression(
						(Expression)VisitUnary_expression(context.unary_expression())
					);

			if (context.CONDITIONAL_NOT() != null)
				return new ConditionalNotExpression(
						(Expression)VisitUnary_expression(context.unary_expression())
					);

			if (context.BITWISE_NOT() != null)
				return new BitwiseNotExpression(
						(Expression)VisitUnary_expression(context.unary_expression())
					);

			if (context.pre_increment_expression() != null)
				return VisitPre_increment_expression(context.pre_increment_expression());

			if (context.pre_decrement_expression() != null)
				return VisitPre_decrement_expression(context.pre_decrement_expression());

			throw new Exception("unhandled unary expression.");
		}

		public override object VisitCast_expression([NotNull] Cast_expressionContext context)
		{
			return new CastExpression(
					(TypeReference)VisitType(context.type()),
					(Expression)VisitUnary_expression(context.unary_expression())
				);
		}

		public override object VisitPre_increment_expression([NotNull] Pre_increment_expressionContext context)
		{
			return new PreIncrementExpression(
					(Expression)VisitUnary_expression(context.unary_expression())
				);
		}

		public override object VisitPre_decrement_expression([NotNull] Pre_decrement_expressionContext context)
		{
			return new PreDecrementExpression(
					(Expression)VisitUnary_expression(context.unary_expression())
				);
		}

		public override object VisitPrimary_expression([NotNull] Primary_expressionContext context)
		{
			if (context.literal() != null)
				return VisitLiteral(context.literal());

			var startExpression = (Expression)VisitPrimary_expression_start(context.primary_expression_start());

			if (context.ChildCount == 1)
				return startExpression;

			var currentExpression = startExpression;
			var children = context.children;
			for (int i = 1; i < children.Count; i++)
			{
				var currentContext = children[i];

				currentExpression = currentContext.Payload switch
				{
					IToken { Text: "++" } => new PostIncrementExpression(currentExpression),
					IToken { Text: "--" } => new PostDecrementExpression(currentExpression),
					Bracket_expressionContext bracketExpressionContext => new IndexerExpression(currentExpression, (ArgumentCollection)VisitArgument_list(bracketExpressionContext.argument_list())),
					Member_accessContext memberAccessContext => new AccessExpression(currentExpression, (Symbol)VisitIdentifier(memberAccessContext.identifier())),
					Method_invocationContext methodInvocationContext => new InvocationExpression(currentExpression, (ArgumentCollection)VisitArgument_list(methodInvocationContext.argument_list())),
					_ => throw new Exception("Unhandled part of primary expression.")
				};
			}

			return currentExpression;
		}

		public override object VisitMethod_invocation([NotNull] Method_invocationContext context)
		{
			return base.VisitMethod_invocation(context);
		}

		public override object VisitMember_access([NotNull] Member_accessContext context)
		{
			return base.VisitMember_access(context);
		}

		public override object VisitPrimary_expression_start([NotNull] Primary_expression_startContext context)
		{
			if (context.simple_name() != null)
				return VisitSimple_name(context.simple_name());

			if (context.parenthesized_expression() != null)
				return VisitParenthesized_expression(context.parenthesized_expression());

			if (context.this_access() != null)
				return VisitThis_access(context.this_access());

			if (context.object_creation_expression() != null)
				return VisitObject_creation_expression(context.object_creation_expression());

			throw new Exception("Unhandled primary expression start.");
		}

		public override object VisitSimple_name([NotNull] Simple_nameContext context)
		{
			return VisitIdentifier(context.identifier());
		}

		public override object VisitObject_creation_expression([NotNull] Object_creation_expressionContext context)
		{
			return new ObjectCreationExpression(
					(TypeReference)VisitType(context.type()),
					(ArgumentCollection)VisitArgument_list(context.argument_list())
				);
		}

		public override object VisitArgument_list(Argument_listContext? context)
		{
			var arguments = new ArgumentCollection();

			if (context == null)
				return arguments;

			foreach (var argumentContext in context.argument())
			{
				arguments.Add(
						(Argument)VisitArgument(argumentContext)
					);
			}

			return arguments;
		}

		public override object VisitArgument([NotNull] ArgumentContext context)
		{
			return new Argument(
					(Expression)VisitArgument_value(context.argument_value())
				);
		}

		public override object VisitArgument_value([NotNull] Argument_valueContext context)
		{
			return VisitExpression(context.expression());
		}

		public override object VisitParenthesized_expression([NotNull] Parenthesized_expressionContext context)
		{
			return VisitExpression(context.expression());
		}

		public override object VisitThis_access([NotNull] This_accessContext context)
		{
			return new ThisAccessReferenceExpression();
		}

		public override object VisitBoolean_type([NotNull] Boolean_typeContext context)
		{
			return BuiltIns.TypeReferences.Boolean;
		}

		public override object VisitLiteral([NotNull] LiteralContext context)
		{
			if (context.booleanLiteral() != null)
				return VisitBooleanLiteral(context.booleanLiteral());

			if (context.Decimal_Integer_Literal() != null)
				return IntegerLiteralExpression.FromDecimal(context.GetText());

			if (context.Hexadecimal_Integer_Literal() != null)
				return IntegerLiteralExpression.FromHex(context.GetText());

			if (context.Binary_Integer_Literal() != null)
				return IntegerLiteralExpression.FromBinary(context.GetText());

			if (context.Real_Literal() != null)
				return new FloatingPointLiteralExpression(context.GetText());

			throw new Exception("Unhandled literal.");
		}

		public override object VisitBooleanLiteral([NotNull] BooleanLiteralContext context)
		{
			if (context.TRUE() != null)
				return new BooleanLiteralExpression(true);

			if (context.FALSE() != null)
				return new BooleanLiteralExpression(false);

			throw new Exception("Unhandled boolean literal");
		}

		public override object VisitChoice_block([NotNull] Choice_blockContext context)
		{
			var choiceBlockStatement = new EmbeddedChoiceBlockStatement();

			foreach (var choiceContext in context.choice_block_choice())
			{
				var choice = VisitChoice_block_choice(choiceContext);

				if (choice is EmbeddedChoiceOption option)
				{
					choiceBlockStatement.ChoiceOptions.Add(option);
				}
				else if (choice is EmbeddedDefaultChoiceOption defaultOption)
				{
					if (choiceBlockStatement.DefaultOption != null)
					{
						//TODO: properly handle this.
						throw new Exception("A choice block can not have more than 1 default choice.");
					}

					choiceBlockStatement.DefaultOption = defaultOption;
				}
				else
				{
					throw new Exception($"Unhandled choice option: rule {choiceContext.RuleIndex}");
				}
			}

			return choiceBlockStatement;
		}

		public override object VisitChoice_block_choice([NotNull] Choice_block_choiceContext context)
		{
			var choiceContext = context.GetChild<RuleContext>(0);

			switch (choiceContext.Payload)
			{
				case Standard_choiceContext:
				case Default_choiceContext:
					return Visit(choiceContext.Payload);

				default:
					throw new Exception($"Unhandled choice block choice: rule {choiceContext.RuleIndex}");
			}
		}

		public override object VisitStandard_choice([NotNull] Standard_choiceContext context)
		{
			var expressions = new ExpressionCollection();
			StringBuilder sb = new();

			HandleTextSegments(context.text_line_segment(), sb, expressions);

			var choiceText = new TextStatement(sb.ToString(), expressions);
			var choiceOption = new EmbeddedChoiceOption(choiceText);

			HandleStorySegments(context.choice_response().story_segment(), choiceOption.OptionStatements);

			return choiceOption;
		}

		public override object VisitDefault_choice([NotNull] Default_choiceContext context)
		{
			var expressions = new ExpressionCollection();
			StringBuilder sb = new();

			HandleTextSegments(context.text_line_segment(), sb, expressions);

			var choiceText = new TextStatement(sb.ToString(), expressions);
			var choiceOption = new EmbeddedDefaultChoiceOption(choiceText);

			HandleStorySegments(context.choice_response().story_segment(), choiceOption.OptionStatements);

			return choiceOption;
		}

		public override object VisitChoice_response([NotNull] Choice_responseContext context)
		{
			var statements = new StatementCollection();

			HandleStorySegments(context.story_segment(), statements);

			return statements;
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
			function.Statements.AddRange((StatementCollection)VisitMethod_body(context.method_body()));

			return function;
		}

		public override object VisitMethod_body([NotNull] Method_bodyContext context)
		{
			StatementCollection statements = new();

			HandleCodeBlock(context.code_block(), statements);

			return statements;
		}

		public override object VisitStatement_expression([NotNull] Statement_expressionContext context)
		{
			StatementCollection statements = new();

			var expressionContexts = context.expression();
			foreach (var expressionContext in expressionContexts)
			{
				var expression = (Expression)VisitExpression(expressionContext);
				var expressionStatement = new ExpressionStatement(expression);

				statements.Add(expressionStatement);
			}

			return statements;
		}

		public override object VisitStatement_return([NotNull] Statement_returnContext context)
		{
			return VisitReturn_statement(context.return_statement());
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

		public override object VisitIdentifier([NotNull] IdentifierContext context)
		{
			return new Symbol(context.GetText());
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
					char escapedCharacter = segment.escaped_text_segment_character().GetText()[1];

					builder.Append(escapedCharacter, escapedCharacter == '{' || escapedCharacter == '}' ? 2 : 1);
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

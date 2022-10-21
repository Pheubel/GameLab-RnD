using Noveler.Compiler;

namespace NovelerCompiler
{
    internal static partial class SyntaxAnalyzer
    {
        #region Grammars

        /// <summary>
        /// : '\n'<br/>
        /// | '\r\n'
        /// </summary>
        static readonly Grammar NewLineGrammar;

        /// <summary>
        /// : ~<see cref="NewLineGrammar"/>
        /// </summary>
        static readonly Grammar CharacterGrammar;

        /// <summary>
        /// : 'true'<br/>
        /// | 'false'
        /// </summary>
        static readonly Grammar BooleanLiteralGrammar;

        /// <summary>
        /// : <see cref="NewLineGrammar"/>
        /// | ';'
        /// </summary>
        static readonly Grammar StatementEliminatorGrammar;

        /// <summary>
        /// : <see cref="PrimaryExpressionGrammar"/><br/>
        /// | '+' <see cref="UnaryExpressionGrammar"/> <br/>
        /// | '-' <see cref="UnaryExpressionGrammar"/><br/>
        /// | <see cref="PreIncrementExpressionGrammar"/><br/>
        /// | <see cref="PreDecrementExpressionGrammar"/>
        /// </summary>
        static readonly Grammar UnaryExpressionGrammar;

        /// <summary>
        /// : <see cref="PrimaryNoArrayCreationExpression"/>
        /// </summary>
        static readonly Grammar PrimaryExpressionGrammar;

        /// <summary>
        /// : '(' <see cref="TypeGrammar"/> ')' <see cref="UnaryExpressionGrammar"/>
        /// </summary>
        static readonly Grammar CastExpressionGrammar;

        /// <summary>
        /// : '++' <see cref="UnaryExpressionGrammar"/>
        /// </summary>
        static readonly Grammar PreIncrementExpressionGrammar;

        /// <summary>
        /// : '--' <see cref="UnaryExpressionGrammar"/>
        /// </summary>
        static readonly Grammar PreDecrementExpressionGrammar;

        /// <summary>
        /// : <see cref="AssignmentGrammar"/>
        /// </summary>
        static readonly Grammar ExpressionGrammar;

        /// <summary>
        /// : <see cref="ExpressionGrammar"/>
        /// </summary>
        static readonly Grammar BooleanExpressionGrammar;

        /// <summary>
        /// : <see cref="ValueTypeGrammar"/>
        /// </summary>
        static readonly Grammar TypeGrammar;

        /// <summary>
        /// : <see cref="TypeGrammar"/><br/>
        /// | 'nothing'
        /// </summary>
        static readonly Grammar ReturnTypeGrammar;

        /// <summary>
        /// : <see cref="UnaryExpressionGrammar"/><br/>
        /// | <see cref="MultiplicativeExpressionGrammar"/> '*' <see cref="UnaryExpressionGrammar"/><br/>
        /// | <see cref="MultiplicativeExpressionGrammar"/> '/' <see cref="UnaryExpressionGrammar"/><br/>
        /// | <see cref="MultiplicativeExpressionGrammar"/> '%' <see cref="UnaryExpressionGrammar"/>
        /// </summary>
        static readonly Grammar MultiplicativeExpressionGrammar;

        /// <summary>
        /// : <see cref="UnaryExpressionGrammar"/><br/>
        /// | <see cref="UnaryExpressionGrammar"/> '+' <see cref="UnaryExpressionGrammar"/><br/>
        /// | <see cref="UnaryExpressionGrammar"/> '-' <see cref="UnaryExpressionGrammar"/>
        /// </summary>
        static readonly Grammar AdditiveExpressionGrammar;

        /// <summary>
        /// : <see cref="AdditiveExpressionGrammar"/><br/>
        /// | <see cref="ShiftExpressionGrammar"/> '<![CDATA[<]]><![CDATA[<]]>' <see cref="AdditiveExpressionGrammar"/><br/>
        /// | <see cref="ShiftExpressionGrammar"/> '>>' <see cref="AdditiveExpressionGrammar"/>
        /// </summary>
        static readonly Grammar ShiftExpressionGrammar;

        /// <summary>
        /// : <see cref="ShiftExpressionGrammar"/><br/>
        /// | <see cref="RelationalExpressionGrammmar"/> '<![CDATA[<]]>' <see cref="RelationalExpressionGrammmar"/><br/>
        /// | <see cref="RelationalExpressionGrammmar"/> '>' <see cref="RelationalExpressionGrammmar"/><br/>
        /// | <see cref="RelationalExpressionGrammmar"/> '<![CDATA[<]]>=' <see cref="RelationalExpressionGrammmar"/><br/>
        /// | <see cref="RelationalExpressionGrammmar"/> '>=' <see cref="RelationalExpressionGrammmar"/>
        /// </summary>
        static readonly Grammar RelationalExpressionGrammmar;

        /// <summary>
        /// : <see cref="RelationalExpressionGrammmar"/><br/>
        /// | <see cref="EqualityExpressionGrammar"/> '==' <see cref="RelationalExpressionGrammmar"/><br/>
        /// | <see cref="EqualityExpressionGrammar"/> '!=' <see cref="RelationalExpressionGrammmar"/>
        /// </summary>
        static readonly Grammar EqualityExpressionGrammar;

        /// <summary>
        /// : <see cref="EqualityExpressionGrammar"/><br/>
        /// | <see cref="AndExpressionGrammar"/> '<![CDATA[&]]>' <see cref="EqualityExpressionGrammar"/>
        /// </summary>
        static readonly Grammar AndExpressionGrammar;

        /// <summary>
        /// : <see cref="AndExpressionGrammar"/>
        /// | <see cref="ExclusiveOrExpressionGrammar"/> '^' <see cref="AndExpressionGrammar"/>
        /// </summary>
        static readonly Grammar ExclusiveOrExpressionGrammar;

        /// <summary>
        /// : <see cref="ExclusiveOrExpressionGrammar"/><br/>
        /// | <see cref="InclusiveOrExpressionGramar"/> '|' <see cref="ExclusiveOrExpressionGrammar"/>
        /// </summary>
        static readonly Grammar InclusiveOrExpressionGramar;

        /// <summary>
        /// : <see cref="InclusiveOrExpressionGramar"/><br/>
        /// | <see cref="ConditionalAndExpressionGrammar"/> '<![CDATA[&]]><![CDATA[&]]>' <see cref="InclusiveOrExpressionGramar"/>
        /// </summary>
        static readonly Grammar ConditionalAndExpressionGrammar;

        /// <summary>
        /// : <see cref="ConditionalAndExpressionGrammar"/><br/>
        /// | <see cref="ConditionalOrExpressionGrammar"/> '||' <see cref="ConditionalAndExpressionGrammar"/>
        /// </summary>
        static readonly Grammar ConditionalOrExpressionGrammar;

        /// <summary>
        /// : '='<br/>
        /// | '+='<br/>
        /// | '-='<br/>
        /// | '*='<br/>
        /// | '/='<br/>
        /// | '%='<br/>
        /// | '<![CDATA[&]]>='<br/>
        /// | '|='<br/>
        /// | '^='<br/>
        /// | '<![CDATA[<]]><![CDATA[<]]>='<br/>
        /// | '>>='
        /// </summary>
        static readonly Grammar AssignmentOperatorGrammar;

        /// <summary>
        /// : <see cref="UnaryExpressionGrammar"/> <see cref="AssignmentOperatorGrammar"/> <see cref="ExpressionGrammar"/>
        /// </summary>
        static readonly Grammar AssignmentGrammar;

        /// <summary>
        /// : <see cref="ExpressionGrammar"/>
        /// </summary>
        static readonly Grammar ConstantExpressionGrammar;

        /// <summary>
        /// : 'if' '(' <see cref="BooleanExpressionGrammar"/> ')' <see cref="EmbeddedStatementGrammar"/><br/>
        /// | 'if' '(' <see cref="BooleanExpressionGrammar"/> ')' <see cref="EmbeddedStatementGrammar"/><br/> 'else' <see cref="EmbeddedStatementGrammar"/>
        /// </summary>
        static readonly Grammar IfStatementGrammar;

        /// <summary>
        /// : <see cref="BlockGrammar"/><br/>
        /// | <see cref="ExpressionStatementGrammar"/><br/>
        /// | <see cref="SelectionStatementGrammar"/><br/>
        /// | <see cref="JumpStatementGrammar"/><br/>
        /// </summary>
        static readonly Grammar EmbeddedStatementGrammar;

        /// <summary>
        /// : <see cref="StatementExpressionGrammar"/> <see cref="StatementEliminatorGrammar"/>
        /// </summary>
        static readonly Grammar ExpressionStatementGrammar;

        /// <summary>
        /// : <see cref="InvocationExpressionGrammar"/><br/>
        /// | <see cref="AssignmentGrammar"/><br/>
        /// | <see cref="PostIncrementExpressionGrammar"/><br/>
        /// | <see cref="PostDecrementExpressionGrammar"/><br/>
        /// | <see cref="PreIncrementExpressionGrammar"/><br/>
        /// | <see cref="PreDecrementExpressionGrammar"/>
        /// </summary>
        static readonly Grammar StatementExpressionGrammar;

        /// <summary>
        /// : <see cref="PrimaryExpressionGrammar"/> '(' <see cref="ArgumentListGrammar"/>? ')'
        /// </summary>
        static readonly Grammar InvocationExpressionGrammar;

        /// <summary>
        /// : <see cref="ArgumentGrammar"/> (',' ArgumentGrammar)*
        /// </summary>
        static readonly Grammar ArgumentListGrammar;

        /// <summary>
        /// : <see cref="ArgumentNameGrammar"/>? <see cref="ArgumentValueGrammar"/>
        /// </summary>
        static readonly Grammar ArgumentGrammar;

        /// <summary>
        /// : <see cref="IdentifierGrammar"/> ':'
        /// </summary>
        static readonly Grammar ArgumentNameGrammar;

        /// <summary>
        /// : <see cref="ExpressionGrammar"/>
        /// </summary>
        static readonly Grammar ArgumentValueGrammar;

        /// <summary>
        /// : ([A-Z] | [a-z]) ([A-Z] | [a-z] | [0-9])*
        /// </summary>
        static readonly Grammar IdentifierGrammar;

        /// <summary>
        /// : '{' <see cref="StatementListGrammar"/>? '}'
        /// </summary>
        static readonly Grammar BlockGrammar;

        /// <summary>
        /// : <see cref="StatementGrammar"/>+
        /// </summary>
        static readonly Grammar StatementListGrammar;

        /// <summary>
        /// : <see cref="DeclarationStatementGrammar"/><br/>
        /// | <see cref="EmbeddedStatementGrammar"/>
        /// </summary>
        static readonly Grammar StatementGrammar;

        /// <summary>
        /// : <see cref="ReturnStatementGrammar"/>
        /// </summary>
        static readonly Grammar JumpStatementGrammar;

        /// <summary>
        /// : 'return' <see cref="ExpressionGrammar"/>? <see cref="StatementEliminatorGrammar"/>
        /// </summary>
        static readonly Grammar ReturnStatementGrammar;

        /// <summary>
        /// : <see cref="LocalVariableDeclaratorGrammar"/> <see cref="StatementEliminatorGrammar"/>
        /// </summary>
        static readonly Grammar DeclarationStatementGrammar;

        /// <summary>
        /// : <see cref="IdentifierGrammar"/> ':' <see cref="TypeGrammar"/><br/>
        /// | <see cref="IdentifierGrammar"/> ':' <see cref="TypeGrammar"/> '=' <see cref="LocalVariableInitializerGrammar"/>
        /// </summary>
        static readonly Grammar LocalVariableDeclaratorGrammar;

        /// <summary>
        /// : <see cref="ExpressionGrammar"/>
        /// </summary>
        static readonly Grammar LocalVariableInitializerGrammar;

        /// <summary>
        /// : <see cref="IfStatementGrammar"/>
        /// </summary>
        static readonly Grammar SelectionStatementGrammar;

        /// <summary>
        /// : <see cref="IntegerTypeGrammar"/><br/>
        /// | <see cref="FloatingPointTypeGrammar"/>
        /// </summary>
        static readonly Grammar NumericTypeGrammar;

        /// <summary>
        /// : <see cref="SignedGrammar"/> <see cref="TypeSizeGrammar"/> 'whole' 'number'
        /// </summary>
        static readonly Grammar IntegerTypeGrammar;

        /// <summary>
        /// : <see cref="TypeSizeGrammar"/> 'number'
        /// </summary>
        static readonly Grammar FloatingPointTypeGrammar;

        /// <summary>
        /// : 'unsigned'
        /// | 'signed'
        /// </summary>
        static readonly Grammar SignedGrammar;

        /// <summary>
        /// : 'tiny'<br/>
        /// | 'small'<br/>
        /// | 'big'
        /// </summary>
        static readonly Grammar TypeSizeGrammar;

        /// <summary>
        /// : <see cref="TypeNameGrammar"/><br/>
        /// | <see cref="SimpleValueTypeGrammar"/>
        /// </summary>
        static readonly Grammar ValueTypeGrammar;

        /// <summary>
        /// : <see cref="NumericTypeGrammar"/><br/>
        /// | 'boolean'
        /// </summary>
        static readonly Grammar SimpleValueTypeGrammar;

        /// <summary>
        /// : <see cref="NamespaceOrTypeNameGrammar"/>
        /// </summary>
        static readonly Grammar TypeNameGrammar;

        /// <summary>
        /// : <see cref="NamespaceOrTypeNameGrammar"/>
        /// </summary>
        static readonly Grammar NamespaceNameGrammar;

        /// <summary>
        /// : ( <see cref="IdentifierGrammar"/> '.' )* <see cref="IdentifierGrammar"/>
        /// </summary>
        static readonly Grammar NamespaceOrTypeNameGrammar;

        /// <summary>
        /// : <see cref="PrimaryExpressionGrammar"/> '++'
        /// </summary>
        static readonly Grammar PostIncrementExpressionGrammar;

        /// <summary>
        /// : <see cref="PrimaryExpressionGrammar"/> '--'
        /// </summary>
        static readonly Grammar PostDecrementExpressionGrammar;

        /// <summary>
        /// : <see cref="BooleanLiteralGrammar"/><br/>
        /// | <see cref="IntegerLiteralGrammar"/><br/>
        /// | <see cref="FloatLiteralGrammar"/>
        /// </summary>
        static readonly Grammar LiteralGrammar;

        static readonly Grammar IntegerLiteralGrammar;

        static readonly Grammar FloatLiteralGrammar;

        /// <summary>
        /// : <see cref="LiteralGrammar"/><br/>
        /// | <see cref="ParenthesizedExpressionGrammar"/><br/>
        /// | <see cref="InvocationExpressionGrammar"/><br/>
        /// | <see cref="PostIncrementExpressionGrammar"/><br/>
        /// | <see cref="PostDecrementExpressionGrammar"/>
        /// </summary>
        static readonly Grammar PrimaryNoArrayCreationExpression;

        /// <summary>
        /// : '(' <see cref="ExpressionGrammar"/> ')'
        /// </summary>
        static readonly Grammar ParenthesizedExpressionGrammar;

        #endregion // Grammars


        static SyntaxAnalyzer()
        {
            #region Instantiation

            NewLineGrammar = new Grammar();
            CharacterGrammar = new Grammar();
            BooleanLiteralGrammar = new Grammar();
            StatementEliminatorGrammar = new Grammar();
            UnaryExpressionGrammar = new Grammar();
            PreIncrementExpressionGrammar = new Grammar();
            PreDecrementExpressionGrammar = new Grammar();
            CastExpressionGrammar = new Grammar();
            ReturnTypeGrammar = new Grammar();
            MultiplicativeExpressionGrammar = new Grammar();
            AdditiveExpressionGrammar = new Grammar();
            AssignmentOperatorGrammar = new Grammar();
            BooleanExpressionGrammar = new Grammar();
            AssignmentGrammar = new Grammar();
            ExpressionGrammar = new Grammar();
            ConstantExpressionGrammar = new Grammar();
            IfStatementGrammar = new Grammar();
            ExpressionStatementGrammar = new Grammar();
            InvocationExpressionGrammar = new Grammar();
            ArgumentGrammar = new Grammar();
            ArgumentNameGrammar = new Grammar();
            IdentifierGrammar = new Grammar();
            ArgumentValueGrammar = new Grammar();
            ShiftExpressionGrammar = new Grammar();
            RelationalExpressionGrammmar = new Grammar();
            EqualityExpressionGrammar = new Grammar();
            AndExpressionGrammar = new Grammar();
            ExclusiveOrExpressionGrammar = new Grammar();
            InclusiveOrExpressionGramar = new Grammar();
            ConditionalAndExpressionGrammar = new Grammar();
            ConditionalOrExpressionGrammar = new Grammar();
            BlockGrammar = new Grammar();
            StatementListGrammar = new Grammar();
            EmbeddedStatementGrammar = new Grammar();
            JumpStatementGrammar = new Grammar();
            ReturnStatementGrammar = new Grammar();
            StatementGrammar = new Grammar();
            SelectionStatementGrammar = new Grammar();
            LocalVariableInitializerGrammar = new Grammar();
            ArgumentListGrammar = new Grammar();
            DeclarationStatementGrammar = new Grammar();
            LocalVariableDeclaratorGrammar = new Grammar();
            IntegerTypeGrammar = new Grammar();
            FloatingPointTypeGrammar = new Grammar();
            TypeSizeGrammar = new Grammar();
            SignedGrammar = new Grammar();
            NumericTypeGrammar = new Grammar();
            SimpleValueTypeGrammar = new Grammar();
            NamespaceNameGrammar = new Grammar();
            NamespaceOrTypeNameGrammar = new Grammar();
            TypeNameGrammar = new Grammar();
            ValueTypeGrammar = new Grammar();
            TypeGrammar = new Grammar();
            PostIncrementExpressionGrammar = new Grammar();
            PostDecrementExpressionGrammar = new Grammar();
            IntegerLiteralGrammar = new Grammar();
            FloatLiteralGrammar = new Grammar();
            LiteralGrammar = new Grammar();
            ParenthesizedExpressionGrammar = new Grammar();
            PrimaryExpressionGrammar = new Grammar();
            StatementExpressionGrammar = new Grammar();
            PrimaryNoArrayCreationExpression = new Grammar();

            #endregion // Instantiation



            #region Initialization

            NewLineGrammar.SetGrammar(
                IPattern.Tokens(TokenType.NewLine)
                );

            CharacterGrammar.SetGrammar(
                IPattern.None(NewLineGrammar)
                );

            BooleanLiteralGrammar.SetGrammar(
                IPattern.Any(TokenType.KeywordTrue,
                             TokenType.KeywordFalse)
                );

            StatementEliminatorGrammar.SetGrammar(
                IPattern.Any(IPattern.Tokens(TokenType.SemiColon), NewLineGrammar)
                );

            UnaryExpressionGrammar.SetGrammar(
                IPattern.Any(PrimaryExpressionGrammar,
                             IPattern.Exact(IPattern.Tokens(TokenType.Add), UnaryExpressionGrammar),
                             IPattern.Exact(IPattern.Tokens(TokenType.Subtract), UnaryExpressionGrammar),
                             PreIncrementExpressionGrammar,
                             PreDecrementExpressionGrammar,
                             CastExpressionGrammar)
                );

            PreIncrementExpressionGrammar.SetGrammar(
                IPattern.Tokens(TokenType.Increment), UnaryExpressionGrammar
                );

            PreDecrementExpressionGrammar.SetGrammar(
                IPattern.Tokens(TokenType.Decrement), UnaryExpressionGrammar
                );

            CastExpressionGrammar.SetGrammar(
                IPattern.Tokens(TokenType.LeftParenthesis), TypeGrammar, IPattern.Tokens(TokenType.RightParenthesis), UnaryExpressionGrammar
                );

            ExpressionGrammar.SetGrammar(
                AssignmentGrammar
                );

            BooleanExpressionGrammar.SetGrammar(
                ExpressionGrammar
                );

            ReturnTypeGrammar.SetGrammar(
                IPattern.Any(TypeGrammar,
                             IPattern.Tokens(TokenType.KeywordNothing))
                );

            MultiplicativeExpressionGrammar.SetGrammar(
                IPattern.Any(UnaryExpressionGrammar,
                             IPattern.Exact(MultiplicativeExpressionGrammar, IPattern.Tokens(TokenType.Multiply), UnaryExpressionGrammar),
                             IPattern.Exact(MultiplicativeExpressionGrammar, IPattern.Tokens(TokenType.Divide), UnaryExpressionGrammar),
                             IPattern.Exact(MultiplicativeExpressionGrammar, IPattern.Tokens(TokenType.Remainder), UnaryExpressionGrammar))
                );

            AdditiveExpressionGrammar.SetGrammar(
                IPattern.Any(UnaryExpressionGrammar,
                             IPattern.Exact(AdditiveExpressionGrammar, IPattern.Tokens(TokenType.Add), UnaryExpressionGrammar),
                             IPattern.Exact(AdditiveExpressionGrammar, IPattern.Tokens(TokenType.Subtract), UnaryExpressionGrammar))
                );

            AssignmentOperatorGrammar.SetGrammar(
                IPattern.Any(TokenType.Assign,
                             TokenType.AddAssign,
                             TokenType.SubtractAssign,
                             TokenType.MultiplyAssign,
                             TokenType.DivideAssign,
                             TokenType.ModuloAssign,
                             TokenType.AndAssign,
                             TokenType.OrAssign,
                             TokenType.XOrAssign,
                             TokenType.LeftShiftAssign,
                             TokenType.RightShiftAssign)
                );

            AssignmentGrammar.SetGrammar(
                UnaryExpressionGrammar, AssignmentOperatorGrammar, ExpressionGrammar
                );

            ConstantExpressionGrammar.SetGrammar(
                ExpressionGrammar
                );

            IfStatementGrammar.SetGrammar(
                IPattern.Any(IPattern.Exact(IPattern.Tokens(TokenType.KeywordIf, TokenType.LeftParenthesis), BooleanExpressionGrammar, IPattern.Tokens(TokenType.RightParenthesis), EmbeddedStatementGrammar),
                             IPattern.Exact(IPattern.Tokens(TokenType.KeywordIf, TokenType.LeftParenthesis), BooleanExpressionGrammar, IPattern.Tokens(TokenType.RightParenthesis), EmbeddedStatementGrammar, IPattern.Tokens(TokenType.KeywordElse), EmbeddedStatementGrammar))
                );

            ExpressionStatementGrammar.SetGrammar(
                StatementExpressionGrammar, StatementEliminatorGrammar
                );

            InvocationExpressionGrammar.SetGrammar(
                PrimaryExpressionGrammar, IPattern.Tokens(TokenType.LeftParenthesis), IPattern.Optional(ArgumentListGrammar), IPattern.Tokens(TokenType.RightParenthesis)
                );

            ArgumentListGrammar.SetGrammar(
                ArgumentGrammar, IPattern.ZeroOrMany(IPattern.Tokens(TokenType.Comma), ArgumentGrammar)
                );

            ArgumentGrammar.SetGrammar(
                IPattern.Optional(ArgumentNameGrammar), ArgumentValueGrammar
                );

            ArgumentNameGrammar.SetGrammar(
                IdentifierGrammar, IPattern.Tokens(TokenType.Colon)
                );

            // can be expanded for passing by reference
            ArgumentValueGrammar.SetGrammar(
                IPattern.Any(ArgumentValueGrammar)
                );

            IdentifierGrammar.SetGrammar(
                IPattern.Tokens(TokenType.Symbol)
                );

            ShiftExpressionGrammar.SetGrammar(
                IPattern.Any(AdditiveExpressionGrammar,
                             IPattern.Exact(ShiftExpressionGrammar, IPattern.Tokens(TokenType.LeftShift), AdditiveExpressionGrammar),
                             IPattern.Exact(ShiftExpressionGrammar, IPattern.Tokens(TokenType.RightShift), AdditiveExpressionGrammar))
                );

            RelationalExpressionGrammmar.SetGrammar(
                IPattern.Any(ShiftExpressionGrammar,
                             IPattern.Exact(RelationalExpressionGrammmar, IPattern.Tokens(TokenType.LessThan), ShiftExpressionGrammar),
                             IPattern.Exact(RelationalExpressionGrammmar, IPattern.Tokens(TokenType.GreaterThan), ShiftExpressionGrammar),
                             IPattern.Exact(RelationalExpressionGrammmar, IPattern.Tokens(TokenType.LessThanOrEqual), ShiftExpressionGrammar),
                             IPattern.Exact(RelationalExpressionGrammmar, IPattern.Tokens(TokenType.GreaterThanOrEqual), ShiftExpressionGrammar))
                );

            EqualityExpressionGrammar.SetGrammar(
                IPattern.Any(RelationalExpressionGrammmar,
                             IPattern.Exact(EqualityExpressionGrammar, IPattern.Tokens(TokenType.EqualsTo), RelationalExpressionGrammmar),
                             IPattern.Exact(EqualityExpressionGrammar, IPattern.Tokens(TokenType.NotEqualsTo), RelationalExpressionGrammmar))
                );

            AndExpressionGrammar.SetGrammar(
                IPattern.Any(EqualityExpressionGrammar,
                             IPattern.Exact(AndExpressionGrammar, IPattern.Tokens(TokenType.And), EqualityExpressionGrammar))
                );

            ExclusiveOrExpressionGrammar.SetGrammar(
                IPattern.Any(AndExpressionGrammar,
                             IPattern.Exact(ExclusiveOrExpressionGrammar, IPattern.Tokens(TokenType.XOr), AndExpressionGrammar))
                );

            InclusiveOrExpressionGramar.SetGrammar(
                IPattern.Any(ExclusiveOrExpressionGrammar,
                             IPattern.Exact(InclusiveOrExpressionGramar, IPattern.Tokens(TokenType.Or), ExclusiveOrExpressionGrammar))
                );

            ConditionalAndExpressionGrammar.SetGrammar(
                IPattern.Any(InclusiveOrExpressionGramar,
                             IPattern.Exact(ConditionalAndExpressionGrammar, IPattern.Tokens(TokenType.ConditionalAnd), InclusiveOrExpressionGramar))
                );

            ConditionalOrExpressionGrammar.SetGrammar(
                IPattern.Any(ConditionalAndExpressionGrammar,
                             IPattern.Exact(ConditionalOrExpressionGrammar, IPattern.Tokens(TokenType.ConditionalOr), ConditionalAndExpressionGrammar))
                );

            BlockGrammar.SetGrammar(
                IPattern.Tokens(TokenType.LeftCurlyBacket), IPattern.Optional(StatementListGrammar), IPattern.Tokens(TokenType.RightCurlyBacket)
                );

            StatementListGrammar.SetGrammar(
                IPattern.OnceOrMany(StatementGrammar)
                );

            EmbeddedStatementGrammar.SetGrammar(
                IPattern.Any(BlockGrammar,
                             ExpressionStatementGrammar),
                             SelectionStatementGrammar,
                             // iteration statement for loops
                             JumpStatementGrammar
                );

            JumpStatementGrammar.SetGrammar(
                IPattern.Any(// break statement
                             // continue statement
                             ReturnStatementGrammar)
                );

            ReturnStatementGrammar.SetGrammar(
                IPattern.Tokens(TokenType.Return), IPattern.Optional(ExpressionGrammar), StatementEliminatorGrammar
                );

            StatementGrammar.SetGrammar(
                IPattern.Any(DeclarationStatementGrammar,
                             EmbeddedStatementGrammar)
                );

            DeclarationStatementGrammar.SetGrammar(
                IPattern.Any(IPattern.Exact(LocalVariableDeclaratorGrammar, StatementEliminatorGrammar)
                             /* local constant declaration */)
                );

            LocalVariableDeclaratorGrammar.SetGrammar(
                IPattern.Any(IPattern.Exact(IdentifierGrammar, IPattern.Tokens(TokenType.Colon), TypeGrammar),
                             IPattern.Exact(IdentifierGrammar, IPattern.Tokens(TokenType.Colon), TypeGrammar, IPattern.Tokens(TokenType.Assign), LocalVariableInitializerGrammar))
                );

            LocalVariableInitializerGrammar.SetGrammar(
                IPattern.Any(ExpressionGrammar
                             /* array initializer */)
                );

            SelectionStatementGrammar.SetGrammar(
                IPattern.Any(IfStatementGrammar
                             /* switch statement */)
                );

            TypeGrammar.SetGrammar(
                IPattern.Any(ValueTypeGrammar)
                );

            NumericTypeGrammar.SetGrammar(
                IPattern.Any(IntegerTypeGrammar,
                             FloatingPointTypeGrammar)
                );

            IntegerTypeGrammar.SetGrammar(
                IPattern.Optional(SignedGrammar), IPattern.Optional(TypeSizeGrammar), IPattern.Tokens(TokenType.KeywordWhole, TokenType.KeywordNumber)
                );

            FloatingPointTypeGrammar.SetGrammar(
                IPattern.Optional(TypeSizeGrammar), IPattern.Tokens(TokenType.KeywordNumber)
                );

            SignedGrammar.SetGrammar(
                IPattern.Any(TokenType.KeywordUnsigned,
                             TokenType.KeywordSigned)
                );

            TypeSizeGrammar.SetGrammar(
                IPattern.Any(TokenType.KeywordTiny,
                             TokenType.KeywordSmall,
                             TokenType.KeywordBig)
                );

            ValueTypeGrammar.SetGrammar(
                IPattern.Any(TypeNameGrammar,
                             SimpleValueTypeGrammar)
                );

            SimpleValueTypeGrammar.SetGrammar(
                IPattern.Any(NumericTypeGrammar,
                             IPattern.Tokens(TokenType.KeywordBoolean))
                );

            TypeNameGrammar.SetGrammar(
                NamespaceOrTypeNameGrammar
                );

            NamespaceNameGrammar.SetGrammar(
                NamespaceOrTypeNameGrammar
                );

            NamespaceOrTypeNameGrammar.SetGrammar(
                IPattern.ZeroOrMany(IdentifierGrammar, IPattern.Tokens(TokenType.Period)), IdentifierGrammar

                //IPattern.Any(IdentifierGrammar,
                //             IPattern.Exact(NamespaceNameGrammar, IPattern.Tokens(TokenType.Period), IdentifierGrammar))
                );

            PostIncrementExpressionGrammar.SetGrammar(
                PrimaryExpressionGrammar, IPattern.Tokens(TokenType.Increment)
                );

            PostDecrementExpressionGrammar.SetGrammar(
                PrimaryExpressionGrammar, IPattern.Tokens(TokenType.Decrement)
                );

            PrimaryExpressionGrammar.SetGrammar(
                IPattern.Any(PrimaryNoArrayCreationExpression)
                );

            StatementExpressionGrammar.SetGrammar(
                IPattern.Any(// null conditional invocation
                             InvocationExpressionGrammar,
                             // object creation expression
                             AssignmentGrammar,
                             PostIncrementExpressionGrammar,
                             PostDecrementExpressionGrammar,
                             PreIncrementExpressionGrammar,
                             PreDecrementExpressionGrammar)
                );

            LiteralGrammar.SetGrammar(
                IPattern.Any(BooleanLiteralGrammar,
                             IntegerLiteralGrammar,
                             FloatLiteralGrammar)
                );

            IntegerLiteralGrammar.SetGrammar(
                IPattern.Any(TokenType.Int8Literal,
                             TokenType.Uint8Literal,
                             TokenType.Int16Literal,
                             TokenType.Uint16Literal,
                             TokenType.Int32Literal,
                             TokenType.Uint32Literal,
                             TokenType.Int64Literal,
                             TokenType.Uint64Literal)
                );

            FloatLiteralGrammar.SetGrammar(
                IPattern.Any(TokenType.FloatLiteral,
                             TokenType.DoubleLiteral)
                );

            PrimaryNoArrayCreationExpression.SetGrammar(
                IPattern.Any(LiteralGrammar,
                            // interpolated string
                            ParenthesizedExpressionGrammar,
                            // member access
                            InvocationExpressionGrammar,
                            // array element access
                            PostIncrementExpressionGrammar,
                            PostDecrementExpressionGrammar
                            // object creation expression
                            )
                );

            ParenthesizedExpressionGrammar.SetGrammar(
                IPattern.Tokens(TokenType.LeftParenthesis), ExpressionGrammar, IPattern.Tokens(TokenType.RightParenthesis)
                );

            #endregion // Initialization
        }
    }
}

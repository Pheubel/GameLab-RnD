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
        /// | '\0'
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
        /// : <see cref="MultiplicativeExpressionGrammar"/><br/>
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
        /// | <see cref="InclusiveOrExpressionGrammar"/> '|' <see cref="ExclusiveOrExpressionGrammar"/>
        /// </summary>
        static readonly Grammar InclusiveOrExpressionGrammar;

        /// <summary>
        /// : <see cref="InclusiveOrExpressionGrammar"/><br/>
        /// | <see cref="ConditionalAndExpressionGrammar"/> '<![CDATA[&]]><![CDATA[&]]>' <see cref="InclusiveOrExpressionGrammar"/>
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
        /// : <see cref="StatementEliminatorGrammar"/>
        /// </summary>
        static readonly Grammar EmptyStatementGrammar;

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
        /// : <see cref="IdentifierGrammar"/> '(' <see cref="ArgumentListGrammar"/>? ')'
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
        /// : <see cref="IdentifierGrammar"/> '++'
        /// </summary>
        static readonly Grammar PostIncrementExpressionGrammar;

        /// <summary>
        /// : <see cref="IdentifierGrammar"/> '--'
        /// </summary>
        static readonly Grammar PostDecrementExpressionGrammar;

        /// <summary>
        /// : <see cref="BooleanLiteralGrammar"/><br/>
        /// | <see cref="IntegerLiteralGrammar"/><br/>
        /// | <see cref="FloatLiteralGrammar"/><br/>
        /// | <see cref="StringLiteralGrammar"/>
        /// </summary>
        static readonly Grammar LiteralGrammar;

        static readonly Grammar IntegerLiteralGrammar;

        static readonly Grammar FloatLiteralGrammar;

        static readonly Grammar StringLiteralGrammar;

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

        /// <summary>
        /// : <see cref="StatementEliminatorGrammar"/>
        /// </summary>
        static readonly Grammar EmptyExpresionGrammar;

        /// <summary>
        /// : <see cref="FunctionNoReturnValueDeclarationGrammar"/>
        /// | <see cref="FunctionDelarationGrammar"/>
        /// </summary>
        static readonly Grammar FunctionDelarationGrammar;

        /// <summary>
        /// : 'function' <see cref="IdentifierGrammar"/> '(' <see cref="ArgumentListGrammar"/>? ')' <see cref="BlockGrammar"/>
        /// </summary>
        static readonly Grammar FunctionNoReturnValueDeclarationGrammar;

        /// <summary>
        /// : 'function' <see cref="IdentifierGrammar"/> '(' <see cref="ArgumentListGrammar"/>? ')' ':' <see cref="TypeGrammar"/> <see cref="BlockGrammar"/>
        /// </summary>
        static readonly Grammar FunctionReturnValueDeclarationGrammar;

        /// <summary>
        /// : <see cref="ImportGrammar"/>* <see cref="StoryPartGrammar"/>*
        /// </summary>
        static readonly Grammar StoryGrammar;

        /// <summary>
        /// : '@' 'import' <see cref="StringLiteralGrammar"/> <see cref="NewLineGrammar"/>+
        /// </summary>
        static readonly Grammar ImportGrammar;

        /// <summary>
        /// : <see cref="StoryAtomGrammar"/>
        /// | <see cref="StoryEmbeddedStatementGrammar"/>
        /// </summary>
        static readonly Grammar StoryPartGrammar;

        /// <summary>
        /// : characters+<br/>
        /// | <see cref="LiteralGrammar"/><br/>
        /// | <see cref="KeywordsGrammar"/><br/>
        /// | <see cref="EscapedCharactersGrammar"/><br/>
        /// | <see cref="EmbeddedValueInStoryGrammar"/>
        /// </summary>
        static readonly Grammar StoryAtomGrammar;

        /// <summary>
        /// : <see cref="StoryEmbeddedChoiceStatementGrammar"/><br/>
        /// | <see cref="StoryEmbeddedIfStatementGrammar"/><br/>
        /// | <see cref="StoryEmbeddedExpressionStatementGrammar"/><br/>
        /// | <see cref="StoryEmbeddedCodeGrammar"/>
        /// </summary>
        static readonly Grammar StoryEmbeddedStatementGrammar;

        /// <summary>
        /// : <see cref="EmbeddedVariableInStoryGrammar"/>
        /// </summary>
        static readonly Grammar EmbeddedValueInStoryGrammar;

        /// <summary>
        /// : '\@'<br/>
        /// | '\\'<br/>
        /// | '\n'<br/>
        /// | '\|'
        /// </summary>
        static readonly Grammar EscapedCharactersGrammar;

        /// <summary>
        /// : 'signed'<br/>
        /// | 'unsigned'<br/>
        /// | 'number'<br/>
        /// | 'tiny'<br/>
        /// | 'small'<br/>
        /// | 'big'<br/>
        /// | 'whole'<br/>
        /// | 'return'<br/>
        /// | 'import'
        /// </summary>
        static readonly Grammar KeywordsGrammar;

        /// <summary>
        /// : '@' <see cref="IdentifierGrammar"/>
        /// </summary>
        static readonly Grammar EmbeddedVariableInStoryGrammar;

        /// <summary>
        /// : '@' 'if' '(' <see cref="BooleanExpressionGrammar"/> ')' '{' <see cref="StoryPartGrammar"/>* '}'<br/>
        /// | '@' 'if' '(' <see cref="BooleanExpressionGrammar"/> ')' '{' <see cref="StoryPartGrammar"/>* '}' '@' 'else' '{' <see cref="StoryPartGrammar"/>* '}'
        /// </summary>
        static readonly Grammar StoryEmbeddedIfStatementGrammar;

        static readonly Grammar StoryEmbeddedChoiceStatementGrammar;

        static readonly Grammar StoryChoiceExpressionGrammar;

        static readonly Grammar StoryChoiceGrammar;

        static readonly Grammar StoryEmbeddedVariableDeclarationGrammar;

        /// <summary>
        /// : '@' 'code' <see cref="BlockGrammar"/>
        /// </summary>
        static readonly Grammar StoryEmbeddedCodeGrammar;

        /// <summary>
        /// : '@' <see cref="StoryExpressionStatementGrammar"/>
        /// </summary>
        static readonly Grammar StoryEmbeddedExpressionStatementGrammar;

        /// <summary>
        /// : <see cref="LocalVariableDeclaratorGrammar"/> <see cref="NewLineGrammar"/>
        /// </summary>
        static readonly Grammar StoryDeclarationStatementGrammar;

        static readonly Grammar StoryExpressionStatementGrammar;

        static readonly Grammar EmptyStoryStatementGrammar;

        static readonly Grammar NonAssignmentExpressionGrammar;

        static readonly Grammar StorySentenceGrammar;

        static readonly Grammar StoryLineTerminatorGrammar;

        static readonly Grammar StorySentenceLegalTokenGrammar;

        #endregion // Grammars


        static SyntaxAnalyzer()
        {
            #region Instantiation

            NewLineGrammar = new Grammar(GrammarKind.NewLine);
            CharacterGrammar = new Grammar(GrammarKind.Character);
            BooleanLiteralGrammar = new Grammar(GrammarKind.BooleanLiteral);
            StatementEliminatorGrammar = new Grammar(GrammarKind.StatementEliminator);
            UnaryExpressionGrammar = new Grammar(GrammarKind.UnaryExpression);
            PreIncrementExpressionGrammar = new Grammar(GrammarKind.PreIncrementExpression);
            PreDecrementExpressionGrammar = new Grammar(GrammarKind.PreDecrementExpression);
            CastExpressionGrammar = new Grammar(GrammarKind.CastExpression);
            ReturnTypeGrammar = new Grammar(GrammarKind.ReturnType);
            MultiplicativeExpressionGrammar = new Grammar(GrammarKind.MultiplicativeExpression);
            AdditiveExpressionGrammar = new Grammar(GrammarKind.AdditiveExpression);
            AssignmentOperatorGrammar = new Grammar(GrammarKind.AssignmentOperator);
            BooleanExpressionGrammar = new Grammar(GrammarKind.BooleanExpression);
            AssignmentGrammar = new Grammar(GrammarKind.Assignment);
            ExpressionGrammar = new Grammar(GrammarKind.Expression);
            ConstantExpressionGrammar = new Grammar(GrammarKind.ConstantExpression);
            IfStatementGrammar = new Grammar(GrammarKind.IfStatement);
            ExpressionStatementGrammar = new Grammar(GrammarKind.ExpressionStatement);
            InvocationExpressionGrammar = new Grammar(GrammarKind.InvocationExpression);
            ArgumentGrammar = new Grammar(GrammarKind.Argument);
            ArgumentNameGrammar = new Grammar(GrammarKind.ArgumentName);
            IdentifierGrammar = new Grammar(GrammarKind.Identifier);
            ArgumentValueGrammar = new Grammar(GrammarKind.ArgumentValue);
            ShiftExpressionGrammar = new Grammar(GrammarKind.ShiftExpression);
            RelationalExpressionGrammmar = new Grammar(GrammarKind.RelationalExpression);
            EqualityExpressionGrammar = new Grammar(GrammarKind.EqualityExpression);
            AndExpressionGrammar = new Grammar(GrammarKind.AndExpression);
            ExclusiveOrExpressionGrammar = new Grammar(GrammarKind.ExclusiveOrExpression);
            InclusiveOrExpressionGrammar = new Grammar(GrammarKind.InclusiveOrExpression);
            ConditionalAndExpressionGrammar = new Grammar(GrammarKind.ConditionalAndExpression);
            ConditionalOrExpressionGrammar = new Grammar(GrammarKind.ConditionalOrExpression);
            BlockGrammar = new Grammar(GrammarKind.Block);
            StatementListGrammar = new Grammar(GrammarKind.StatementList);
            EmbeddedStatementGrammar = new Grammar(GrammarKind.EmbeddedStatement);
            JumpStatementGrammar = new Grammar(GrammarKind.JumpStatement);
            ReturnStatementGrammar = new Grammar(GrammarKind.ReturnStatement);
            StatementGrammar = new Grammar(GrammarKind.Statement);
            SelectionStatementGrammar = new Grammar(GrammarKind.SelectionStatement);
            LocalVariableInitializerGrammar = new Grammar(GrammarKind.LocalVariableInitializer);
            ArgumentListGrammar = new Grammar(GrammarKind.ArgumentList);
            DeclarationStatementGrammar = new Grammar(GrammarKind.DeclarationStatement);
            LocalVariableDeclaratorGrammar = new Grammar(GrammarKind.LocalVariableDeclarator);
            IntegerTypeGrammar = new Grammar(GrammarKind.IntegerType);
            FloatingPointTypeGrammar = new Grammar(GrammarKind.FloatingPointType);
            TypeSizeGrammar = new Grammar(GrammarKind.TypeSize);
            SignedGrammar = new Grammar(GrammarKind.Signed);
            NumericTypeGrammar = new Grammar(GrammarKind.NumericType);
            SimpleValueTypeGrammar = new Grammar(GrammarKind.SimpleValueType);
            NamespaceNameGrammar = new Grammar(GrammarKind.NamespaceName);
            NamespaceOrTypeNameGrammar = new Grammar(GrammarKind.NamespaceOrTypeName);
            TypeNameGrammar = new Grammar(GrammarKind.TypeName);
            ValueTypeGrammar = new Grammar(GrammarKind.ValueType);
            TypeGrammar = new Grammar(GrammarKind.Type);
            PostIncrementExpressionGrammar = new Grammar(GrammarKind.PostIncrementExpression);
            PostDecrementExpressionGrammar = new Grammar(GrammarKind.PostDecrementExpression);
            IntegerLiteralGrammar = new Grammar(GrammarKind.IntegerLiteral);
            FloatLiteralGrammar = new Grammar(GrammarKind.FloatLiteral);
            LiteralGrammar = new Grammar(GrammarKind.Literal);
            ParenthesizedExpressionGrammar = new Grammar(GrammarKind.ParenthesizedExpression);
            PrimaryExpressionGrammar = new Grammar(GrammarKind.PrimaryExpression);
            StatementExpressionGrammar = new Grammar(GrammarKind.StatementExpression);
            PrimaryNoArrayCreationExpression = new Grammar(GrammarKind.PrimaryNoArrayCreation);
            EmptyExpresionGrammar = new Grammar(GrammarKind.EmptyExpression);
            FunctionReturnValueDeclarationGrammar = new Grammar(GrammarKind.FunctionReturnValueDeclaration);
            FunctionNoReturnValueDeclarationGrammar = new Grammar(GrammarKind.FunctionNoReturnValueDeclaration);
            FunctionDelarationGrammar = new Grammar(GrammarKind.FunctionDeclaration);
            KeywordsGrammar = new Grammar(GrammarKind.Keywords);
            EmbeddedVariableInStoryGrammar = new Grammar(GrammarKind.EmbeddedVariableInStory);
            StoryGrammar = new Grammar(GrammarKind.Story);
            ImportGrammar = new Grammar(GrammarKind.Import);
            StoryAtomGrammar = new Grammar(GrammarKind.StoryAtom);
            EscapedCharactersGrammar = new Grammar(GrammarKind.EscapedCharacters);
            StoryPartGrammar = new Grammar(GrammarKind.StoryPart);
            EmbeddedValueInStoryGrammar = new Grammar(GrammarKind.EmbeddedValueInStory);
            StoryEmbeddedExpressionStatementGrammar = new Grammar(GrammarKind.StoryEmbeddedExpressionStatement);
            StoryEmbeddedIfStatementGrammar = new Grammar(GrammarKind.StoryEmbeddedIfStatement);
            StoryDeclarationStatementGrammar = new Grammar(GrammarKind.StoryDeclarationStatement);
            StoryEmbeddedChoiceStatementGrammar = new Grammar(GrammarKind.StoryEmbeddedChoiceStatement);
            StoryEmbeddedVariableDeclarationGrammar = new Grammar(GrammarKind.StoryEmbeddedVariableDeclaration);
            StoryChoiceGrammar = new Grammar(GrammarKind.StoryChoice);
            StoryChoiceExpressionGrammar = new Grammar(GrammarKind.StoryChoiceExpression);
            StoryExpressionStatementGrammar = new Grammar(GrammarKind.StoryExpressionStatement);
            StoryEmbeddedStatementGrammar = new Grammar(GrammarKind.StoryEmbeddedStatement);
            StringLiteralGrammar = new Grammar(GrammarKind.StringLiteral);
            EmptyStatementGrammar = new Grammar(GrammarKind.EmptyStatement);
            StoryEmbeddedCodeGrammar = new Grammar(GrammarKind.StoryEmbeddedCode);
            EmptyStoryStatementGrammar = new Grammar(GrammarKind.EmptyStoryStatement);
            NonAssignmentExpressionGrammar = new Grammar(GrammarKind.NonAssignmentExpression);
            StorySentenceGrammar = new Grammar(GrammarKind.StorySentence);
            StoryLineTerminatorGrammar = new Grammar(GrammarKind.StoryLineTerminator);
            StorySentenceLegalTokenGrammar = new Grammar(GrammarKind.StorySentenceLegalToken);

            #endregion // Instantiation



            #region Initialization

            StoryGrammar.SetGrammar(
                IPattern.ZeroOrMany(ImportGrammar), IPattern.ZeroOrMany(IPattern.Any(StoryPartGrammar, EmptyStoryStatementGrammar))
                );

            ImportGrammar.SetGrammar(
                IPattern.Tokens(TokenType.AtSign, TokenType.KeywordImport, TokenType.StringLiteral), IPattern.OnceOrMany(NewLineGrammar)
                );

            StoryPartGrammar.SetGrammar(
                IPattern.Any(StoryEmbeddedStatementGrammar, StorySentenceGrammar), StoryLineTerminatorGrammar
                );

            StoryAtomGrammar.SetGrammar(
                IPattern.Any(IPattern.Tokens(TokenType.RawText),
                             LiteralGrammar,
                             KeywordsGrammar,
                             EscapedCharactersGrammar,
                             EmbeddedValueInStoryGrammar,
                             IdentifierGrammar,
                             StorySentenceLegalTokenGrammar)
                );

            StorySentenceLegalTokenGrammar.SetGrammar(
                IPattern.Any(TokenType.Add,
                             TokenType.Subtract,
                             TokenType.Multiply,
                             TokenType.Divide,
                             TokenType.Assign,
                             TokenType.AddAssign,
                             TokenType.SubtractAssign,
                             TokenType.MultiplyAssign,
                             TokenType.DivideAssign,
                             TokenType.EqualsTo,
                             TokenType.Increment,
                             TokenType.Decrement,
                             TokenType.Comma,
                             TokenType.SemiColon,
                             TokenType.Remainder,
                             TokenType.ModuloAssign,
                             TokenType.AndAssign,
                             TokenType.XOrAssign,
                             TokenType.LeftShiftAssign,
                             TokenType.RightShiftAssign,
                             TokenType.LessThan,
                             TokenType.LeftShift,
                             TokenType.RightShift,
                             TokenType.GreaterThan,
                             TokenType.GreaterThanOrEqual,
                             TokenType.LessThanOrEqual,
                             TokenType.ExclamationMark,
                             TokenType.NotEqualsTo,
                             TokenType.ConditionalAnd,
                             TokenType.And,
                             TokenType.XOr,
                             TokenType.Period)
                );

            StorySentenceGrammar.SetGrammar(
                IPattern.OnceOrMany(StoryAtomGrammar), IPattern.ZeroOrMany(IPattern.Tokens(TokenType.Or), NewLineGrammar, IPattern.OnceOrMany(StoryAtomGrammar))
                );

            EscapedCharactersGrammar.SetGrammar(
                IPattern.Any(TokenType.EscapedAtSign,
                             TokenType.EscapedBackslash,
                             TokenType.EscapedNewLine,
                             TokenType.EscapedPipe,
                             TokenType.EscapedColon)
                );

            KeywordsGrammar.SetGrammar(
                IPattern.Any(TokenType.KeywordSigned,
                             TokenType.KeywordUnsigned,
                             TokenType.KeywordNumber,
                             TokenType.KeywordTiny,
                             TokenType.KeywordSmall,
                             TokenType.KeywordBig,
                             TokenType.KeywordWhole,
                             TokenType.KeywordReturn,
                             TokenType.KeywordImport)
                );

            EmptyStoryStatementGrammar.SetGrammar(
                NewLineGrammar
                );

            StoryLineTerminatorGrammar.SetGrammar(
                IPattern.Any(NewLineGrammar,
                             IPattern.Tokens(TokenType.EndOfFile))
                );

            EmbeddedValueInStoryGrammar.SetGrammar(
                IPattern.Any(EmbeddedVariableInStoryGrammar)
                );

            EmbeddedVariableInStoryGrammar.SetGrammar(
                IPattern.Tokens(TokenType.AtSign), IdentifierGrammar
                );

            StoryEmbeddedStatementGrammar.SetGrammar(
                IPattern.Any(StoryEmbeddedVariableDeclarationGrammar,
                             StoryEmbeddedChoiceStatementGrammar,
                             StoryEmbeddedIfStatementGrammar,
                             StoryEmbeddedExpressionStatementGrammar,
                             StoryEmbeddedCodeGrammar)
                );

            StoryEmbeddedIfStatementGrammar.SetGrammar(
                IPattern.Any(IPattern.Exact(IPattern.Tokens(TokenType.AtSign, TokenType.KeywordIf, TokenType.LeftParenthesis), BooleanExpressionGrammar, IPattern.Tokens(TokenType.RightParenthesis, TokenType.LeftCurlyBracket), IPattern.ZeroOrMany(StoryPartGrammar), IPattern.Tokens(TokenType.RightCurlyBacket)),
                             IPattern.Exact(IPattern.Tokens(TokenType.AtSign, TokenType.KeywordIf, TokenType.LeftParenthesis), BooleanExpressionGrammar, IPattern.Tokens(TokenType.RightParenthesis, TokenType.LeftCurlyBracket), IPattern.ZeroOrMany(StoryPartGrammar), IPattern.Tokens(TokenType.RightCurlyBacket, TokenType.AtSign, TokenType.KeywordElse, TokenType.LeftCurlyBracket), IPattern.ZeroOrMany(StoryPartGrammar), IPattern.Tokens(TokenType.RightCurlyBacket)))
                );

            StoryChoiceGrammar.SetGrammar(
                IPattern.OnceOrMany(StoryAtomGrammar), IPattern.Tokens(TokenType.Colon), NewLineGrammar
                );

            StoryEmbeddedChoiceStatementGrammar.SetGrammar(
                IPattern.Tokens(TokenType.AtSign, TokenType.KeywordChoice), IPattern.ZeroOrMany(TokenType.NewLine), IPattern.Tokens(TokenType.LeftCurlyBracket), IPattern.ZeroOrMany(TokenType.NewLine), IPattern.OnceOrMany(StoryChoiceGrammar, StoryChoiceExpressionGrammar), IPattern.Tokens(TokenType.RightCurlyBacket)
                );

            StoryChoiceExpressionGrammar.SetGrammar(
                IPattern.OnceOrMany(StoryPartGrammar)
                );

            StoryExpressionStatementGrammar.SetGrammar(
                StatementExpressionGrammar
                );

            StoryEmbeddedExpressionStatementGrammar.SetGrammar(
                IPattern.Tokens(TokenType.AtSign), StoryExpressionStatementGrammar
                );

            StoryEmbeddedVariableDeclarationGrammar.SetGrammar(
                IPattern.Tokens(TokenType.AtSign), StoryDeclarationStatementGrammar
                );

            StoryDeclarationStatementGrammar.SetGrammar(
                LocalVariableDeclaratorGrammar
                );

            StoryEmbeddedCodeGrammar.SetGrammar(
                IPattern.Tokens(TokenType.AtSign, TokenType.KeywordCode), BlockGrammar
                );

            NewLineGrammar.SetGrammar(
                IPattern.Tokens(TokenType.NewLine)
                );

            CharacterGrammar.SetGrammar(
                IPattern.None(TokenType.NewLine)
                );

            BooleanLiteralGrammar.SetGrammar(
                IPattern.Any(TokenType.KeywordTrue,
                             TokenType.KeywordFalse)
                );

            StatementEliminatorGrammar.SetGrammar(
                IPattern.Any(IPattern.Tokens(TokenType.SemiColon, TokenType.EndOfFile), NewLineGrammar)
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
                IPattern.Any(NonAssignmentExpressionGrammar,
                             AssignmentGrammar)
                );

            BooleanExpressionGrammar.SetGrammar(
                ExpressionGrammar
                );

            ReturnTypeGrammar.SetGrammar(
                IPattern.Any(TypeGrammar,
                             IPattern.Tokens(TokenType.KeywordNothing))
                );

            MultiplicativeExpressionGrammar.SetGrammar(
                IPattern.Any(IPattern.Exact(UnaryExpressionGrammar, IPattern.Tokens(TokenType.Multiply), UnaryExpressionGrammar),
                             IPattern.Exact(UnaryExpressionGrammar, IPattern.Tokens(TokenType.Divide), UnaryExpressionGrammar),
                             IPattern.Exact(UnaryExpressionGrammar, IPattern.Tokens(TokenType.Remainder), UnaryExpressionGrammar),
                             UnaryExpressionGrammar)
                );

            AdditiveExpressionGrammar.SetGrammar(
                IPattern.Any(IPattern.Exact(MultiplicativeExpressionGrammar, IPattern.Tokens(TokenType.Add), UnaryExpressionGrammar),
                             IPattern.Exact(MultiplicativeExpressionGrammar, IPattern.Tokens(TokenType.Subtract), UnaryExpressionGrammar),
                             MultiplicativeExpressionGrammar)
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

            // TODO: see if this can be better
            InvocationExpressionGrammar.SetGrammar(
                IdentifierGrammar, IPattern.Tokens(TokenType.LeftParenthesis), IPattern.Optional(ArgumentListGrammar), IPattern.Tokens(TokenType.RightParenthesis)
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
                IPattern.Any(IPattern.Exact(AdditiveExpressionGrammar, IPattern.Tokens(TokenType.LeftShift), AdditiveExpressionGrammar),
                             IPattern.Exact(AdditiveExpressionGrammar, IPattern.Tokens(TokenType.RightShift), AdditiveExpressionGrammar),
                             AdditiveExpressionGrammar)
                );

            RelationalExpressionGrammmar.SetGrammar(
                IPattern.Any(IPattern.Exact(ShiftExpressionGrammar, IPattern.Tokens(TokenType.LessThan), ShiftExpressionGrammar),
                             IPattern.Exact(ShiftExpressionGrammar, IPattern.Tokens(TokenType.GreaterThan), ShiftExpressionGrammar),
                             IPattern.Exact(ShiftExpressionGrammar, IPattern.Tokens(TokenType.LessThanOrEqual), ShiftExpressionGrammar),
                             IPattern.Exact(ShiftExpressionGrammar, IPattern.Tokens(TokenType.GreaterThanOrEqual), ShiftExpressionGrammar),
                             ShiftExpressionGrammar)
                );

            EqualityExpressionGrammar.SetGrammar(
                IPattern.Any(IPattern.Exact(RelationalExpressionGrammmar, IPattern.Tokens(TokenType.EqualsTo), RelationalExpressionGrammmar),
                             IPattern.Exact(RelationalExpressionGrammmar, IPattern.Tokens(TokenType.NotEqualsTo), RelationalExpressionGrammmar),
                             RelationalExpressionGrammmar)
                );

            AndExpressionGrammar.SetGrammar(
                IPattern.Any(IPattern.Exact(EqualityExpressionGrammar, IPattern.Tokens(TokenType.And), EqualityExpressionGrammar),
                             EqualityExpressionGrammar)
                );

            ExclusiveOrExpressionGrammar.SetGrammar(
                IPattern.Any(IPattern.Exact(AndExpressionGrammar, IPattern.Tokens(TokenType.XOr), AndExpressionGrammar),
                             AndExpressionGrammar)
                );

            InclusiveOrExpressionGrammar.SetGrammar(
                IPattern.Any(IPattern.Exact(ExclusiveOrExpressionGrammar, IPattern.Tokens(TokenType.Or), ExclusiveOrExpressionGrammar),
                             ExclusiveOrExpressionGrammar)
                );

            ConditionalAndExpressionGrammar.SetGrammar(
                IPattern.Any(IPattern.Exact(InclusiveOrExpressionGrammar, IPattern.Tokens(TokenType.ConditionalAnd), InclusiveOrExpressionGrammar),
                             InclusiveOrExpressionGrammar)
                );

            ConditionalOrExpressionGrammar.SetGrammar(
                IPattern.Any(IPattern.Exact(ConditionalAndExpressionGrammar, IPattern.Tokens(TokenType.ConditionalOr), ConditionalAndExpressionGrammar),
                             ConditionalAndExpressionGrammar)
                );

            BlockGrammar.SetGrammar(
                IPattern.Tokens(TokenType.LeftCurlyBracket), IPattern.Optional(StatementListGrammar), IPattern.Tokens(TokenType.RightCurlyBacket)
                );

            EmptyStatementGrammar.SetGrammar(
                StatementEliminatorGrammar
                );

            StatementListGrammar.SetGrammar(
                IPattern.OnceOrMany(StatementGrammar)
                );

            EmbeddedStatementGrammar.SetGrammar(
                IPattern.Any(BlockGrammar,
                             EmptyStatementGrammar,
                             ExpressionStatementGrammar,
                             SelectionStatementGrammar,
                             // iteration statement for loops
                             JumpStatementGrammar)
                );

            JumpStatementGrammar.SetGrammar(
                IPattern.Any(// break statement
                             // continue statement
                             ReturnStatementGrammar)
                );

            ReturnStatementGrammar.SetGrammar(
                IPattern.Tokens(TokenType.KeywordReturn), IPattern.Optional(ExpressionGrammar), StatementEliminatorGrammar
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
                IdentifierGrammar, IPattern.Tokens(TokenType.Increment)
                );

            PostDecrementExpressionGrammar.SetGrammar(
                IdentifierGrammar, IPattern.Tokens(TokenType.Decrement)
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
                             FloatLiteralGrammar,
                             StringLiteralGrammar)
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

            StringLiteralGrammar.SetGrammar(
                IPattern.Tokens(TokenType.StringLiteral)
                );

            PrimaryNoArrayCreationExpression.SetGrammar(
                IPattern.Any(LiteralGrammar,
                            // interpolated string
                            IdentifierGrammar,
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

            EmptyExpresionGrammar.SetGrammar(
                StatementEliminatorGrammar
                );

            FunctionReturnValueDeclarationGrammar.SetGrammar(
                IPattern.Tokens(TokenType.KeywordFunction), IdentifierGrammar, IPattern.Tokens(TokenType.LeftParenthesis), IPattern.Optional(ArgumentListGrammar), IPattern.Tokens(TokenType.Colon), TypeGrammar, BlockGrammar
                );

            FunctionNoReturnValueDeclarationGrammar.SetGrammar(
                IPattern.Tokens(TokenType.KeywordFunction), IdentifierGrammar, IPattern.Tokens(TokenType.LeftParenthesis), IPattern.Optional(ArgumentListGrammar), BlockGrammar
                );

            FunctionDelarationGrammar.SetGrammar(
                IPattern.Any(FunctionNoReturnValueDeclarationGrammar,
                             FunctionReturnValueDeclarationGrammar)
                );

            NonAssignmentExpressionGrammar.SetGrammar(
                ConditionalOrExpressionGrammar
                );

            #endregion // Initialization
        }
    }
}

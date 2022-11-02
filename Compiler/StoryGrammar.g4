parser grammar StoryGrammar;
options { tokenVocab = StoryLexerGrammar; }

story
    : (emptySentence | import_statement)* (emptySentence | storyPart)*
    ;

emptySentence
    : LineTerminator
    ;

import_statement
    : ImportStatementBegin stringLiteral EndEmbeddedStatement?
    ;

stringLiteral
    : Open_String_Literal string_literal_value? Close_String_Literal
    ;

string_literal_value
    : String_Literal_Character+
    ;

storyPart
    : storyStatement
    | storySentence
    ;

storySentence
    : storyFormattedSentence 
    | storyUnformattedSentence 
    ;

storyUnformattedSentence
    :  StartSentencePart Sentence_Part*
    ;

storyFormattedSentence
    :  (OpenEmbeddedVariableOutsideStory Identifier CloseEmbeddedVariable) ((OpenEmbeddedVariableInsideStory Identifier Sentence_Close_Embedded_Variable) | Sentence_Part)*
    ;

storyStatement
    : storyIfStatement
    | storyVariableDeclaration
    ;

storyVariableDeclaration
    : StoryEmbedSymbol identifier COLON identifier LineTerminator
    ;

storyIfStatement
    : StoryIfStatementBegin Embedded_If_Enter_Condition booleanExpression Embedded_If_Leave_Condition SCOPE_OPEN (emptySentence | storyPart)* SCOPE_CLOSE LineTerminator
    // | StoryIfStatementBegin Embedded_If_Enter_Condition booleanExpression Embedded_If_Leave_Condition SCOPE_OPEN (emptySentence | storyPart)* SCOPE_CLOSE StoryElseStatement SCOPE_OPEN (emptySentence | storyPart)* SCOPE_CLOSE LineTerminator
    ;

literal
    : booleanLiteral
    | Integer_Literal
    | Real_Literal
    | stringLiteral
    ;

booleanLiteral
    : TRUE
    | FALSE
    ;

expression
    : non_assignment_expression
    | assignment
    ;

assignment
    : unary_expression AssignmentOperator expression
    ;

booleanExpression
    : expression
    ;

expression_statement
    : statement_expression LineTerminator
    ;

statement_expression
    : invocation_expression
    | object_creation_expression
    | assignment
    | post_increment_expression
    | post_decrement_expression
    | pre_increment_expression
    | pre_decrement_expression
    ;

method_invocation
    : LeftParenthesis argument_list? RightParenthesis
    ;

invocation_expression
    : primary_expression LeftParenthesis argument_list? RightParenthesis
    ;

if_statement
    : If LeftParenthesis booleanExpression RightParenthesis embedded_statement
    | If LeftParenthesis booleanExpression RightParenthesis embedded_statement
      Else embedded_statement
    ;

return_statement
    : Return expression? LineTerminator
    ;

statement
    : declaration_statement
    | embedded_statement
    ;

declaration_statement
    : local_variable_declaration LineTerminator
    // | local_constant_declaration LineTerminator  
    ;

embedded_statement
    : block
    | empty_statement
    | expression_statement
    | selection_statement
    // | iteration_statement
    | jump_statement
    ;

empty_statement
    : LineTerminator
    ;

local_variable_declaration
    : identifier COLON local_variable_type 
    | identifier COLON local_variable_type Assign local_variable_initializer
    ;

selection_statement
    : if_statement
    // | switch_statement
    ;

jump_statement
    : // break_statement
    // | continue_statement
    return_statement
    // | throw_statement
    ;

object_creation_expression
    : Object_Creation_Keyword type LeftParenthesis argument_list? RightParenthesis
    ;

local_variable_type
    : type
    ;

local_variable_initializer
    : expression
    // | array_initializer
    ;

block
    : LeftCurlyBracket statement_list? RigthCurlyBracket
    ;

statement_list
    : statement+
    ;

non_assignment_expression
    : conditional_expression
    ;

multiplicative_expression
    : unary_expression
    | multiplicative_expression MULTIPLY unary_expression
    | multiplicative_expression DIVIDE unary_expression
    | multiplicative_expression REMAINDER unary_expression
    ;

additive_expression
    : multiplicative_expression
    | additive_expression PLUS multiplicative_expression
    | additive_expression MINUS multiplicative_expression
    ;

shift_expression
    : additive_expression
    | shift_expression LEFT_SHIFT additive_expression
    | shift_expression RIGHT_SHIFT additive_expression
    ;

relational_expression
    : shift_expression
    | relational_expression Less_Than shift_expression
    | relational_expression Greater_Than shift_expression
    | relational_expression Less_Than_Or_Equal_To shift_expression
    | relational_expression Greater_Than_Or_Equal_To shift_expression
    // | relational_expression 'is' type
    // | relational_expression 'as' type
    ;

equality_expression
    : relational_expression
    | equality_expression Equal_To relational_expression
    | equality_expression Not_Equal_To relational_expression
    ;

and_expression
    : equality_expression
    | and_expression BITWISE_AND equality_expression
    ;

exclusive_or_expression
    : and_expression
    | exclusive_or_expression BITWISE_XOR and_expression
    ;

inclusive_or_expression
    : exclusive_or_expression
    | inclusive_or_expression BITWISE_OR exclusive_or_expression
    ;

conditional_and_expression
    : inclusive_or_expression
    | conditional_and_expression CONDITIONAL_AND inclusive_or_expression
    ;

conditional_or_expression
    : conditional_and_expression
    | conditional_or_expression CONDITIONAL_OR conditional_and_expression
    ;

null_coalescing_expression
    : conditional_or_expression
    ;

conditional_expression
    : null_coalescing_expression
    | null_coalescing_expression QUESTION_MARK expression COLON expression
    ;

unary_expression
    : primary_expression
    | PLUS unary_expression
    | MINUS unary_expression
    | CONDITIONAL_NOT unary_expression
    | BITWISE_NOT unary_expression
    | pre_increment_expression
    | pre_decrement_expression
    ;

pre_increment_expression
    : Increment unary_expression
    ;

pre_decrement_expression
    : Decrement unary_expression
    ;

post_increment_expression
    : primary_expression Increment
    ;

post_decrement_expression
    : primary_expression Decrement
    ;


primary_expression
    : primary_expression_start bracket_expression* ((member_access | method_invocation | Increment | Decrement | identifier) bracket_expression* )*
    ;
    

primary_expression_start
    : literal
    | simple_name
    | parenthesized_expression
    | member_access
    | this_access
    | object_creation_expression
    ;

// primary_expression_start
// 	: literal                                   #literalExpression
// 	| identifier type_argument_list?            #simpleNameExpression
// 	| OPEN_PARENS expression CLOSE_PARENS       #parenthesisExpressions
// 	| predefined_type                           #memberAccessExpression
// 	| qualified_alias_member                    #memberAccessExpression
// 	| LITERAL_ACCESS                            #literalAccessExpression
// 	| THIS                                      #thisReferenceExpression
// 	| BASE ('.' identifier type_argument_list? | '[' expression_list ']') #baseAccessExpression
// 	| NEW (type_ (object_creation_expression
// 	             | object_or_collection_initializer
// 	             | '[' expression_list ']' rank_specifier* array_initializer?
// 	             | rank_specifier+ array_initializer)
// 	      | anonymous_object_initializer
// 	      | rank_specifier array_initializer)                       #objectCreationExpression
// 	| OPEN_PARENS argument ( ',' argument )+ CLOSE_PARENS           #tupleExpression
// 	| TYPEOF OPEN_PARENS (unbound_type_name | type_ | VOID) CLOSE_PARENS   #typeofExpression
// 	| CHECKED OPEN_PARENS expression CLOSE_PARENS                   #checkedExpression
// 	| UNCHECKED OPEN_PARENS expression CLOSE_PARENS                 #uncheckedExpression
// 	| DEFAULT (OPEN_PARENS type_ CLOSE_PARENS)?                     #defaultValueExpression
// 	| ASYNC? DELEGATE (OPEN_PARENS explicit_anonymous_function_parameter_list? CLOSE_PARENS)? block #anonymousMethodExpression
// 	| SIZEOF OPEN_PARENS type_ CLOSE_PARENS                          #sizeofExpression
// 	// C# 6: https://msdn.microsoft.com/en-us/library/dn986596.aspx
// 	| NAMEOF OPEN_PARENS (identifier '.')* identifier CLOSE_PARENS  #nameofExpression
// 	;

bracket_expression
    : LeftSquareBracket argument_list RightSquareBracket
    ;

array_creation_expression
    : Object_Creation_Keyword non_array_type LeftSquareBracket expression_list RightSquareBracket rank_specifier*
    | Object_Creation_Keyword array_type
    // | 'new' rank_specifier array_initializer
    ;

array_type
    : non_array_type rank_specifier+
    ;

rank_specifier
    : LeftSquareBracket COMMA* RightSquareBracket
    ;

non_array_type
    : value_type
    // | class_type
    | type_parameter
    ;


expression_list
    : expression
    | expression_list COMMA expression
    ;

parenthesized_expression
    : LeftParenthesis expression RightParenthesis
    ;

member_access
    : PERIOD identifier type_argument_list?
    ;

simple_name
    : identifier type_argument_list?
    ;

type_argument_list
    : LEFT_ANGLE_QUOTATION_MARK type_arguments RIGHT_ANGLE_QUOTATION_MARK
    ;

type_arguments
    : type_argument (COMMA type_argument)*
    ;   

type_argument
    : type
    ;

type
    : // reference_type
    | value_type
    | type_parameter
    ;

type_parameter
    : identifier
    ;

identifier
    : Simple_Identifier
    // | contextual_keyword
    ;

value_type
    : non_nullable_value_type
    // | nullable_value_type
    ;

non_nullable_value_type
    : struct_type
    // | enum_type
    ;

struct_type
    : type_name
    | simple_type
    ;

simple_type
    : numeric_type
    | Boolean
    ;

numeric_type
    : integral_type
    | floating_point_type
    ;

floating_point_type
    : Float32
    | Float64
    ;

integral_type
    : Int8
    | UInt8
    | Int16
    | UInt16
    | Int32
    | UInt32
    | Int64
    | UInt64
    ;

namespace_name
    : namespace_or_type_name
    ;

type_name
    : namespace_or_type_name
    ;

namespace_or_type_name
    : identifier type_argument_list?
    | namespace_or_type_name PERIOD identifier type_argument_list?
    ;

this_access
    : Object_Self_Keyword
    ;

argument_list
    : argument (COMMA argument)*
    ;

argument
    : argument_name? argument_value
    ;

argument_name
    : identifier COLON
    ;

argument_value
    : expression
    // | 'ref' variable_reference
    // | 'out' variable_reference
    ;
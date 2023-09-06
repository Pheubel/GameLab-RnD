parser grammar NovelerParser;
options { tokenVocab = NovelerLexer; }

story
    : import_statement* story_segment+
    ;

// TODO: assess if this is the way i want to add other files of code
imported_file
    : import_statement* imported_content+
    ;

// TODO: set up import stuff, but disallow global story
imported_content
    : PLUS
    ;

import_statement
    : EMBED_COMMAND IMPORT String_Literal New_Line
    ;

story_segment
    : embed_statement
    | text_segment
    | empty_segment
    ;

// TODO: find a better way to do this
text_segment
    : continued_text_line* text_line
    ;

continued_text_line
    : text_line_segment+ PIPE New_Line
    ;

text_line
    : text_line_segment+ New_Line
    ;

text_line_segment
    : text_line_valid_tokens
    | escaped_text_segment_character
    | Simple_Identifier
    | Text_Segment_Legal_Character
    | interpolated_value
    ;

escaped_text_segment_character
    : Escaped_Text_Segment_Character
    ;

interpolated_value
    : OPEN_CURLY expression CLOSE_CURLY
    ;

empty_segment
    : New_Line
    ;

embed_statement
    : embedded_variable_declaration
    | embedded_code_block
    | embedded_if_statement
    | choice_block
    ;

embedded_variable_declaration
    : EMBED_COMMAND variable_declare_assign New_Line
    | EMBED_COMMAND variable_declare New_Line
    ;

embedded_if_statement
    : EMBED_COMMAND if_statement_if_segment OPEN_CURLY story_segment* CLOSE_CURLY #embedded_if
    //| embedded_if_statement New_Line+ EMBED_COMMAND ELSE OPEN_CURLY story_segment* CLOSE_CURLY #embedded_if_else
    | embedded_if_statement New_Line+ (embedded_else_if_statement New_Line+)* EMBED_COMMAND ELSE OPEN_CURLY story_segment* CLOSE_CURLY New_Line #embedded_if_else
    ;

if_statement_if_segment
    : IF OPEN_BRACKET booleanExpression CLOSE_BRACKET
    ;

embedded_else_if_statement
    : EMBED_COMMAND ELSE if_statement_if_segment OPEN_CURLY story_segment* CLOSE_CURLY
    ;

choice_block
    : EMBED_COMMAND CHOICE New_Line* OPEN_CURLY New_Line* choice_block_choice+ CLOSE_CURLY
    ;

// TODO: add conditional and subtractable choice
choice_block_choice
    : standard_choice
    | default_choice
    ;

standard_choice
    : text_line_segment+ COLON New_Line choice_response
    ;

default_choice
    : ASTERISK text_line_segment+ COLON New_Line choice_response
    ;

choice_response
    : story_segment+
    ;

embedded_code_block
    : EMBED_COMMAND CODE code_block New_Line
    | EMBED_COMMAND code_block New_Line
    ;

code_block
    : New_Line* OPEN_CURLY  statement* CLOSE_CURLY
    ;

statement
    : empty_statement #statement_empty
    | variable_declaration #statement_variable_declaration
    | method_declaration #statement_method_declaration
    | structure_declaration #statement_structure_declaration
    | return_statement #statement_return
    | (expression SEMI_COLON)* expression empty_statement #statement_expression
    ;

return_statement
    : RETURN expression? SEMI_COLON?
    ;

empty_statement
    : New_Line
    | SEMI_COLON
    ;

identifier
    : Simple_Identifier
    ;

variable_declaration
    : variable_declare
    | variable_declare_assign
    ;

variable_declare
    : Simple_Identifier COLON type
    ;

variable_declare_assign
    : variable_declare EQUALS expression
    ;

variable_initializer
    : expression
    ;

structure_declaration
    : structure_header structure_body
    ;

structure_header
    : STRUCTURE identifier
    ;

structure_body
    : structure_block
    ;

structure_block
    : New_Line* OPEN_CURLY structure_member_declarations? CLOSE_CURLY
    ;

structure_member_declarations
    : structure_member_declaration+
    ;

structure_member_declaration
    : empty_statement
    | variable_declaration
    | method_declaration
    | constructor_declaration
    ;

constructor_declaration
    : identifier OPEN_BRACKET parameter_list? CLOSE_BRACKET code_block
    ;

method_declaration
    : method_header method_body
    ;

method_header
    : FUNCTION identifier OPEN_BRACKET parameter_list? CLOSE_BRACKET COLON return_type
    ;

parameter_list
    : parameter (COMMA parameter)*
    ;

parameter
    : identifier COLON type
    ;

method_body
    : code_block
    ;

return_type
    : type
    | NOTHING
    ;

booleanExpression
    : expression
    ;

expression
    : non_assignment_expression
    | assignment
    ;

non_assignment_expression
    : conditional_expression
    ;

conditional_expression
    : null_coalescing_expression
    | null_coalescing_expression QUESTION_MARK expression COLON expression
    ;

null_coalescing_expression
    : conditional_or_expression
    ;

conditional_or_expression
    : conditional_and_expression
    | conditional_or_expression CONDITIONAL_OR conditional_and_expression
    ;

conditional_and_expression
    : inclusive_or_expression
    | conditional_and_expression CONDITIONAL_AND inclusive_or_expression
    ;

inclusive_or_expression
    : exclusive_or_expression
    | inclusive_or_expression PIPE exclusive_or_expression
    ;

exclusive_or_expression
    : and_expression
    | exclusive_or_expression BITWISE_XOR and_expression
    ;

and_expression
    : equality_expression
    | and_expression BITWISE_AND equality_expression
    ;

equality_expression
    : relational_expression
    | equality_expression Equal_To relational_expression
    | equality_expression Not_Equal_To relational_expression
    ;

relational_expression
    : shift_expression
    | relational_expression LESS_THAN shift_expression
    | relational_expression GREATER_THAN shift_expression
    | relational_expression Less_Than_Or_Equal_To shift_expression
    | relational_expression Greater_Than_Or_Equal_To shift_expression
    // | relational_expression 'is' type
    // | relational_expression 'as' type
    ;

shift_expression
    : additive_expression
    | shift_expression LEFT_SHIFT additive_expression
    | shift_expression RIGHT_SHIFT additive_expression
    ;

additive_expression
    : multiplicative_expression
    | additive_expression PLUS multiplicative_expression
    | additive_expression MINUS multiplicative_expression
    ;

multiplicative_expression
    : unary_expression
    | multiplicative_expression ASTERISK unary_expression
    | multiplicative_expression SLASH unary_expression
    | multiplicative_expression REMAINDER unary_expression
    ;

assignment
    : unary_expression EQUALS expression
    ;

unary_expression
    : cast_expression
    | primary_expression
    | PLUS unary_expression
    | MINUS unary_expression
    | CONDITIONAL_NOT unary_expression
    | BITWISE_NOT unary_expression
    | pre_increment_expression
    | pre_decrement_expression
    ;

pre_increment_expression
    : INCREMENT unary_expression
    ;

pre_decrement_expression
    : DECREMENT unary_expression
    ;

post_increment_expression
    : primary_expression INCREMENT
    ;

post_decrement_expression
    : primary_expression DECREMENT
    ;

cast_expression
    : OPEN_BRACKET type CLOSE_BRACKET unary_expression
    ;

primary_expression
    : literal
    | primary_expression_start bracket_expression* ((member_access | method_invocation | INCREMENT | DECREMENT) bracket_expression* )*
    ;

primary_expression_start
    : simple_name
    | parenthesized_expression
    // | member_access
    | this_access
    | object_creation_expression
    ;

object_creation_expression
    : type OPEN_BRACKET argument_list? CLOSE_BRACKET
    ;

type
    : // reference_type
    | value_type
    | type_parameter
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
    | BOOLEAN
    ;

numeric_type
    : integer_type
    | floating_point_type
    ;

type_parameter
    : identifier
    ;

this_access
    : THIS
    ;

method_invocation
    : OPEN_BRACKET argument_list? CLOSE_BRACKET
    ;

parenthesized_expression
    : OPEN_BRACKET expression CLOSE_BRACKET
    ;

member_access
    : PERIOD identifier /*type_argument_list? // TODO: generics? */
    ;

simple_name
    : identifier /*type_argument_list? // TODO: generics? */
    ;

bracket_expression
    : OPEN_SQUARE_BRACKET argument_list CLOSE_SQUARE_BRACKET
    ;

argument_list
    : argument (COMMA argument)*
    ;

argument
    //: argument_name? argument_value
    : argument_value
    ;

// argument_name
//     : identifier COLON
//     ;

argument_value
    : expression
    ;

type_specifier
    : integer_type
    | floating_point_type
    | boolean_type
    | Simple_Identifier
    ;

integer_type
    : UNSIGNED? (TINY | SMALL | BIG)? WHOLE NUMBER
    ;

floating_point_type
    : BIG? NUMBER
    ;

boolean_type
    : BOOLEAN
    ;

// TODO: enable string literals when ready
literal
    : booleanLiteral
    | Decimal_Integer_Literal
    | Hexadecimal_Integer_Literal
    | Binary_Integer_Literal
    | Real_Literal
    //| String_Literal
    ;

booleanLiteral
    : TRUE
    | FALSE
    ;

namespace_name
    : namespace_or_type_name
    ;

type_name
    : namespace_or_type_name
    ;

namespace_or_type_name
    : identifier /* type_argument_list? // generics? */
    | namespace_or_type_name PERIOD identifier /* type_argument_list? // generics? */
    ;


// TODO: keep this up to date
text_line_valid_tokens
    : keywords
    | literal
    | Less_Than
    | Greater_Than
    | Less_Than_Or_Equal_To
    | Greater_Than_Or_Equal_To
    | SEMI_COLON
    | EQUALS
    | PLUS
    | MINUS
    | OPEN_BRACKET
    | CLOSE_BRACKET
    | CONDITIONAL_NOT
    | BITWISE_NOT
    | INCREMENT
    | DECREMENT
    | OPEN_SQUARE_BRACKET
    | CLOSE_SQUARE_BRACKET
    | COMMA
    | PERIOD
    | QUESTION_MARK
    | CONDITIONAL_OR
    | CONDITIONAL_AND
    | BITWISE_XOR
    | BITWISE_AND
    | REMAINDER
    ;

// TODO: keep up to date
keywords
    : IF
    | ELSE
    | THIS
    | UNSIGNED
    | TINY
    | SMALL
    | BIG
    | NUMBER
    | BOOLEAN
    | TRUE
    | FALSE
    | IMPORT
    | CODE
    | CHOICE
    | NOTHING
    | FUNCTION
    | RETURN
    | EXTERN
    | STRUCTURE
    ;
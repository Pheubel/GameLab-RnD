parser grammar NovelerParser;
options { tokenVocab = NovelerLexer; }

story
    : import_statement* story_segment+
    ;

import_statement
    : EMBED_COMMAND IMPORT String_Literal New_Line
    ;

story_segment
    : embed_statement
    | text_line
    | empty_segment
    ;

// TODO: find a better way to do this
text_line
    : (text_line_segment+ PIPE New_Line)* text_line_segment+ New_Line
    ;

text_line_segment
    : text_line_valid_tokens
    | Simple_Identifier
    | Text_Segment_Legal_Character
    | String_Literal
    | interpolated_value
    ;

interpolated_value
    : OPEN_CURLY Simple_Identifier CLOSE_CURLY
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
    : EMBED_COMMAND if_statement_if_segment OPEN_CURLY story_segment* CLOSE_CURLY
    | embedded_if_statement New_Line+ (EMBED_COMMAND if_statement_else_segment OPEN_CURLY story_segment* CLOSE_CURLY)+
    ;

if_statement_if_segment
    : IF OPEN_BRACKET    CLOSE_BRACKET
    ;

if_statement_else_segment
    : ELSE
    | ELSE if_statement_if_segment
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
    : OPEN_CURLY statement* CLOSE_CURLY
    ;

// TODO: add more statement rules
statement
    : empty_statement
    ;

empty_statement
    : New_Line
    | SEMI_COLON
    ;

variable_declare
    : Simple_Identifier COLON type_specifier
    ;

variable_declare_assign
    : variable_declare EQUALS // TODO value here
    ;

type_specifier
    : integer_specifier
    | float_specifier
    | boolean_specifier
    | Simple_Identifier
    ;

integer_specifier
    : UNSIGNED? (TINY | SMALL | BIG)? WHOLE NUMBER
    ;

float_specifier
    : BIG? NUMBER
    ;

boolean_specifier
    : BOOLEAN
    ;


// TODO: keep this up to date
text_line_valid_tokens
    : keywords
    | SEMI_COLON
    | EQUALS
    | PLUS
    | MINUS
    | OPEN_BRACKET
    | CLOSE_BRACKET
    ;

// TODO: keep up to date
keywords
    : IF
    | ELSE
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
    ;
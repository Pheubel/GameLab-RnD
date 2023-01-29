parser grammar StoryGrammar;
options { tokenVocab=StoryLexerGrammar; }

story
    : import_statements? story_parts?
    ;

story_parts
    : story_part+
    ;

end_of_file
    : EOF
    ;

story_part
    : embedded_statement
    | text
    ;

code_block
    : CODE 
    ;

text
    : Story_String_Start Story_Line? story_line_termination
    ;

variable_declaration
    : identifier Embedded_Type_Declarer identifier
    ;

embedded_statement
    : BEGIN_STATEMENT variable_declaration Exit_Statement
    | BEGIN_STATEMENT Exit_Statement
    | BEGIN_STATEMENT Start_Embedded_If expression
    ;

embedded_code_block
    :Start_Embedded_Code_Block Embed_Block (statement Exit_Statement)* EXIT_CODE_BLOCK
    ;

statement
    : variable_declaration
    ;

expression
    :
    ;

import_statements
    : import_statement+
    ;

import_statement
    : BEGIN_STATEMENT IMPORT string_literal end_statement?
    ;

end_statement
    : Exit_Statement
    | end_of_file
    ;

story_line_termination
    : End_Story_String
    | end_of_file
    ;

string_literal
    : (Embedded_Open_String_Literal | Code_Open_String_Literal) String_Literal_Content? Close_String_Literal
    ;

identifier
    : internal_identier
    | Code_Identifier
    | Embedded_Identifier
    ;

internal_identier
    : boolean_identifier
    | integer_identifier
    | float_identifier
    ;

boolean_identifier
    : Embedded_Boolean
    | Code_Boolean
    ;

integer_identifier
    : unsigned_keyword? (tiny_keyword | small_keyword | big_keyword)? whole_keyword number_keyword
    ;

float_identifier
    : big_keyword? number_keyword
    ;



// keywords

unsigned_keyword
    : Embedded_Unsigned
    | Code_Unsigned
    ;

tiny_keyword
    : Embedded_Tiny
    | Code_Tiny
    ;

small_keyword
    : Embedded_Small
    | Code_Small
    ;

big_keyword
    : Embedded_Big
    | Code_Big
    ;

whole_keyword
    : Embedded_Whole
    | Code_Whole
    ;

number_keyword
    : Embedded_Number
    | Code_Number
    ;

if_keyword
    : Embedded_If
    | Code_If
    ;

else_keword
    : Embedded_Else
    | Code_Else
    ;
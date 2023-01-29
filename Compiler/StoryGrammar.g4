parser grammar StoryGrammar;
options { tokenVocab=StoryLexerGrammar; }

story
    : import_statement* story_part*
    ;

end_of_file
    : EOF
    ;

story_part
    : embedded_statement
    | text
    ;

text
    : Story_String_Start Story_Line? story_line_termination
    ;

embedded_variable_declaration
    : BEGIN_STATEMENT identifier Embedded_Type_Declarer identifier
    ;

embedded_statement
    : embedded_variable_declaration
    ;

import_statement
    : BEGIN_STATEMENT IMPORT string_literal Exit_Statement
    ;

story_line_termination
    : End_Story_String
    | end_of_file
    ;

string_literal
    : (Embedded_Open_String_Literal | Code_Open_String_Literal) string_literal_content? Close_String_Literal
    ;

string_literal_content
    : String_Literal_Character+
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
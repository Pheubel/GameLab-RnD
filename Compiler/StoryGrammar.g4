parser grammar StoryGrammar;
options { tokenVocab=StoryLexerGrammar; }

story
    : import_statement* story_part*
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
    : Embedded_Identifier
    | Code_Identifier
    ;

end_of_file
    : EOF
    ;








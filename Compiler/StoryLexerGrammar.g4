lexer grammar StoryLexerGrammar;

BEGIN_STATEMENT: '@' -> pushMode(Embedded_Statement);
ASTERISK: '*';
SLASH: '/';

fragment TYPE_DECLARER: ':' ;
fragment STATEMENT_TERMINATOR_CHARACTER: ';';

fragment ASSIGN: '=';
fragment ADD : '+';
fragment SUBTRACT: '-';
fragment MULTIPLY: '*';
fragment DIVIDE: '/';
fragment REMAINDER: '%';

fragment BITWISE_NOT: '~';
fragment BITWISE_OR: '|';
fragment BITWISE_XOR: '^';
fragment BITWISE_AND: '&';
fragment LEFT_SHIFT: '<<';
fragment RIGHT_SHIFT: '>>';

fragment ASSIGN_ADD: ASSIGN ADD;
fragment ASSIGN_SUBTRACT: ASSIGN SUBTRACT;
fragment ASSIGN_MULTIPLY: ASSIGN MULTIPLY;
fragment ASSIGN_DIVIDE: ASSIGN DIVIDE;
fragment ASSIGN_REMAINDER: ASSIGN REMAINDER;

fragment ASSIGN_BITWISE_OR: ASSIGN BITWISE_OR;
fragment ASSIGN_BITWISE_XOR: ASSIGN BITWISE_XOR;
fragment ASSIGN_BITWISE_AND: ASSIGN BITWISE_AND;
fragment ASSIGN_LEFT_SHIFT: ASSIGN LEFT_SHIFT;
fragment ASSIGN_RIGHT_SHIFT: ASSIGN RIGHT_SHIFT;

fragment CONDITIONAL_AND: BITWISE_AND BITWISE_AND;
fragment CONDITIONAL_OR: BITWISE_OR BITWISE_OR;
fragment CONDITIONAL_NOT: '!';

fragment EQUALS_TO: ASSIGN ASSIGN;
fragment NOT_EQUALS_TO: CONDITIONAL_NOT ASSIGN;

fragment LESS_THAN: '<';
fragment GREATER_THAN: '>';
fragment LESS_THAN_OR_EQUAL_TO: LESS_THAN ASSIGN;
fragment GREATER_THAN_OR_EQUAL_TO: GREATER_THAN ASSIGN;

fragment INCREMENT: ADD ADD;
fragment DECREMENT: SUBTRACT SUBTRACT;

fragment BOOLEAN: 'boolean';
fragment UNSIGNED: 'unsigned';
fragment TINY: 'tiny';
fragment SMALL: 'small';
fragment BIG: 'big';
fragment WHOLE: 'whole';
fragment NUMBER: 'number';

fragment Letter_Character
    // Category Letter, all subcategories; category Number, subcategory letter.
    : [\p{L}\p{Nl}]
    // Only escapes for categories L & Nl allowed.
    | Unicode_Escape_Sequence
    ;

fragment Decimal_Digit_Character
    // Category Number, subcategory decimal digit.
    : [\p{Nd}]
    // Only escapes for category Nd allowed.
    | Unicode_Escape_Sequence
    ;

fragment Unicode_Escape_Sequence
    : '\\u' Hex_Digit Hex_Digit Hex_Digit Hex_Digit
    | '\\U' Hex_Digit Hex_Digit Hex_Digit Hex_Digit
            Hex_Digit Hex_Digit Hex_Digit Hex_Digit
    ;

fragment Hex_Digit
    : '0'..'9' | 'A'..'F' | 'a'..'f'
    ;

fragment Identifier_Start
    : Letter_Character
    ;

fragment Identifier_Part
    : Letter_Character
    | Decimal_Digit_Character
    ;

fragment Identifier_Part_Character
    : Letter_Character
    | Decimal_Digit_Character
    | Connecting_Character
    | Combining_Character
    | Formatting_Character
    ;

fragment Skipable_Whitepace
    :[ \t]
    ;

fragment Combining_Character
    // Category Mark, subcategories non-spacing and spacing combining.
    : [\p{Mn}\p{Mc}]
    // Only escapes for categories Mn & Mc allowed.
    | Unicode_Escape_Sequence
    ;

fragment Comment
    : Single_Line_Comment
    | Delimited_Comment_Section
    ;

fragment Single_Line_Comment
    : '//' Input_Character*
    ;

fragment Delimited_Comment
    : '/*' Delimited_Comment_Section* ASTERISK+ '/'
    ;

fragment Delimited_Comment_Section
    : SLASH
    | ASTERISK* Not_Slash_Or_Asterisk
    ;

fragment Not_Slash_Or_Asterisk
    : ~('/' | '*')    // Any except SLASH or ASTERISK
    ;

fragment Input_Character
    // anything but New_Line_Character
    : ~('\u000D' | '\u000A'   | '\u0085' | '\u2028' | '\u2029')
    ;

fragment New_Line_Character
    : '\u000D'  // carriage return
    | '\u000A'  // line feed
    | '\u0085'  // next line
    | '\u2028'  // line separator
    | '\u2029'  // paragraph separator
    ;

fragment Underscore_Character
    : '_'           // underscore
    | '\\u005' [fF] // Unicode_Escape_Sequence for underscore
    ;

fragment Connecting_Character
    // Category Punctuation, subcategory connector.
    : [\p{Pc}]
    // Only escapes for category Pc allowed.
    | Unicode_Escape_Sequence
    ;

fragment Formatting_Character
    // Category Other, subcategory format.
    : [\p{Cf}]
    // Only escapes for category Cf allowed.
    | Unicode_Escape_Sequence
    ;

Story_String_Start
    : Letter_Character -> pushMode(Story)
    ;

Default_WS
    : Skipable_Whitepace+ -> skip
    ;

Default_Comment
    : Comment -> skip
    ;

mode Story;

PIPE: '|';

Continue_String
    : PIPE '\n'
    ;

Story_Line
    : Letter_Character+
    ;

End_Story_String
    : New_Line_Character -> popMode
    ;



mode Embedded_Statement;

Embedded_Type_Declarer: TYPE_DECLARER;

Embedded_Assign: ASSIGN;
Embedded_Add: ADD;
Embedded_Subtract: SUBTRACT;
Embedded_Multiply: MULTIPLY;
Embedded_Divide: DIVIDE;
Embedded_Remainder: REMAINDER;

Embedded_Bitwise_Not: BITWISE_NOT;
Embedded_Bitwise_Or: BITWISE_OR;
Embedded_Bitwise_Xor: BITWISE_XOR;
Embedded_Bitwise_And: BITWISE_AND;
Embedded_Left_Shift: LEFT_SHIFT;
Embedded_Right_Shift: RIGHT_SHIFT;

Embedded_Assign_Add: ASSIGN_ADD;
Embedded_Assign_Subtract: ASSIGN_SUBTRACT;
Embedded_Assign_Multiply: ASSIGN_MULTIPLY;
Embedded_Assign_Divide: ASSIGN_DIVIDE;
Embedded_Assign_Remainder: ASSIGN_REMAINDER;

Embedded_Assign_Bitwise_Or: ASSIGN_BITWISE_OR;
Embedded_Assign_Bitwise_Xor: ASSIGN_BITWISE_XOR;
Embedded_Assign_Bitwise_And: ASSIGN_BITWISE_AND;
Embedded_Assign_Left_Shift: ASSIGN_LEFT_SHIFT;
Embedded_Assign_Right_Shift: ASSIGN_RIGHT_SHIFT;

Embedded_Conditional_And: CONDITIONAL_AND;
Embedded_Conditional_Or: CONDITIONAL_OR;
Embedded_Conditional_Not: CONDITIONAL_NOT;

Embedded_Equals_To: EQUALS_TO;
Embedded_Not_Equals_To: NOT_EQUALS_TO;

Embedded_Less_Than: LESS_THAN;
Embedded_Greater_than: GREATER_THAN;
Embedded_Less_Than_or_Equal_To: LESS_THAN_OR_EQUAL_TO;
Embedded_Greater_Than_Or_Equal_To: GREATER_THAN_OR_EQUAL_TO;

Embedded_Increment: INCREMENT;
Embedded_Decrement: DECREMENT;

Embedded_Boolean: BOOLEAN;
Embedded_Unsigned: UNSIGNED;
Embedded_Tiny: TINY;
Embedded_Small: SMALL;
Embedded_Big: BIG;
Embedded_Whole: WHOLE;
Embedded_Number: NUMBER;

IMPORT: 'import';
CODE: 'code';
ENTER_BLOCK: '{';



Embedded_WS
    : Skipable_Whitepace+ -> skip
    ;

Embedded_Comment
    : Single_Line_Comment ->skip
    ;

Start_Embedded_Code_Block
    : CODE Embedded_WS* ENTER_BLOCK -> pushMode(In_Code_Block)
    ;

Embedded_Identifier
    : Identifier_Start Identifier_Part*
    ;

Embedded_Open_String_Literal
    : '"' -> pushMode(In_String_Literal)
    ;

Exit_Statement
    : New_Line_Character -> popMode
    ;



mode In_String_Literal;

fragment Escaped_String_Literal_Character
    : String_Literal_Escape_Character '\\'
    | String_Literal_Escape_Character '"'
    ;

fragment String_Literal_Escape_Character
    : '\\'
    ;

String_Literal_Character
    : ~('\u000D' | '\u000A'   | '\u0085' | '\u2028' | '\u2029' | '\\' | '"')
    | Escaped_String_Literal_Character
    ;

Close_String_Literal
    : '"' -> popMode
    ;



mode In_Code_Block;

Code_Type_Declarer: TYPE_DECLARER;
Code_Statement_Terminator_Character: STATEMENT_TERMINATOR_CHARACTER;

Code_Add: ADD;
Code_Subtract: SUBTRACT;
Code_Multiply: MULTIPLY;
Code_Divide: DIVIDE;
Code_Remainder: REMAINDER;

Code_Bitwise_Not: BITWISE_NOT;
Code_Bitwise_Or: BITWISE_OR;
Code_Bitwise_Xor: BITWISE_XOR;
Code_Bitwise_And: BITWISE_AND;
Code_Left_Shift: LEFT_SHIFT;
Code_Right_Shift: RIGHT_SHIFT;

Code_Assign_Add: ASSIGN_ADD;
Code_Assign_Subtract: ASSIGN_SUBTRACT;
Code_Assign_Multiply: ASSIGN_MULTIPLY;
Code_Assign_Divide: ASSIGN_DIVIDE;
Code_Assign_Remainder: ASSIGN_REMAINDER;

Code_Assign_Bitwise_Or: ASSIGN_BITWISE_OR;
Code_Assign_Bitwise_Xor: ASSIGN_BITWISE_XOR;
Code_Assign_Bitwise_And: ASSIGN_BITWISE_AND;
Code_Assign_Left_Shift: ASSIGN_LEFT_SHIFT;
Code_Assign_Right_Shift: ASSIGN_RIGHT_SHIFT;

Code_Conditional_And: CONDITIONAL_AND;
Code_Conditional_Or: CONDITIONAL_OR;
Code_Conditional_Not: CONDITIONAL_NOT;

Code_Equals_To: EQUALS_TO;
Code_Not_Equals_To: NOT_EQUALS_TO;

Code_Less_Than: LESS_THAN;
Code_Greater_than: GREATER_THAN;
Code_Less_Than_or_Equal_To: LESS_THAN_OR_EQUAL_TO;
Code_Greater_Than_Or_Equal_To: GREATER_THAN_OR_EQUAL_TO;

Code_Increment: INCREMENT;
Code_Decrement: DECREMENT;

Code_Boolean: BOOLEAN;
Code_Unsigned: UNSIGNED;
Code_Tiny: TINY;
Code_Small: SMALL;
Code_Big: BIG;
Code_Whole: WHOLE;
Code_Number: NUMBER;

EXIT_CODE_BLOCK: '}' -> popMode;

Code_WS
    : Skipable_Whitepace+ -> skip
    ;

Code_New_Line
    : New_Line_Character
    ;

Code_Identifier
    : Identifier_Start Identifier_Part*
    ;

Code_Open_String_Literal
    : '"' -> pushMode(In_String_Literal)
    ;


lexer grammar NovelerLexer;

channels {
    WHITESPACE_CHANNEL,
    COMMENT_CHANNEL
}

ASTERISK: '*' ;
SLASH: '/' ;
EMBED_COMMAND: '@';
EQUALS: '=';
ESCAPE_CHARACTER: '\\';
PERIOD: '.';
QUESTION_MARK: '?';
REMAINDER: '%';

// keywords
THIS: 'this';
IMPORT: 'import';
CODE: 'code';
CHOICE: 'choice';

FUNCTION: 'function';
RETURN: 'return';

UNSIGNED: 'unsigned';
TINY: 'tiny';
SMALL: 'small';
BIG: 'big';
WHOLE: 'whole';
NUMBER: 'number';
BOOLEAN: 'boolean';

NOTHING: 'nothing';

EXTERN: 'extern';

TRUE: 'true';
FALSE: 'false';

IF: 'if';
ELSE: 'else';

OPEN_CURLY: '{';
CLOSE_CURLY: '}';

OPEN_BRACKET: '(';
CLOSE_BRACKET: ')';

OPEN_SQUARE_BRACKET: '[';
CLOSE_SQUARE_BRACKET: ']';

COLON: ':';
PIPE: '|';
SEMI_COLON: ';';
COMMA: ',';

PLUS: '+';
MINUS: '-';
CONDITIONAL_NOT: '!';
CONDITIONAL_OR: '||';
CONDITIONAL_AND: '&&';
BITWISE_NOT: '~';
// BITWISE_OR: '|';
BITWISE_XOR: '^';
BITWISE_AND: '&';

Equal_To: EQUALS EQUALS;
Not_Equal_To: CONDITIONAL_NOT EQUALS;

INCREMENT: '++';
DECREMENT: '--';
LEFT_SHIFT: '<<';
RIGHT_SHIFT: '>>';

fragment Comment
    : Single_Line_Comment
    | Delimited_Comment
    ;

fragment Single_Line_Comment
    : '//' Input_Character*
    ;

fragment Delimited_Comment
    : '/*' Delimited_Comment_Section* ASTERISK+ '/'
    ;

fragment Not_Slash_Or_Asterisk
    : ~('/' | '*')    // Any except SLASH or ASTERISK
    ;

fragment Input_Character
    // anything but New_Line_Character
    : ~('\u000D' | '\u000A'   | '\u0085' | '\u2028' | '\u2029')
    ;

fragment Delimited_Comment_Section
    : SLASH
    | ASTERISK* Not_Slash_Or_Asterisk
    ;

fragment Whitespace
    : [\p{Zs}]  // any character with Unicode class Zs
    | '\u0009'  // horizontal tab
    | '\u000B'  // vertical tab
    | '\u000C'  // form feed
    ;

Skipped_Whitespace: Whitespace -> channel (WHITESPACE_CHANNEL);

Skipped_Comment: Comment -> channel(COMMENT_CHANNEL);

Escaped_Whitespace: ESCAPE_CHARACTER Whitespace;

New_Line
    : New_Line_Character
    | '\u000D\u000A'    // carriage return, line feed
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

Simple_Identifier
    : Available_Identifier
    ;

fragment Available_Identifier
    : Basic_Identifier
    ;

fragment Basic_Identifier
    : Identifier_Start_Character Identifier_Part_Character*
    ;

fragment Identifier_Start_Character
    : Letter_Character
    | Underscore_Character
    ;

fragment Identifier_Part_Character
    : Letter_Character
    | Decimal_Digit_Character
    | Connecting_Character
    | Combining_Character
    | Formatting_Character
    ;

fragment Unicode_Escape_Sequence
    : '\\u' Hex_Digit Hex_Digit Hex_Digit Hex_Digit
    | '\\U' Hex_Digit Hex_Digit Hex_Digit Hex_Digit
            Hex_Digit Hex_Digit Hex_Digit Hex_Digit
    ;

fragment Decorated_Hex_Digit
    : '_'* Hex_Digit
    ;

fragment Hex_Digit
    : '0'..'9' | 'A'..'F' | 'a'..'f'
    ;

fragment Letter_Character
    // Category Letter, all subcategories; category Number, subcategory letter.
    : [\p{L}\p{Nl}]
    // Only escapes for categories L & Nl allowed. See note below.
    | Unicode_Escape_Sequence
    ;

fragment Combining_Character
    // Category Mark, subcategories non-spacing and spacing combining.
    : [\p{Mn}\p{Mc}]
    // Only escapes for categories Mn & Mc allowed. See note below.
    | Unicode_Escape_Sequence
    ;

fragment Decimal_Digit_Character
    // Category Number, subcategory decimal digit.
    : [\p{Nd}]
    // Only escapes for category Nd allowed. See note below.
    | Unicode_Escape_Sequence
    ;

fragment Connecting_Character
    // Category Punctuation, subcategory connector.
    : [\p{Pc}]
    // Only escapes for category Pc allowed. See note below.
    | Unicode_Escape_Sequence
    ;

fragment Formatting_Character
    // Category Other, subcategory format.
    : [\p{Cf}]
    // Only escapes for category Cf allowed, see note below.
    | Unicode_Escape_Sequence
    ;

String_Literal
    : '"' Input_Character* '"'
    ;

Escaped_Text_Segment_Character
    : '\\@'
    | '{{'
    | '}}'
    | '\\|'
    | '\\\\'
    | '\\'COLON
    | '\\/'
    ;

// Integer_Literal
//     : Decimal_Integer_Literal
//     | Hexadecimal_Integer_Literal
//     | Binary_Integer_Literal
//     ;

Decimal_Integer_Literal
    : Decimal_Digit Decorated_Decimal_Digit* // Integer_Type_Suffix?
    ;

fragment Decorated_Decimal_Digit
    : '_'* Decimal_Digit
    ;

fragment Decimal_Digit
    : '0'..'9'
    ;

fragment Integer_Type_Suffix
    : 'U' | 'u' | 'L' | 'l' |
      'UL' | 'Ul' | 'uL' | 'ul' | 'LU' | 'Lu' | 'lU' | 'lu'
    ;

Hexadecimal_Integer_Literal
    : ('0x' | '0X') Decorated_Hex_Digit+ // Integer_Type_Suffix?
    ;

Binary_Integer_Literal
    : ('0b' | '0B') Decorated_Binary_Digit+ // Integer_Type_Suffix?
    ;

fragment Decorated_Binary_Digit
    : '_'* Binary_Digit
    ;

fragment Binary_Digit
    : '0' | '1'
    ;

Real_Literal
    : Decimal_Digit Decorated_Decimal_Digit* '.'
      Decimal_Digit Decorated_Decimal_Digit* Exponent_Part? //Real_Type_Suffix?
    | '.' Decimal_Digit Decorated_Decimal_Digit* Exponent_Part? //Real_Type_Suffix?
    | Decimal_Digit Decorated_Decimal_Digit* Exponent_Part //Real_Type_Suffix?
    | Decimal_Digit Decorated_Decimal_Digit* //Real_Type_Suffix
    ;

fragment Exponent_Part
    : ('e' | 'E') Sign? Decimal_Digit Decorated_Decimal_Digit*
    ;

fragment Sign
    : '+' | '-'
    ;

fragment Real_Type_Suffix
    : 'F' | 'f' | 'D' | 'd'
    ;

LESS_THAN: '<';
GREATER_THAN: '>';

Less_Than
    : LESS_THAN
    ;

Greater_Than
    : GREATER_THAN
    ;

Less_Than_Or_Equal_To
    : LESS_THAN EQUALS
    ;

Greater_Than_Or_Equal_To
    : GREATER_THAN EQUALS
    ;

Text_Segment_Legal_Character
    : ~('\u000D' | '\u000A'   | '\u0085' | '\u2028' | '\u2029' | '@' | '|' | '{' | '}'| '\\' | ':' | '/' | '0'..'9')
    ;
lexer grammar NovelerLexer;

channels {
    WHITESPACE_CHANNEL,
    COMMENT_CHANNEL
}

ASTERISK: '*' ;
SLASH: '/' ;
EMBED_COMMAND: '@';
ESCAPE_CHARACTER: '\\';

// keywords
IMPORT: 'import';
CODE: 'code';
CHOICE: 'choice';

UNSIGNED: 'unsigned';
TINY: 'tiny';
SMALL: 'small';
BIG: 'big';
WHOLE: 'whole';
NUMBER: 'number';
BOOLEAN: 'boolean';

TRUE: 'true';
FALSE: 'false';

IF: 'if';
ELSE: 'else';

OPEN_CURLY: '{';
CLOSE_CURLY: '}';

COLON: ':';
PIPE: '|';
SEMI_COLON: ';';

PLUS: '+';
MINUS: '-';

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



// NEW!
Text_Segment_Legal_Character
    : ~('\u000D' | '\u000A'   | '\u0085' | '\u2028' | '\u2029' | '@' | '|' | '{' | '}'| '\\' | ':' | '/')
    | Escaped_Text_Segment_Character
    ;

fragment Escaped_Text_Segment_Character
    : '\\@'
    | '{{'
    | '}}'
    | '\\|'
    | '\\\\'
    | '\\:'
    | '\\/'
    ;
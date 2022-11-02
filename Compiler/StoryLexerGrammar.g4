lexer grammar StoryLexerGrammar;

channels{
    COMMENTS_CHANNEL
}

TRUE     : 'true' ;
FALSE    : 'false' ;
ASTERISK : '*' ;
SLASH    : '/' ;
SCOPE_OPEN : '{';
SCOPE_CLOSE : '}';

StoryEmbedSymbol : '@';


ImportStatementBegin
    : StoryEmbedSymbol 'import' -> pushMode(In_Embedded_Statement)
    ;

StoryIfStatementBegin
    : StoryEmbedSymbol 'if' -> pushMode(In_Embedded_If_Statement)
    ;

StoryElseStatement
    : StoryEmbedSymbol 'else'
    ;

LineTerminator
    : New_Line
    | EOF
    ;

New_Line
    : New_Line_Character
    | '\u000D\u000A'    // carriage return, line feed 
    ;

OpenEmbeddedVariableOutsideStory
    : '{' -> pushMode(Inside_Story), pushMode(Inside_Story_Variable)
    ;

StartSentencePart
    : Sentence_Starter_Character+ -> pushMode(Inside_Story)
    ;

Identifier
    : Identifier_Start_Character Identifier_Part_Character*
    ;

Empty_Line
    : New_Line -> skip
    ;

Comment
    : Single_Line_Comment
    | Delimited_Comment_Section
    ;

fragment Single_Line_Comment
    : '//' Input_Character*
    ;

fragment New_Line_Character
    : '\u000D'  // carriage return
    | '\u000A'  // line feed
    | '\u0085'  // next line
    | '\u2028'  // line separator
    | '\u2029'  // paragraph separator
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

fragment Sentence_Starter_Character
    // anything but New_Line_Character or illegal sentence tokens
    : ~('\u000D' | '\u000A'   | '\u0085' | '\u2028' | '\u2029' | '|' | '@' | ':' | '{' | '}')
    ;

fragment Underscore_Character
    : '_'           // underscore
    | '\\u005' [fF] // Unicode_Escape_Sequence for underscore
    ;

fragment Letter_Character
    // Category Letter, all subcategories; category Number, subcategory letter.
    : [\p{L}\p{Nl}]
    // Only escapes for categories L & Nl allowed.
    | Unicode_Escape_Sequence
    ;

fragment Identifier_Start_Character
    : Letter_Character
    | Underscore_Character
    ;

fragment Decimal_Digit_Character
    // Category Number, subcategory decimal digit.
    : [\p{Nd}]
    // Only escapes for category Nd allowed.
    | Unicode_Escape_Sequence
    ;
    
fragment Identifier_Part_Character
    : Letter_Character
    | Decimal_Digit_Character
    | Connecting_Character
    | Combining_Character
    | Formatting_Character
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

fragment Combining_Character
    // Category Mark, subcategories non-spacing and spacing combining.
    : [\p{Mn}\p{Mc}]
    // Only escapes for categories Mn & Mc allowed.
    | Unicode_Escape_Sequence
    ;

fragment Unicode_Escape_Sequence
    : '\\u' Hex_Digit Hex_Digit Hex_Digit Hex_Digit
    | '\\U' Hex_Digit Hex_Digit Hex_Digit Hex_Digit
            Hex_Digit Hex_Digit Hex_Digit Hex_Digit
    ;



mode In_Embedded_If_Statement;

Embedded_If_Enter_Condition: LeftParenthesis -> pushMode(In_Embedded_Statement);
Embedded_If_Leave_Condition: RightParenthesis -> popMode;

Conditional_Variable: Identifier;



mode In_Embedded_Statement;

COLON: ':';
COMMA: ',';
QUESTION_MARK: '?';
EXCLAMATION_MARK: '!';
PERIOD: '.';
LEFT_ANGLE_QUOTATION_MARK: '<';
RIGHT_ANGLE_QUOTATION_MARK: '>';

PLUS: '+';
MINUS: '-';
REMAINDER: '%';
MULTIPLY: ASTERISK;
DIVIDE: SLASH;

BITWISE_NOT: '~';
BITWISE_OR: '|';
BITWISE_XOR: '^';
BITWISE_AND: '&';
LEFT_SHIFT: '<<';
RIGHT_SHIFT: '>>';

CONDITIONAL_AND: '&&';
CONDITIONAL_OR: '||';
CONDITIONAL_NOT: EXCLAMATION_MARK;

Assign: '=';
AssignAdd: '+=';
AssignSubtract: '-=';
AssignMultiply: '*=';
AssignDivide: '/=';

Greater_Than: RIGHT_ANGLE_QUOTATION_MARK;
Less_Than: LEFT_ANGLE_QUOTATION_MARK;
Greater_Than_Or_Equal_To: '>=';
Less_Than_Or_Equal_To: '<=';

Equal_To: '==';
Not_Equal_To: '!=';

LeftParenthesis: '(';
RightParenthesis: ')';
LeftCurlyBracket: '{';
RigthCurlyBracket: '}';
LeftSquareBracket: '[';
RightSquareBracket: ']';

Boolean: 'boolean';

Unsigned: 'unsigned';
Tiny: 'tiny';
Small: 'small';
Big: 'big';
Whole: 'whole';
Number: 'number';

Int8: Tiny Whole Number;
UInt8: Unsigned Whole Number;
Int16: Small Whole Number;
UInt16: Unsigned Small Whole Number;
Int32: Whole Number;
UInt32: Unsigned Whole Number;
Int64: Big Whole Number;
UInt64: Unsigned Big Whole Number;

Float32: Big;
Float64: Big Number;

If: 'if';
Else: 'else';
Return: 'return';

Object_Creation_Keyword: 'new';
Object_Self_Keyword: 'this';

Increment
    : '++'
    ;

Decrement
    : '--'
    ;

AssignmentOperator
    : Assign | AssignAdd | AssignSubtract | AssignMultiply | AssignDivide | '%=' | '&=' | '|=' | '^=' | '<<=' | '>>='
    ;

Embedded_Whitespace 
    : [ \t] + -> skip
    ;

Open_String_Literal
    : '"' -> pushMode(In_String_Literal)
    ;

EndEmbeddedStatement
    :LineTerminator -> popMode
    ;

Integer_Literal
    : Decimal_Integer_Literal
    | Hexadecimal_Integer_Literal
    ;

Real_Literal
    : Decimal_Digit Decorated_Decimal_Digit* '.'
      Decimal_Digit Decorated_Decimal_Digit* Exponent_Part? Real_Type_Suffix?
    | '.' Decimal_Digit Decorated_Decimal_Digit* Exponent_Part? Real_Type_Suffix?
    | Decimal_Digit Decorated_Decimal_Digit* Exponent_Part Real_Type_Suffix?
    | Decimal_Digit Decorated_Decimal_Digit* Real_Type_Suffix
    ;

Simple_Identifier
    : Available_Identifier
    // | Escaped_Identifier
    ;

fragment Available_Identifier
    // excluding keywords or contextual keywords, see note below
    : Basic_Identifier
    ;

fragment Basic_Identifier
    : Identifier_Start_Character Identifier_Part_Character*
    ;

fragment Decimal_Integer_Literal
    : Decimal_Digit Decorated_Decimal_Digit* Integer_Type_Suffix?
    ;

fragment Integer_Type_Suffix
    : 'U' | 'u' | 'L' | 'l' |
      'UL' | 'Ul' | 'uL' | 'ul' | 'LU' | 'Lu' | 'lU' | 'lu'
    ;

fragment Decorated_Decimal_Digit
    : '_'* Decimal_Digit
    ;

fragment Decimal_Digit
    : '0'..'9'
    ;

fragment Hexadecimal_Integer_Literal
    : ('0x' | '0X') Decorated_Hex_Digit+ Integer_Type_Suffix?
    ;

fragment Decorated_Hex_Digit
    : '_'* Hex_Digit
    ;

fragment Hex_Digit
    : '0'..'9' | 'A'..'F' | 'a'..'f'
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



mode In_String_Literal;

String_Literal_Character
    : ~('\u000D' | '\u000A'   | '\u0085' | '\u2028' | '\u2029' | '\\' | '"') 
    | Escaped_String_Literal_Character
    ;

Close_String_Literal 
    : '"' -> popMode
    ;

fragment Escaped_String_Literal_Character
    : String_Literal_Escape_Character '\\'
    | String_Literal_Escape_Character '"'
    ;

fragment String_Literal_Escape_Character
    : '\\'
    ;



mode Inside_Story;

Line_Glue: '|';

Story_Whitespace
    : [ \t]
    ;

Sentence_Part
    : Sentence_Character (Story_NewLine+ | Story_Line_End | Sentence_Character+)
    ;

Story_NewLine
    : [ \t]* Line_Glue -> skip
    ;

Story_Line_End
    : [ \t]* LineTerminator -> skip, popMode
    ;

OpenEmbeddedVariableInsideStory
    : '{' -> pushMode(Inside_Story_Variable)
    ;

fragment Sentence_Character
    // anything but New_Line_Character or illegal sentence tokens
    : ~('\u000D' | '\u000A'   | '\u0085' | '\u2028' | '\u2029' | '|' | '@' | ':' | '{' | '}')
    ;

mode Inside_Story_Variable;

// ensure that we end inside of the story context
CloseEmbeddedVariable
    : '}' -> popMode, pushMode(Inside_Story)
    ;

Sentence_Close_Embedded_Variable
    : CloseEmbeddedVariable (Story_NewLine+ | Story_Line_End)?
    ;
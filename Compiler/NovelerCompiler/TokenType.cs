namespace Noveler.Compiler
{
    public enum TokenType
    {
        InvalidToken,

        // literals
        Int8Literal,
        Uint8Literal,
        Int16Literal,
        Uint16Literal,
        Int32Literal,
        Uint32Literal,
        Int64Literal,
        Uint64Literal,
        FloatLiteral,
        DoubleLiteral,

        // operation
        /// <summary> +</summary>
        Add,
        /// <summary> -</summary>
        Subtract,
        /// <summary> *</summary>
        Multiply,
        /// <summary> /</summary>
        Divide,

        /// <summary> =</summary>
        Assign,
        /// <summary> +=</summary>
        AddAssign,
        /// <summary> -=</summary>
        SubtractAssign,
        /// <summary> *=</summary>
        MultiplyAssign,
        /// <summary> /=</summary>
        DivideAssign,

        /// <summary> ==</summary>
        EqualsTo,
        /// <summary> ++</summary>
        Increment,
        /// <summary> --</summary>
        Decrement,
        /// <summary> -(value)</summary>
        Negate,

        // function
        FunctionDeclaration,
        FunctionName,

        //evaluation
        LeftParenthesis,
        RightParenthesis,

        // defining
        /// <summary> :</summary>
        Colon,

        // separating
        /// <summary> ,</summary>
        Comma,

        // terminating
        /// <summary> ;</summary>
        SemiColon,

        // variables
        Identifier,

        // special
        EndOfLine,
        EndOfFile,
        UndeclaredVariable,

        // keywords
        KeywordNumber,
        KeywordBig,
        KeywordSmall,
        KeywordTiny,
        KeywordWhole,
        KeywordSigned,
        KeywordUnsigned,
        KeywordTrue,
        KeywordFalse,
        KeywordNothing,
        KeywordReturn,
        KeywordBoolean,
        KeywordFunction,
        KeywordIf,

        //unsorted
        UndefinedSymbol,
        NewLine,
        Symbol,
        Remainder,
        ModuloAssign,
        AndAssign,
        OrAssign,
        XOrAssign,
        LeftShiftAssign,
        RightShiftAssign,
        KeywordElse,
        LessThan,
        LeftShift,
        RightShift,
        GreaterThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        ExclamationMark,
        NotEqualsTo,
        ConditionalAnd,
        And,
        XOr,
        ConditionalOr,
        /// <summary> |</summary>
        Or,
        /// <summary> {</summary>
        LeftCurlyBracket,
        /// <summary> }</summary>
        RightCurlyBacket,
        /// <summary> .</summary>
        Period,
        EscapedWhiteSpace,
        EscapedBackslash,
        EscapedPipe,
        InvalidEscapedCharacter,
        EscapedNewLine,
        AtSign,
        EscapedAtSign,
        KeywordImport,
        RawText,
        MalformedStringLiteral,
        StringLiteral,
        KeywordCode,
        SingleLineComment,
        KeywordChoice,
        EscapedColon,
    }
}
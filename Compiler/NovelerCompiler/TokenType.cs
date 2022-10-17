namespace Noveler.Compiler
{
    public enum TokenType
    {
        InvalidToken,

        /*
        //types
        IntValue,
        LongValue,
        FloatValue,
        DoubleValue,
        */

        // literals
        IntLiteral,
        LongLiteral,
        FloatLiteral,
        DoubleLiteral,

        // operation
        Add,
        Subtract,
        Multiply,
        Divide,

        Assign,
        AddAssign,
        SubtractAssign,
        MultiplyAssign,
        DivideAssign,

        EqualsTo,
        Increment,
        Decrement,
        Negate,

        // function
        FunctionDeclaration,
        FunctionName,
        OpenFunction,
        CloseFunction,

        //evaluation
        LeftParenthesis,
        RightParenthesis,

        // defining
        Colon,

        // separating
        Comma,

        // terminating
        SemiColon,
        ClosingCurlyBracket,

        // variables
        Identifier,

        // special
        Root,
        EndOfLine,
        EndOfFile,

        // keywords
        KeywordNumber,


        //unsorted
        UndeclaredVariable,
        CustomType,
        UndefinedSymbol,
        KeywordBig,
        KeywordWhole,
        KeywordSmall,
        KeywordTiny,
        KeywordSigned,
        KeywordUnsigned,
        NewLine,
        Symbol,
        KeywordTrue,
        KeywordFalse,
        KeywordNothing,
        Remainder,
        ModuloAssign,
        AndAssign,
        OrAssign,
        XOrAssign,
        LeftShiftAssign,
        RightShiftAssign,
        KeywordIf,
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
        Or,
        LeftCurlyBacket,
        RightCurlyBacket,
        Return,
        KeywordBoolean,
        Period,
    }
}
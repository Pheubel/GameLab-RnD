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

        Compare,
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
    }
}
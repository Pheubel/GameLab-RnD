namespace Noveler.Compiler
{
    enum TokenType
    {
        InvalidToken,

        //types
        IntValue,
        LongValue,
        FloatValue,
        DoubleValue,

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
        AssignAdd,
        AssignSubtract,

        Compare,
        Increment,
        Decrement,

        // function
        FunctionDeclaration,
        FunctionName,
        OpenFunction,
        CloseFunction,

        //evaluation
        OpenEvaluationScope,
        CloseEvaluationScope,

        // closing
        SemiColon,
        ClosingCurlyBracket,

        // variables
        ValueVariable,

        // special
        Root,
        EndOfLine,
        EndOfFile,

        // keywords


        //unsorted
        Negate,
        MultiplyAssign,
        DivideAssign,
        UndeclaredVariable,
        CustomType,
    }
}
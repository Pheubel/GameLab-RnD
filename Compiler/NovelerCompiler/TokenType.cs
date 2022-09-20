namespace Noveler.Compiler
{
    enum TokenType
    {
        InvalidToken,

        //types
        Int,
        Long,
        Float,
        Double,

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

        //unsorted
    }
}
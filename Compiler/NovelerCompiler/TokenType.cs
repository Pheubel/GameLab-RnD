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
        Plus,
        Minus,
        Multiply,
        Divide,
        Assign,
        Compare,

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
        EndOfFile
    }
}
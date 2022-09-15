namespace Noveler.Compiler
{
    enum TokenType
    {
        InvalidToken,
        //types

        // literals
        IntLiteral,

        // operators
        Plus,
        Minus,

        // special
        EndOfLine,
        EndOfFile
    }
}
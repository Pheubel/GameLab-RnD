namespace Noveler.Compiler
{
    internal ref struct ReadingContext
    {
        public int LineNumber;
        public int CharacterOnLine;
        public string? CurrentFile;
        public int ScopeLevel;
        public Stack<Token> TokenStack;
        public Dictionary<string, VariableTableEntry> VariableTable;

        public ReadingContext(Dictionary<string, VariableTableEntry> table, int lineNumber, int characterOnLine, string? currentFile, int scopeLevel) : this()
        {
            VariableTable = table;
            LineNumber = lineNumber;
            CharacterOnLine = characterOnLine;
            CurrentFile = currentFile;
            ScopeLevel = scopeLevel;
            TokenStack = new Stack<Token>();
        }
    }
}
namespace Noveler.Compiler
{
    internal ref struct ReadingContext
    {
        public int LineNumber;
        public int CharacterOnLine;
        public string? CurrentFile;
        public int ScopeLevel;
        public Stack<TreeNode> NodeStack;
        public Dictionary<string, VariableTableEntry> VariableTable;
        public ReadState ReadState;

        public ReadingContext(Dictionary<string, VariableTableEntry> table, int lineNumber, int characterOnLine, string? currentFile, int scopeLevel) : this()
        {
            VariableTable = table;
            LineNumber = lineNumber;
            CharacterOnLine = characterOnLine;
            CurrentFile = currentFile;
            ScopeLevel = scopeLevel;
            NodeStack = new Stack<TreeNode>();
            ReadState = ReadState.Story;
        }

        public void AdvanceLine()
        {
            LineNumber++;
            CharacterOnLine = 1;
        }
    }
}
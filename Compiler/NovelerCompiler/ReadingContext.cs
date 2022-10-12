namespace Noveler.Compiler
{
    internal ref struct ReadingContext
    {
        public int LineNumber;
        public int CharacterOnLine;
        public ReadState ReadState;
        public int ScopeLevel;
        public bool IsInHealthyState;
        public string? CurrentFile;
        public Stack<TreeNode> NodeStack;
        public Dictionary<string, VariableTableEntry> VariableTable;
        public List<CompilerMessage> OutMessages;

        public ReadingContext(Dictionary<string, VariableTableEntry> table, int lineNumber, int characterOnLine, string? currentFile, int scopeLevel, List<CompilerMessage> outMessages) : this()
        {
            VariableTable = table;
            LineNumber = lineNumber;
            CharacterOnLine = characterOnLine;
            CurrentFile = currentFile;
            ScopeLevel = scopeLevel;
            NodeStack = new Stack<TreeNode>();
            ReadState = ReadState.Story;
            IsInHealthyState = true;
            OutMessages = outMessages;
        }

        public void AdvanceLine()
        {
            LineNumber++;
            CharacterOnLine = 1;
        }

        public void Recover() => 
            IsInHealthyState = true;

        public void AddErrorMessage(CompilerMessage message)
        {
            IsInHealthyState = false;
            OutMessages.Add(message);
        }
    }
}
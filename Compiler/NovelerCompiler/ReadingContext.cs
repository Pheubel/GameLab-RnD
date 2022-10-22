namespace Noveler.Compiler
{
    internal ref struct ReadingContext
    {
        public int LineNumber;
        public int CharacterOnLine;
        public bool IsInHealthyState;
        public string? CurrentFile;
        public Dictionary<string, SymbolTableEntry> SymbolTable;
        public List<CompilerMessage> OutMessages;

        public ReadingContext(Dictionary<string, SymbolTableEntry> symbolTable, int lineNumber, int characterOnLine, string? currentFile, List<CompilerMessage> outMessages) : this()
        {
            SymbolTable = symbolTable;
            LineNumber = lineNumber;
            CharacterOnLine = characterOnLine;
            CurrentFile = currentFile;
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

        public void AddErrorMessage(string message, CompilerMessage.MessageCode messageCode)
        {
            ref var context = ref this;

            IsInHealthyState = false;
            OutMessages.Add(new CompilerMessage(message, messageCode, ref context));
        }

        public void AddErrorMessage(string message, CompilerMessage.MessageCode messageCode, Token token)
        {
            IsInHealthyState = false;
            OutMessages.Add(new CompilerMessage(message, messageCode, token));
        }
    }


}
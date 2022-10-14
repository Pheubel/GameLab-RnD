namespace Noveler.Compiler
{
    internal sealed class SymbolTableEntry
    {
        private int _typeIdValue;
        public List<Token> Appearances { get; }
        public SymbolKind Kind { get; set; }

        public SymbolTableEntry(SymbolKind symbolKind = SymbolKind.Unknown)
        {
            Kind = symbolKind;
            Appearances = new List<Token>();
        }

        public int GetTypeId() => _typeIdValue;
        public void SetTypeId(int id) => _typeIdValue = id;
        public void SetTypeId(TypeId id) => _typeIdValue = (int)id;

    }

    enum SymbolKind
    {
        Unknown,
        Variable,
        Type,
        Function
    }
}
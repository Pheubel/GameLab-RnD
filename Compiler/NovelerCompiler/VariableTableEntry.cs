namespace Noveler.Compiler
{
    internal class VariableTableEntry
    {
        private int _typeIdValue;
        public List<Token> Appearances { get; }

        public VariableTableEntry()
        {
            Appearances = new List<Token>();
        }

        public int GetTypeId() => _typeIdValue;
        public void SetTypeId(int id) => _typeIdValue = id;
        public void SetTypeId(InternalType id) => _typeIdValue = (int)id;
    }
}
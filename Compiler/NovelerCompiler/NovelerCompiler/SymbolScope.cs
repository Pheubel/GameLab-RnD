using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Noveler.Compiler
{
    internal class SymbolScope
    {
        [MemberNotNullWhen(true, nameof(ParentScope))]
        public bool HasParent => ParentScope != null;

        public required SymbolScope? ParentScope { get; init; }

        public Dictionary<string, SymbolInfo> SymbolDictionary { get; } = new();

        public int GetDepth() => HasParent ? GetDepth() + 1 : 0;

        public bool TryLookUp(string symbolName, [MaybeNullWhen(false)] ref SymbolInfo symbol)
        {
            symbol = ref CollectionsMarshal.GetValueRefOrNullRef(SymbolDictionary, symbolName)!;
            if (symbol != null)
                return true;

            if (HasParent)
            {
                return ParentScope.TryLookUp(symbolName, ref symbol!);
            }
            else
            {
                return false;
            }
        }

        public bool TryInsert(string symbolName, SymbolInfo symbol)
        {
            //SymbolScope? scope = this.ParentScope;
            //while (scope != null)
            //{
            //    if (scope.SymbolDictionary.ContainsKey(symbolName))
            //        return false;

            //    scope = scope.ParentScope;
            //}

            // symbol name was not found in parent scopes.
            ref SymbolInfo? symbolEntry = ref CollectionsMarshal.GetValueRefOrAddDefault(SymbolDictionary, symbolName, out bool exists);

            if (exists)
                return false;

            symbolEntry = symbol;
            return true;
        }

        public SymbolScope CreateChildScope()
        {
            return new SymbolScope() { ParentScope = this };
        }
    }
}

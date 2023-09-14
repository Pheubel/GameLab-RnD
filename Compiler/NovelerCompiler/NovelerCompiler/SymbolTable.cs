using Noveler.Compiler.CodeDomainObjectModel.Expressions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.SymbolStore;

namespace Noveler.Compiler
{
    abstract record SymbolInfo(string Name);

    sealed record SymbolInfo<T>(string Name, T Info) : SymbolInfo(Name) where T : notnull;

    /// <summary>
    /// Class for keeping track of the symbols referenced in the current scope
    /// </summary>
    internal sealed class SymbolTable
    {
        private readonly Stack<SymbolScope> _symbolScopes;
        public SymbolTable()
        {
            _symbolScopes = new();
        }

        public int CurrentScopeLevel => _symbolScopes.Count - 1;
        public SymbolScope? CurrentScope => _symbolScopes.TryPeek(out var scope) ? scope : null;

        public SymbolScope EnterScope()
        {
            _symbolScopes.TryPeek(out SymbolScope? parentScope);
            var scope = new SymbolScope() { ParentScope = parentScope };
            _symbolScopes.Push(scope);
            return scope;
        }

        public bool ExitScope()
        {
            return _symbolScopes.TryPop(out _);
        }

        public bool TryInsert(string symbolName, SymbolInfo symbolInfo)
        {
            return _symbolScopes.TryPeek(out SymbolScope? scope) && scope.TryInsert(symbolName, symbolInfo);
        }

        public bool TryLookupSymbol(string symbolName, [NotNullWhen(true)] ref SymbolInfo? symbol)
        {
            symbol = default;
            return _symbolScopes.TryPeek(out SymbolScope? scope) && scope.TryLookUp(symbolName, ref symbol!);
        }
    }
}

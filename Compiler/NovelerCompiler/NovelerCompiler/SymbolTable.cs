using Noveler.Compiler.CodeDomainObjectModel.Expressions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.SymbolStore;

namespace Noveler.Compiler
{
    abstract class SymbolInfo
    {
        public required string Name { get; internal set; }
    }

    sealed class SymbolInfo<T> : SymbolInfo
        where T : notnull
    {
        public required T Info { get; internal set; }
    }

    /// <summary>
    /// Class for keeping track of the symbols referenced in the current scope
    /// </summary>
    internal sealed class SymbolTable
    {
        private readonly Stack<SymbolScope> _symbolScopes;
        public SymbolTable()
        {
            _symbolScopes = new();

            EnterScope();
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

        public void ExitScope()
        {
            _symbolScopes.Pop();
        }

        public bool TryInsert(string symbolName, SymbolInfo symbolInfo)
        {
            return _symbolScopes.TryPeek(out SymbolScope? scope) && scope.TryInsert(symbolName, symbolInfo);
        }

        public bool TryLookupSymbol(string symbolName, [NotNullWhen(true)] out SymbolInfo? symbol)
        {
            symbol = default;
            return _symbolScopes.TryPeek(out SymbolScope? scope) && scope.TryLookUp(symbolName, out symbol);
        }
    }
}

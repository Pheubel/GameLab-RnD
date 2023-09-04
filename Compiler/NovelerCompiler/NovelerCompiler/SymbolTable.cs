using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Noveler.Compiler
{
	//internal sealed class SymbolTable
	//{
	//	private readonly List<Dictionary<string, SymbolInfo>> _symbolScopes;

	//	public SymbolTable()
	//	{
	//		_symbolScopes = new();

	//		// create top level scope
	//		EnterScope();
	//	}

	//	public bool Insert(SymbolInfo symbolInfo)
	//	{
	//		return _symbolScopes[^1].TryAdd(symbolInfo.Name, symbolInfo);
	//	}

	//	public Optional<SymbolInfo> LookUp(string name)
	//	{
	//		for (int i = _symbolScopes.Count - 1; i >= 0; --i)
	//		{
	//			if (_symbolScopes[i].TryGetValue(name, out SymbolInfo? symbol))
	//			{
	//				return symbol;
	//			}
	//		}

	//		return default;
	//	}

	//	/// <summary>
	//	/// Returns the current scope of the symbol table.
	//	/// </summary>
	//	/// <returns> The 0 indexed level of the current scope, -1 if not in a scope.</returns>
	//	public int GetCurrentScope() =>
	//		_symbolScopes.Count - 1;

	//	public void EnterScope()
	//	{
	//		_symbolScopes.Add(new Dictionary<string, SymbolInfo>(StringComparer.Ordinal));
	//	}

	//	public void ExitScope()
	//	{
	//		_symbolScopes.RemoveAt(_symbolScopes.Count - 1);
	//	}
	//}
}

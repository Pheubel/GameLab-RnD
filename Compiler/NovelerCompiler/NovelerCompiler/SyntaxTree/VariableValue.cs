using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Noveler.Compiler.SyntaxTree
{
    internal class VariableValue : ISyntaxTreeNode
    {
        public VariableValue(VariableSymbolInfo symbol)
        {
            Symbol = symbol;
        }

        public bool IsLeaf => true;
        public VariableSymbolInfo Symbol { get; }
    }
}

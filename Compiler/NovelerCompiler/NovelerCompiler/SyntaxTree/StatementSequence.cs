﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noveler.Compiler.SyntaxTree
{
    internal class StatementSequence : ISyntaxTreeNode
    {
        public bool IsLeaf => throw new NotImplementedException();
    }
}

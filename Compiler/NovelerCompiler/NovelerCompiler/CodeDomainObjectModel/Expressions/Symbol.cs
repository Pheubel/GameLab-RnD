﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	internal sealed record Symbol(string SymbolName) : Expression
	{
		public override IReadOnlyList<DomainObject> GetChildren() => Array.Empty<DomainObject>();
	}
}

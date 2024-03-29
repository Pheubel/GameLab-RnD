﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noveler.Compiler.CodeDomainObjectModel
{
	abstract record DomainObject()
	{
		public abstract IReadOnlyList<DomainObject> GetChildren();
	};
}

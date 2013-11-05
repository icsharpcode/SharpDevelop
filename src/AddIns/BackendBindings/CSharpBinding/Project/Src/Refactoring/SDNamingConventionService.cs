// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace CSharpBinding.Refactoring
{
	sealed class SDNamingConventionService : NamingConventionService
	{
		public override IEnumerable<NamingRule> Rules {
			get {
				return Enumerable.Empty<NamingRule>();
			}
		}
	}
}

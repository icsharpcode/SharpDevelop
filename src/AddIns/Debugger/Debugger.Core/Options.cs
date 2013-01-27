// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System.Collections.Generic;

namespace Debugger
{
	public abstract class Options
	{
		public abstract bool EnableJustMyCode { get; set; }
		public abstract bool StepOverNoSymbols { get; set; }
		public abstract bool StepOverDebuggerAttributes { get; set; }
		public abstract bool StepOverAllProperties { get; set; }
		public abstract bool StepOverFieldAccessProperties { get; set; }
		public abstract IEnumerable<string> SymbolsSearchPaths { get; set; }
		public abstract bool PauseOnHandledExceptions { get; set; }
	}
}

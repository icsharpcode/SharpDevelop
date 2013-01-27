// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System.Collections.Generic;

namespace Debugger
{
	public class Options
	{
		public Options()
		{
			EnableJustMyCode = true;
			StepOverNoSymbols = true;
			StepOverDebuggerAttributes = true;
			StepOverAllProperties = false;
			StepOverFieldAccessProperties = true;
			SymbolsSearchPaths = new string[0];
			PauseOnHandledExceptions = false;
		}
		
		public virtual bool EnableJustMyCode { get; set; }
		public virtual bool StepOverNoSymbols { get; set; }
		public virtual bool StepOverDebuggerAttributes { get; set; }
		public virtual bool StepOverAllProperties { get; set; }
		public virtual bool StepOverFieldAccessProperties { get; set; }
		public virtual IEnumerable<string> SymbolsSearchPaths { get; set; }
		public virtual bool PauseOnHandledExceptions { get; set; }
	}
}

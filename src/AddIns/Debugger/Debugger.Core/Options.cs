// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System.Collections.Generic;

using System;

namespace Debugger
{
	public class Options
	{
		public virtual bool EnableJustMyCode { get; set; }
		public virtual bool EnableEditAndContinue { get; set; }
		public virtual bool SuppressJITOptimization { get; set; }
		public virtual bool SuppressNGENOptimization { get; set; }
		public virtual bool StepOverDebuggerAttributes { get; set; }
		public virtual bool StepOverAllProperties { get; set; }
		public virtual bool StepOverFieldAccessProperties { get; set; }
		public virtual IEnumerable<string> SymbolsSearchPaths { get; set; }
		public virtual bool PauseOnHandledExceptions { get; set; }
	}
}

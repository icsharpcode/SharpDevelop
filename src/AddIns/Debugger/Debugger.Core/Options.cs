using System;
// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

namespace Debugger
{
	[Serializable]
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
		
		public bool EnableJustMyCode { get; set; }
		public bool StepOverNoSymbols { get; set; } // Decompilation
		public bool StepOverDebuggerAttributes { get; set; }
		public bool StepOverAllProperties { get; set; }
		public bool StepOverFieldAccessProperties { get; set; }
		public string[] SymbolsSearchPaths { get; set; }
		public bool PauseOnHandledExceptions { get; set; }
	}
}

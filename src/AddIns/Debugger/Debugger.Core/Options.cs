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
			StepOverSingleLineProperties = false;
			StepOverFieldAccessProperties = true;
			Verbose = false;
			SymbolsSearchPaths = new string[0];
			SuspendOtherThreads = true;
			PauseOnHandledExceptions = false;
		}
		
		public bool EnableJustMyCode { get; set; }
		public bool StepOverNoSymbols { get; set; }
		public bool StepOverDebuggerAttributes { get; set; }
		public bool StepOverAllProperties { get; set; }
		public bool StepOverSingleLineProperties { get; set; }
		public bool StepOverFieldAccessProperties { get; set; }
		public bool Verbose { get; set; }
		public string[] SymbolsSearchPaths { get; set; }
		public bool SuspendOtherThreads { get; set; }
		public bool PauseOnHandledExceptions { get; set; }
		bool decompileCodeWithoutSymbols;
		
		public bool DecompileCodeWithoutSymbols {
			get { return decompileCodeWithoutSymbols; }
			set {
				decompileCodeWithoutSymbols = value;
				EnableJustMyCode = !decompileCodeWithoutSymbols;
				StepOverNoSymbols = !decompileCodeWithoutSymbols;
			}
		}
	}
}

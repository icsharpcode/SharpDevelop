// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;

namespace Debugger
{
	public class Options
	{
		bool enableJustMyCode = true;
		bool stepOverNoSymbols = true;
		bool stepOverDebuggerAttributes = true;
		bool stepOverAllProperties = false;
		bool stepOverSingleLineProperties = false;
		bool stepOverFieldAccessProperties = true;
		bool verbose = false;
		string[] symbolsSearchPaths = new string[0];
		
		public bool EnableJustMyCode {
			get { return enableJustMyCode; }
			set { enableJustMyCode = value; }
		}
		
		public bool StepOverNoSymbols {
			get { return stepOverNoSymbols; }
			set { stepOverNoSymbols = value; }
		}
		
		public bool StepOverDebuggerAttributes {
			get { return stepOverDebuggerAttributes; }
			set { stepOverDebuggerAttributes = value; }
		}
		
		public bool StepOverAllProperties {
			get { return stepOverAllProperties; }
			set { stepOverAllProperties = value; }
		}
		
		public bool StepOverSingleLineProperties {
			get { return stepOverSingleLineProperties; }
			set { stepOverSingleLineProperties = value; }
		}
		
		public bool StepOverFieldAccessProperties {
			get { return stepOverFieldAccessProperties; }
			set { stepOverFieldAccessProperties = value; }
		}
		
		public bool Verbose {
			get { return verbose; }
			set { verbose = value; }
		}
		
		public string[] SymbolsSearchPaths {
			get { return symbolsSearchPaths; }
			set { symbolsSearchPaths = value; }
		}
	}
}

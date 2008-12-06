// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger
{
	public class Options
	{
		public bool EnableJustMyCode = true;
		public bool StepOverNoSymbols = true;
		public bool StepOverDebuggerAttributes = true;
		public bool StepOverAllProperties = false;
		public bool StepOverSingleLineProperties = false;
		public bool StepOverFieldAccessProperties = true;
		public bool Verbose = false;
		public string[] SymbolsSearchPaths = new string[0];
	}
}

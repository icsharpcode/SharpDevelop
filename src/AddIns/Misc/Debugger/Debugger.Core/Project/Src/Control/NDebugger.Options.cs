// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger
{
	public partial class NDebugger
	{
		bool justMyCodeEnabled;
		bool obeyDebuggerAttributes;
		string[] symbolsSearchPaths;
		
		public bool JustMyCodeEnabled {
			get { return justMyCodeEnabled; }
			// Affects steppers during their creation so there is nothing to update
			set { justMyCodeEnabled = value; }
		}
		
		public bool ObeyDebuggerAttributes {
			get { return obeyDebuggerAttributes; }
			set {
				if (obeyDebuggerAttributes != value) {
					obeyDebuggerAttributes = value;
					foreach(Process process in this.Processes) {
						foreach(Module module in process.Modules) {
							// Rechceck the module for attributes
							module.SetJustMyCodeStatus(module.HasSymbols, obeyDebuggerAttributes);
						}
					}
				}
			}
		}
		
		public string[] SymbolsSearchPaths {
			get { return symbolsSearchPaths; }
			set {
				if (symbolsSearchPaths != value) {
					symbolsSearchPaths = value;
					foreach(Process process in this.Processes) {
						foreach(Module module in process.Modules) {
							// Try to load the symbols
							module.LoadSymbols(symbolsSearchPaths);
						}
					}
				}
			}
		}
	}
}

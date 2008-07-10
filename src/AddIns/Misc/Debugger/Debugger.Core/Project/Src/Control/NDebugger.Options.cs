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
		bool justMyCodeEnabled = true;
		bool obeyDebuggerAttributes = true;
		bool skipProperties = true;
		bool skipOnlySingleLineProperties = true;
		bool verbose = false;
		string[] symbolsSearchPaths;
		
		void ResetJustMyCodeInModules()
		{
			foreach(Process process in this.Processes) {
				foreach(Module module in process.Modules) {
					module.SetJustMyCodeStatus();
				}
			}
		}
		
		public bool JustMyCodeEnabled {
			get { return justMyCodeEnabled; }
			set {
				if (justMyCodeEnabled != value) {
					justMyCodeEnabled = value;
					ResetJustMyCodeInModules();
				}
			}
		}
		
		public bool ObeyDebuggerAttributes {
			get { return obeyDebuggerAttributes; }
			set {
				if (obeyDebuggerAttributes != value) {
					obeyDebuggerAttributes = value;
					ResetJustMyCodeInModules();
				}
			}
		}
		
		public bool SkipProperties {
			get { return skipProperties; }
			set {
				if (skipProperties != value) {
					skipProperties = value;
					ResetJustMyCodeInModules();
				}
			}
		}
		
		public bool SkipOnlySingleLineProperties {
			get { return skipOnlySingleLineProperties; }
			set {
				if (skipOnlySingleLineProperties != value) {
					skipOnlySingleLineProperties = value;
					ResetJustMyCodeInModules();
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
		
		public bool Verbose {
			get { return verbose; }
			set { verbose = value; }
		}
	}
}

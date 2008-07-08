// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using Debugger;
using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.SharpDevelop.Services
{
	public static class DebuggingOptions
	{
		public static Properties DebuggingProperties {
			get {
				return PropertyService.Get("Debugging", new Properties());
			}
		}
		
		public static bool ICorDebugVisualizerEnabled {
			get { return DebuggingProperties.Get("ICorDebugVisualizerEnabled", false); }
			set { DebuggingProperties.Set("ICorDebugVisualizerEnabled", value); }
		}
		
		public static bool ShowValuesInHexadecimal {
			get { return DebuggingProperties.Get("ShowValuesInHexadecimal", false); }
			set { DebuggingProperties.Set("ShowValuesInHexadecimal", value); }
		}
		
		public static bool ShowArgumentNames {
			get { return DebuggingProperties.Get("ShowArgumentNames", true); }
			set { DebuggingProperties.Set("ShowArgumentNames", value); }
		}
		
		public static bool ShowArgumentValues {
			get { return DebuggingProperties.Get("ShowArgumentValues", true); }
			set { DebuggingProperties.Set("ShowArgumentValues", value); }
		}
		
		public static bool ShowExternalMethods {
			get { return DebuggingProperties.Get("ShowExternalMethods", false); }
			set { DebuggingProperties.Set("ShowExternalMethods", value); }
		}
		
		public static bool JustMyCodeEnabled {
			get { return DebuggingProperties.Get("JustMyCodeEnabled", true); }
			set { DebuggingProperties.Set("JustMyCodeEnabled", value); }
		}
		
		public static bool ObeyDebuggerAttributes {
			get { return DebuggingProperties.Get("ObeyDebuggerAttributes", true); }
			set { DebuggingProperties.Set("ObeyDebuggerAttributes", value); }
		}
		
		public static string[] SymbolsSearchPaths {
			get { return DebuggingProperties.Get("SymbolsSearchPaths", new string[0]); }
			set { DebuggingProperties.Set("SymbolsSearchPaths", value); }
		}
		
		public static void ApplyToCurrentDebugger()
		{
			WindowsDebugger winDbg = DebuggerService.CurrentDebugger as WindowsDebugger;
			if (winDbg != null && winDbg.DebuggerCore != null) {
				NDebugger debugger = winDbg.DebuggerCore;
				
				debugger.JustMyCodeEnabled = JustMyCodeEnabled;
				debugger.ObeyDebuggerAttributes = ObeyDebuggerAttributes;
				debugger.SymbolsSearchPaths = SymbolsSearchPaths;
			}
		}
	}
}

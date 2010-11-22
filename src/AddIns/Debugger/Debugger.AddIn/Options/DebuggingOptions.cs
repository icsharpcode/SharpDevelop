// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Drawing;
using System.Windows.Forms;
using Debugger;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Services
{
	public enum ShowIntegersAs { Auto, Decimal, Hexadecimal };
	
	public class DebuggingOptions: Options
	{
		public static DebuggingOptions Instance {
			get {
				return PropertyService.Get("DebuggingOptions", new DebuggingOptions());
			}
		}
		
		public bool ICorDebugVisualizerEnabled;
		public ShowIntegersAs ShowIntegersAs = ShowIntegersAs.Auto;
		public bool ShowArgumentNames;
		public bool ShowArgumentValues;
		public bool ShowExternalMethods;
		public bool ShowLineNumbers;
		public bool ShowModuleNames;
		
		// Properties for the DebuggerExceptionForm
		public FormWindowState DebuggerEventWindowState = FormWindowState.Normal;
		
		// Properties for the DebuggeeExceptionForm
		public FormWindowState DebuggeeExceptionWindowState = FormWindowState.Normal;
	}
}

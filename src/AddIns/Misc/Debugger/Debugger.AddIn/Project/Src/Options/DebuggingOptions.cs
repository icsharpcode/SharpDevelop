// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System.Drawing;
using System.Windows.Forms;
using Debugger;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Services
{
	public class DebuggingOptions: Options
	{
		public static DebuggingOptions Instance {
			get {
				return PropertyService.Get("DebuggingOptions", new DebuggingOptions());
			}
		}
		
		public bool ICorDebugVisualizerEnabled;
		public bool ShowValuesInHexadecimal;
		public bool ShowArgumentNames;
		public bool ShowArgumentValues;
		public bool ShowExternalMethods;
		
		// Properties for the DebuggerExceptionForm
		public FormWindowState DebuggerEventWindowState = FormWindowState.Normal;
		
		// Properties for the DebuggeeExceptionForm
		public FormWindowState DebuggeeExceptionWindowState = FormWindowState.Normal;
	}
}

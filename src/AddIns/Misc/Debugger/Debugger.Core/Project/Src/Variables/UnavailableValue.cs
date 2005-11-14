// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.InteropServices;

using Debugger.Interop.CorDebug;

namespace Debugger
{
	public class UnavailableValue: Value
	{
		string message;
		
		public override string AsString {
			get {
				return message; 
			} 
		}
		
		public override string Type { 
			get {
				return String.Empty;
			} 
		}
		
		internal UnavailableValue(NDebugger debugger): this(debugger, "Value is not available")
		{
			
		}
		
		internal UnavailableValue(NDebugger debugger, string message):base(debugger, null)
		{
			this.message = message;
		}

		public override bool MayHaveSubVariables {
			get {
				return false;
			}
		}
	}
}

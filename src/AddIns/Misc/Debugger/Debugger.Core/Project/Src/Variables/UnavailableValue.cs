// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.InteropServices;

using Debugger.Wrappers.CorDebug;

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
		
		internal UnavailableValue(NDebugger debugger, PersistentValue pValue, string message):base(debugger, pValue)
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

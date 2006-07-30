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
		
		internal UnavailableValue(Variable variable, string message):base(variable)
		{
			this.message = message;
		}

		protected override bool GetMayHaveSubVariables()
		{
			return false;
		}
	}
}

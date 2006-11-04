// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger
{
	public class UnavailableValue: ValueProxy
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
		
		internal UnavailableValue(Value @value, string message):base(@value)
		{
			this.message = message;
		}

		protected override bool GetMayHaveSubVariables()
		{
			return false;
		}
	}
}

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger 
{	
	[Serializable]
	public class ValueEventArgs : ProcessEventArgs
	{
		ValueProxy val;
		
		public ValueProxy ValueProxy {
			get {
				return val;
			}
		}
		
		public ValueEventArgs(ValueProxy val): base(val.Process)
		{
			this.val = val;
		}
	}
}

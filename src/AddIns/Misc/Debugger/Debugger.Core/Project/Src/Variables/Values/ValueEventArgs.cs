// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger 
{	
	/// <summary>
	/// Provides data for events related to <see cref="Debugger.Value"/> class
	/// </summary>
	[Serializable]
	public class ValueEventArgs : ProcessEventArgs
	{
		Value val;
		
		/// <summary> The value that caused the event </summary>
		public Value Value {
			get {
				return val;
			}
		}
		
		/// <summary> Initializes a new instance of the class </summary>
		public ValueEventArgs(Value val): base(val.Process)
		{
			this.val = val;
		}
	}
}

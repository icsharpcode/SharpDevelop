// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Runtime.InteropServices;

using DebuggerInterop.Core;

namespace DebuggerLibrary
{
	public class NullRefVariable: Variable
	{
		public override object Value { 
			get {
				return "<null reference>"; 
			} 
		}
		
		public override string Type
		{
			get
			{
				switch (corType)
				{
					case CorElementType.SZARRAY:
					case CorElementType.ARRAY: return typeof(System.Array).ToString();
					case CorElementType.OBJECT: return typeof(System.Object).ToString();
					case CorElementType.STRING: return typeof(System.String).ToString();
					case CorElementType.CLASS: return "<class>";
					default: return string.Empty;
				}
			}
		}

		internal unsafe NullRefVariable(ICorDebugValue corValue, string name):base(corValue, name)
		{
			
		}
	}
}
			

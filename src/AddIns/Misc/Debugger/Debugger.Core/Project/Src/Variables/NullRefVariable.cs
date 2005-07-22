// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
//     <version>$Revision$</version>
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
				switch (CorType)
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

		internal unsafe NullRefVariable(NDebugger debugger, ICorDebugValue corValue, string name):base(debugger, corValue, name)
		{
			
		}
	}
}

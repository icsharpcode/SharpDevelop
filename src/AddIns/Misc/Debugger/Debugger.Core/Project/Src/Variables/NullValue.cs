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
	public class NullValue: Value
	{
		public override string AsString { 
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

		internal unsafe NullValue(NDebugger debugger, ICorDebugValue corValue):base(debugger, corValue)
		{
			
		}

		public override bool MayHaveSubVariables {
			get {
				return false;
			}
		}
	}
}

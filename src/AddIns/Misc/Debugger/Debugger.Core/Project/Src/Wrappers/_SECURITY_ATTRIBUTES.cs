// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 1591

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public static class _SECURITY_ATTRIBUTES
	{
		public static Debugger.Interop.CorDebug._SECURITY_ATTRIBUTES Default;
		
		static unsafe _SECURITY_ATTRIBUTES() {
			Default = new Debugger.Interop.CorDebug._SECURITY_ATTRIBUTES();
			Default.bInheritHandle = 0;
			Default.lpSecurityDescriptor = IntPtr.Zero;
			Default.nLength = (uint)sizeof(Debugger.Interop.CorDebug._SECURITY_ATTRIBUTES);
		}
	}
}

#pragma warning restore 1591

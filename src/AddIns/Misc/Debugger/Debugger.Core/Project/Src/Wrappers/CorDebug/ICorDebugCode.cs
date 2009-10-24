// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 1591

namespace Debugger.Interop.CorDebug
{
	using System;
	
	public static partial class CorDebugExtensionMethods
	{
		public static unsafe byte[] GetCode(this ICorDebugCode corCode)
		{
			if (corCode.IsIL() == 0) return null;
			byte[] code = new byte[corCode.GetSize()];
			fixed(void* pCode = code)
				corCode.GetCode(0, (uint)code.Length, (uint)code.Length, new IntPtr(pCode));
			return code;
		}
	}
}

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
	
	public partial class ICorDebugCode
	{
		public unsafe byte[] GetCode()
		{
			if (this.IsIL == 0) return null;
			byte[] code = new byte[this.Size];
			fixed(void* pCode = code) {
				this.GetCode(0, (uint)code.Length, (uint)code.Length, new IntPtr(pCode));
			}
			return code;
		}
	}
}

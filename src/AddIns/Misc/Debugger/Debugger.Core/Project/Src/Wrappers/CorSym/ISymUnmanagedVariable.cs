// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 1591

namespace Debugger.Interop.CorSym
{
	using System;

	public static partial class CorSymExtensionMethods
	{
		public static string GetName(this ISymUnmanagedVariable symVar)
		{
			return Util.GetString(symVar.GetName);
		}
		
		const int defaultSigSize = 8;
		
		public static unsafe byte[] GetSignature(this ISymUnmanagedVariable symVar)
		{
			byte[] sig = new byte[defaultSigSize];
			uint acualSize;
			fixed(byte* pSig = sig)
				symVar.GetSignature((uint)sig.Length, out acualSize, new IntPtr(pSig));
			Array.Resize(ref sig, (int)acualSize);
			if (acualSize > defaultSigSize)
				fixed(byte* pSig = sig)
					symVar.GetSignature((uint)sig.Length, out acualSize, new IntPtr(pSig));
			return sig;
		}
	}
}

#pragma warning restore 1591

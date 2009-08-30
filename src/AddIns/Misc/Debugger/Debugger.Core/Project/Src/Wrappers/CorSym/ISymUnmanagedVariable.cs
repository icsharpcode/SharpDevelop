// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 1591

namespace Debugger.Wrappers.CorSym
{
	using System;

	public partial class ISymUnmanagedVariable
	{
		public string Name {
			get {
				return Util.GetString(GetName);
			}
		}
		
		const int defaultSigSize = 8;
		
		public unsafe byte[] Signature {
			get {
				byte[] sig = new byte[defaultSigSize];
				uint acualSize;
				fixed(byte* pSig = sig)
					this.GetSignature((uint)sig.Length, out acualSize, new IntPtr(pSig));
				Array.Resize(ref sig, (int)acualSize);
				if (acualSize > defaultSigSize)
					fixed(byte* pSig = sig)
						this.GetSignature((uint)sig.Length, out acualSize, new IntPtr(pSig));
				return sig;
			}
		}
	}
}

#pragma warning restore 1591

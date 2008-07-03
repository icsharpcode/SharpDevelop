// <file>
//	 <copyright see="prj:///doc/copyright.txt"/>
//	 <license see="prj:///doc/license.txt"/>
//	 <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//	 <version>$Revision$</version>
// </file>

#pragma warning disable 108, 1591 

namespace Debugger.Interop.CorPub
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    
	[ComImport, Guid("A37FBD41-5A69-11D3-8F84-00A0C9B4D50C"), InterfaceType((short) 1)]
	public interface ICorPublishProcessEnum : ICorPublishEnum
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void Skip([In] uint celt);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void Reset();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void Clone([MarshalAs(UnmanagedType.Interface)] out ICorPublishEnum ppEnum);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void GetCount(out uint pcelt);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void Next([In] uint celt, [MarshalAs(UnmanagedType.Interface)] out ICorPublishProcess objects, out uint pceltFetched);
	}
}

#pragma warning restore 108, 1591
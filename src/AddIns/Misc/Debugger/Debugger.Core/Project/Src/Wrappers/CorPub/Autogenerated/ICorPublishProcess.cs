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
    using System.Text;
	
	[ComImport, Guid("18D87AF1-5A6A-11D3-8F84-00A0C9B4D50C"), InterfaceType((short) 1)]
	public interface ICorPublishProcess
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void IsManaged(out int pbManaged);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void EnumAppDomains([MarshalAs(UnmanagedType.Interface)] out ICorPublishAppDomainEnum ppEnum);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void GetProcessID(out uint pid);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void GetDisplayName([In] uint cchName, out uint pcchName, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder szName);
	}
}

#pragma warning restore 108, 1591
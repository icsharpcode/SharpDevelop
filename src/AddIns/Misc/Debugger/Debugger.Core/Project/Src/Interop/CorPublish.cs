// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 108, 1591 

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Debugger.Interop.CorPublish
{
	[ComImport, TypeLibType((short) 2), ClassInterface((short) 0), Guid("047A9A40-657E-11D3-8D5B-00104B35E7EF")]
	public class CorpubPublishClass : ICorPublish, CorpubPublish, ICorPublishProcess, ICorPublishAppDomain, ICorPublishProcessEnum, ICorPublishAppDomainEnum
	{		
	    // Methods
	    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
	    public virtual extern void Clone([MarshalAs(UnmanagedType.Interface)] out ICorPublishEnum ppEnum);
	    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
	    public virtual extern void EnumAppDomains([MarshalAs(UnmanagedType.Interface)] out ICorPublishAppDomainEnum ppEnum);
	    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
	    public virtual extern void EnumProcesses([In, ComAliasName("CorpubProcessLib.COR_PUB_ENUMPROCESS")] COR_PUB_ENUMPROCESS Type, [MarshalAs(UnmanagedType.Interface)] out ICorPublishProcessEnum ppIEnum);
	    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
	    public virtual extern void GetCount(out uint pcelt);
	    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
	    public virtual extern void GetDisplayName([In] uint cchName, out uint pcchName, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder szName);
	    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
	    public virtual extern void GetID(out uint puId);
	    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
	    public virtual extern void GetName([In] uint cchName, out uint pcchName, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder szName);
	    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
	    public virtual extern void GetProcess([In] uint pid, [MarshalAs(UnmanagedType.Interface)] out ICorPublishProcess ppProcess);
	    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
	    public virtual extern void GetProcessID(out uint pid);
	    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
	    public virtual extern void ICorPublishAppDomainEnum_Clone([MarshalAs(UnmanagedType.Interface)] out ICorPublishEnum ppEnum);
	    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
	    public virtual extern void ICorPublishAppDomainEnum_GetCount(out uint pcelt);
	    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
	    public virtual extern void ICorPublishAppDomainEnum_Reset();
	    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
	    public virtual extern void ICorPublishAppDomainEnum_Skip([In] uint celt);
	    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
	    public virtual extern void IsManaged(out int pbManaged);
	    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
	    public virtual extern void Next([In] uint celt, [MarshalAs(UnmanagedType.Interface)] out ICorPublishAppDomain objects, out uint pceltFetched);
	    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
	    public virtual extern void Next([In] uint celt, [MarshalAs(UnmanagedType.Interface)] out ICorPublishProcess objects, out uint pceltFetched);
	    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
	    public virtual extern void Reset();
	    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
	    public virtual extern void Skip([In] uint celt);
	}

	[ComImport, CoClass(typeof(CorpubPublishClass)), Guid("9613A0E7-5A68-11D3-8F84-00A0C9B4D50C")]
	public interface CorpubPublish : ICorPublish
	{
	}

	public enum COR_PUB_ENUMPROCESS
	{
		COR_PUB_MANAGEDONLY = 1
	}
 
	[ComImport, Guid("9613A0E7-5A68-11D3-8F84-00A0C9B4D50C"), InterfaceType((short) 1)]
	public interface ICorPublish
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void EnumProcesses([In, ComAliasName("CorpubProcessLib.COR_PUB_ENUMPROCESS")] COR_PUB_ENUMPROCESS Type, [MarshalAs(UnmanagedType.Interface)] out ICorPublishProcessEnum ppIEnum);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void GetProcess([In] uint pid, [MarshalAs(UnmanagedType.Interface)] out ICorPublishProcess ppProcess);
	}

	[ComImport, Guid("D6315C8F-5A6A-11D3-8F84-00A0C9B4D50C"), InterfaceType((short) 1)]
	public interface ICorPublishAppDomain
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void GetID(out uint puId);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void GetName([In] uint cchName, out uint pcchName, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder szName);
	}

	[ComImport, Guid("9F0C98F5-5A6A-11D3-8F84-00A0C9B4D50C"), InterfaceType((short) 1),]
	public interface ICorPublishAppDomainEnum : ICorPublishEnum
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
		void Next([In] uint celt, [MarshalAs(UnmanagedType.Interface)] out ICorPublishAppDomain objects, out uint pceltFetched);
	}

	[ComImport, Guid("C0B22967-5A69-11D3-8F84-00A0C9B4D50C"), InterfaceType((short) 1)]
	public interface ICorPublishEnum
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void Skip([In] uint celt);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void Reset();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void Clone([MarshalAs(UnmanagedType.Interface)] out ICorPublishEnum ppEnum);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void GetCount(out uint pcelt);
	}

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

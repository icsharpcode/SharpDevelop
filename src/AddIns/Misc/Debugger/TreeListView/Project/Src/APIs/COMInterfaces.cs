using System;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Runtime.InteropServices.APIs
{
	public class COMInterfaces
	{
		#region IShellFolder
		[ComImport,
			Guid("000214E6-0000-0000-C000-000000000046"),
			InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			public interface IShellFolder
		{
			void ParseDisplayName(
				IntPtr hwnd, 
				IntPtr pbc, 
				[MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName,
				out uint pchEaten, 
				out IntPtr ppidl, 
				ref uint pdwAttributes);

			[PreserveSig]
			int EnumObjects(IntPtr hWnd, APIsEnums.ShellFolderEnumObjectsTypes flags,  ref IEnumIDList enumList);

			void BindToObject(
				IntPtr pidl, 
				IntPtr pbc, 
				[In()] ref Guid riid, 
				out IShellFolder ppv);
			//[MarshalAs(UnmanagedType.Interface)] out object ppv);

			void BindToStorage(
				IntPtr pidl, 
				IntPtr pbc, 
				[In()] ref Guid riid, 
				[MarshalAs(UnmanagedType.Interface)] out object ppv);

			[PreserveSig()]    
			uint CompareIDs(
				int lParam, 
				IntPtr pidl1, 
				IntPtr pidl2);

			void CreateViewObject(
				IntPtr hwndOwner, 
				[In()] ref Guid riid, 
				[MarshalAs(UnmanagedType.Interface)] out object ppv);

			void GetAttributesOf(
				uint cidl, 
				[In(), MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl,
				ref APIsEnums.ShellFolderAttributes rgfInOut);

			[return: MarshalAs(UnmanagedType.Interface)]
			object GetUIObjectOf(
				IntPtr hwndOwner, 
				uint cidl, 
				[MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl,
				[In()] ref Guid riid, 
				IntPtr rgfReserved);

			void GetDisplayNameOf(
				IntPtr pidl, 
				APIsEnums.ShellFolderDisplayNames uFlags, 
				out APIsStructs.STRRET pName);

			IntPtr SetNameOf(
				IntPtr hwnd, 
				IntPtr pidl, 
				[MarshalAs(UnmanagedType.LPWStr)] string pszName, 
				APIsEnums.ShellFolderDisplayNames uFlags);
		}
		#endregion
		#region IEnumIDList
		[ComImport(),
			Guid("000214F2-0000-0000-C000-000000000046"),
			InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			public interface IEnumIDList
		{
			[PreserveSig()]
			uint Next(
				uint celt,
				[In(), Out(), MarshalAs(UnmanagedType.LPArray)] IntPtr[] rgelt,
				out uint pceltFetched);
        
			void Skip(
				uint celt);

			void Reset();

			IEnumIDList Clone();
		}
		#endregion
		#region IDataObject
		[ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), GuidAttribute("0000010e-0000-0000-C000-000000000046")]
			public interface IDataObject
		{
			[PreserveSig()]
			int GetData(ref APIsStructs.FORMATETC a, ref APIsStructs.STGMEDIUM b);
			[PreserveSig()]
			void GetDataHere(int a, ref APIsStructs.STGMEDIUM b);
			[PreserveSig()]
			int QueryGetData(int a);
			[PreserveSig()]
			int GetCanonicalFormatEtc(int a, ref int b);
			[PreserveSig()]
			int SetData(int a, int b, int c);
			[PreserveSig()]
			int EnumFormatEtc(uint a, ref Object b);
			[PreserveSig()]
			int DAdvise(int a, uint b, Object c, ref uint d);
			[PreserveSig()]
			int DUnadvise(uint a);
			[PreserveSig()]
			int EnumDAdvise(ref Object a);
		}
		#endregion
		#region IShellExtInit
		[ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), GuidAttribute("000214e8-0000-0000-c000-000000000046")]
			public interface IShellExtInit
		{
			[PreserveSig()]
			int	Initialize (IntPtr pidlFolder, IntPtr lpdobj, uint /*HKEY*/ hKeyProgID);
		}
		#endregion

		#region IContextMenu
		[ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), GuidAttribute("000214e4-0000-0000-c000-000000000046")]
			public interface IContextMenu
		{
			// IContextMenu methods
			[PreserveSig()]
			int		QueryContextMenu(uint hmenu, uint iMenu, int idCmdFirst, int idCmdLast, uint uFlags);
			[PreserveSig()]
			void	InvokeCommand (ref APIsStructs.CMINVOKECOMMANDINFO pici);
			[PreserveSig()]
			void	GetCommandString(
				int idcmd,
				APIsEnums.GetCommandStringInformations uflags,
				int reserved,
				StringBuilder commandstring,
				int cch);
		}
		#endregion
		#region IContextMenu2
		[ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), GuidAttribute("000214f4-0000-0000-c000-000000000046")]
			public interface IContextMenu2
		{
			// IContextMenu methods
			[PreserveSig()]
			int		QueryContextMenu(uint hmenu, uint iMenu, int idCmdFirst, int idCmdLast, uint uFlags);
			[PreserveSig()]
			void	InvokeCommand (ref APIsStructs.CMINVOKECOMMANDINFO pici);
			[PreserveSig()]
			void	GetCommandString(int idcmd,
				APIsEnums.GetCommandStringInformations uflags,
				int reserved,
				StringBuilder commandstring,
				int cch);
			// IContextMenu2 methods
			[PreserveSig()]
			uint HandleMenuMsg(uint uMsg,IntPtr wParam,IntPtr lParam);
		}
		#endregion
	}
}

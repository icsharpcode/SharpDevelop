using System;
using System.Runtime.InteropServices;
using System.IO;

namespace System.Runtime.InteropServices.APIs
{
	public class APIsShell
	{
		#region GetShellFolder function
		public static COMInterfaces.IShellFolder GetShellFolder(IntPtr handle, string path, out IntPtr FolderID)
		{
			string tempPath = path;

			// Get the desktop folder
			COMInterfaces.IShellFolder returnedShellFolder = null;
			APIsShell.SHGetDesktopFolder(out returnedShellFolder);

			System.Guid IID_IShellFolder = new System.Guid("000214E6-0000-0000-C000-000000000046");
			IntPtr Pidl = IntPtr.Zero;
			uint i,j=0;
			// Connect to "My Computer"
			APIsShell.SHGetSpecialFolderLocation(handle, APIsEnums.ShellSpecialFolders.DRIVES, out Pidl);
			COMInterfaces.IShellFolder tempfolder;
			returnedShellFolder.BindToObject(Pidl, IntPtr.Zero, ref IID_IShellFolder, out tempfolder);
			Marshal.ReleaseComObject(returnedShellFolder);
			returnedShellFolder = tempfolder;
			int lastindexposition = 0;
			while(lastindexposition != -1)
			{
				lastindexposition = tempPath.IndexOf('\\');
				if(lastindexposition == -1) break;
				string foldername = tempPath.Remove(lastindexposition, tempPath.Length - lastindexposition) + @"\";
				returnedShellFolder.ParseDisplayName(handle, IntPtr.Zero, foldername, out i, out Pidl, ref j);
				returnedShellFolder.BindToObject(Pidl, IntPtr.Zero, ref IID_IShellFolder, out tempfolder);
				Marshal.ReleaseComObject(returnedShellFolder);
				returnedShellFolder = tempfolder;
				tempPath = tempPath.Substring(++lastindexposition);
			}
			FolderID = Pidl;
			return(returnedShellFolder);
		}
		#endregion
		#region Copy, Delete, Move, Rename
		public static bool DoOperation(IntPtr Handle, string[] source, string dest, APIsEnums.FileOperations operation)
		{
			APIsStructs.SHFILEOPSTRUCT fileop = new APIsStructs.SHFILEOPSTRUCT();
			fileop.hwnd = Handle;
			fileop.lpszProgressTitle = Enum.GetName(typeof(APIsEnums.FileOperations), operation);
			fileop.wFunc = (uint) operation;
			fileop.pFrom = Marshal.StringToHGlobalUni(StringArrayToMultiString(source));
			fileop.pTo = Marshal.StringToHGlobalUni(dest);
			fileop.fAnyOperationsAborted = 0;
			fileop.hNameMappings = IntPtr.Zero;

			return SHFileOperation(ref fileop) == 0;
		}
		private static String StringArrayToMultiString(String[] stringArray)
		{
			String multiString = "";

			if (stringArray == null)
				return "";

			for (int i=0 ; i<stringArray.Length ; i++)
				multiString += stringArray[i] + '\0';
    
			multiString += '\0';
    
			return multiString;
		}
		#endregion
		#region Properties Launch
		public static int ViewFileProperties(string path)
		{
			if(!File.Exists(path)) return(-1);
			APIsStructs.SHELLEXECUTEINFO info = new APIsStructs.SHELLEXECUTEINFO();
			info.cbSize = Marshal.SizeOf(typeof(APIsStructs.SHELLEXECUTEINFO));
			info.fMask = APIsEnums.ShellExecuteFlags.INVOKEIDLIST;
			info.hWnd = IntPtr.Zero;
			info.lpVerb = "properties";
			info.lpFile = Path.GetFileName(path);
			info.lpParameters = "";
			info.lpDirectory = Path.GetDirectoryName(path);
			info.nShow = APIsEnums.ShowWindowStyles.SHOW;
			info.hInstApp = IntPtr.Zero;
			IntPtr ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(APIsStructs.SHELLEXECUTEINFO)));
			Marshal.StructureToPtr(info, ptr, false);
			return(ShellExecuteEx(ptr));
		}
		public static int ViewDirectoryProperties(string path)
		{
			if(!Directory.Exists(path)) return(-1);
			APIsStructs.SHELLEXECUTEINFO info = new APIsStructs.SHELLEXECUTEINFO();
			info.cbSize = Marshal.SizeOf(typeof(APIsStructs.SHELLEXECUTEINFO));
			info.fMask = APIsEnums.ShellExecuteFlags.INVOKEIDLIST;
			info.hWnd = IntPtr.Zero;
			info.lpVerb = "properties";
			info.lpFile = "";
			info.lpParameters = "";
			info.lpDirectory = path;
			info.nShow = APIsEnums.ShowWindowStyles.SHOW;
			info.hInstApp = IntPtr.Zero;
			IntPtr ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(APIsStructs.SHELLEXECUTEINFO)));
			Marshal.StructureToPtr(info, ptr, false);
			return(ShellExecuteEx(ptr));
		}
		#endregion
		#region GetIcon
		static uint FILE_ATTRIBUTE_DIRECTORY        = 0x00000010;
		static uint FILE_ATTRIBUTE_NORMAL           = 0x00000080;
		public static System.Drawing.Icon GetFileIcon(string strPath, bool bSmall)
		{
			APIsStructs.SHFILEINFO info = new APIsStructs.SHFILEINFO(true);
			APIsEnums.ShellGetFileInformationFlags flags =
				APIsEnums.ShellGetFileInformationFlags.Icon |
				(File.Exists(strPath) ? 0 : APIsEnums.ShellGetFileInformationFlags.UseFileAttributes) |
				(bSmall ? APIsEnums.ShellGetFileInformationFlags.SmallIcon : APIsEnums.ShellGetFileInformationFlags.LargeIcon);
			SHGetFileInfo(strPath,
				FILE_ATTRIBUTE_NORMAL,
				out info,
				(uint)Marshal.SizeOf(info),
				flags);
			System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(info.hIcon);
			return icon;
		}
		public static System.Drawing.Icon GetFolderIcon(string strPath, bool bSmall, bool bOpen)
		{
			APIsStructs.SHFILEINFO info = new APIsStructs.SHFILEINFO(true);
			APIsEnums.ShellGetFileInformationFlags flags =
				APIsEnums.ShellGetFileInformationFlags.Icon |
				(bSmall ? APIsEnums.ShellGetFileInformationFlags.SmallIcon : APIsEnums.ShellGetFileInformationFlags.LargeIcon) |
				(bOpen ? APIsEnums.ShellGetFileInformationFlags.OpenIcon : 0) |
				(Directory.Exists(strPath) ? 0 : APIsEnums.ShellGetFileInformationFlags.UseFileAttributes);
			SHGetFileInfo(strPath,
				FILE_ATTRIBUTE_DIRECTORY,
				out info,
				(uint)Marshal.SizeOf(info),
				flags);
			System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(info.hIcon);
			return icon;
		}
		#endregion

		#region SHParseDisplayName
		[DllImport("shell32.dll")]
		public static extern Int32 SHParseDisplayName(
			[MarshalAs(UnmanagedType.LPWStr)]
			String pszName,
			IntPtr pbc,
			out IntPtr ppidl,
			UInt32 sfgaoIn,
			out UInt32 psfgaoOut);
		#endregion
		#region SHGetDataFromIDList
		[DllImport("shell32.dll", CharSet=CharSet.Auto)]
		public static extern uint SHGetDataFromIDList(
			COMInterfaces.IShellFolder psf,
			IntPtr pidl,
			APIsEnums.ShellFolderGetaFromIDList nFormat,
			out APIsStructs.WIN32_FIND_DATA pv,
			int cb);
		#endregion
		#region SHGetPathFromIDList
		[DllImport("Shell32.Dll", CharSet=CharSet.Auto)]
		[return:MarshalAs(UnmanagedType.Bool)]
		public static extern Boolean SHGetPathFromIDList(
			IntPtr pidl,
			[In,Out,MarshalAs(UnmanagedType.LPTStr)] String pszPath);
		#endregion
		#region SHGetSpecialFolderPath
		[DllImport("Shell32.Dll", CharSet=CharSet.Auto)]
		[return:MarshalAs(UnmanagedType.Bool)]
		public static extern Boolean SHGetSpecialFolderPath(
			IntPtr hwndOwner,
			[In,Out,MarshalAs(UnmanagedType.LPTStr)] String pszPath,
			APIsEnums.ShellSpecialFolders nFolder,
			[In,MarshalAs(UnmanagedType.Bool)] Boolean fCreate);
		#endregion
		#region SHGetDesktopFolder
		/// <summary>
		/// Retrieves the IShellFolder interface for the desktop folder, which is the root of the Shell's namespace.
		/// </summary>
		[DllImport("shell32.dll")]
		public static extern int SHGetDesktopFolder(out COMInterfaces.IShellFolder folder);
		#endregion
		#region SHGetSpecialFolderLocation
		/// <summary>
		/// Retrieves a pointer to the ITEMIDLIST structure of a special folder.
		/// </summary>
		[DllImport("shell32.dll")]
		public static extern int SHGetSpecialFolderLocation(
			IntPtr handle,
			APIsEnums.ShellSpecialFolders nFolder,
			out IntPtr ppidl);
		#endregion
		#region SHBindToParent
		/// <summary>
		/// This function takes the fully-qualified PIDL of a namespace object, and returns a specified interface pointer on the parent object.
		/// </summary>
		[DllImport("shell32.dll")]
		public static extern int SHBindToParent(
			IntPtr pidl,
			Guid iid,
			out COMInterfaces.IShellFolder folder,
			out IntPtr pidlRelative);
		#endregion
		#region SHFileOperation
		/// <summary>
		/// Copies, moves, renames, or deletes a file system object.
		/// </summary>
		[DllImport("shell32.dll", CharSet=CharSet.Unicode)]
		public static extern Int32 SHFileOperation(
			ref APIsStructs.SHFILEOPSTRUCT FileOp);
		#endregion
		#region SHGetFileInfo
		[DllImport("Shell32.dll")]
		public static extern int SHGetFileInfo(
			string pszPath,
			uint dwFileAttributes,
			out APIsStructs.SHFILEINFO psfi,
			uint cbfileInfo,
			APIsEnums.ShellGetFileInformationFlags uFlags);
		#endregion
		#region ShellExecute
		/// <summary>
		/// Execute the file
		/// </summary>
		[DllImport("Shell32.dll")]
		public static extern int ShellExecute
			(IntPtr Hwnd,
			string strOperation,
			string strFile,
			string strParametres,
			string strDirectory,
			int ShowCmd);
		#endregion
		#region ShellExecuteEx
		[DllImport("Shell32.dll")]
		public static extern int ShellExecuteEx(IntPtr infos);
		#endregion

		#region StrRetToBuf
		/// <summary>
		/// Takes a STRRET structure returned by IShellFolder::GetDisplayNameOf, converts it to a string, and places the result in a buffer.
		/// </summary>
		[DllImport("Shlwapi.Dll", CharSet=CharSet.Auto)]
		public static extern uint StrRetToBuf(
			APIsStructs.STRRET pstr,
			IntPtr pidl,
			[In,Out,MarshalAs(UnmanagedType.LPTStr)] String pszBuf,
			uint cchBuf);
		#endregion
	}
}

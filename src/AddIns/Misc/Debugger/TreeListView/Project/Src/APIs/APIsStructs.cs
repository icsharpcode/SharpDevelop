using System;
using System.Runtime.InteropServices;

namespace System.Runtime.InteropServices.APIs
{
	public class APIsStructs
	{
		#region DLLVERSIONINFO
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		public struct DLLVERSIONINFO
		{
			public int cbSize;
			public int dwMajorVersion;
			public int dwMinorVersion;
			public int dwBuildNumber;
			public int dwPlatformID;
		}
		#endregion
		#region DLLVERSIONINFO2
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		public struct DLLVERSIONINFO2
		{
			public DLLVERSIONINFO info1;
			public int dwFlags;
			ulong ullVersion;
		}
		#endregion
		#region WIN32_FIND_DATA
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
			public struct WIN32_FIND_DATA
		{
			public uint fileAttributes;
			public FILETIME creationTime;
			public FILETIME lastAccessTime;
			public FILETIME lastWriteTime;
			public uint fileSizeHigh;
			public uint fileSizeLow;
			public uint reserved0;
			public uint reserved1;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=260)]
			public string fileName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=14)]
			public string alternateFileName;
		}
		#endregion

		#region SHITEMIDLIST
		[StructLayout(LayoutKind.Sequential)]
			public struct SHITEMIDLIST
		{
			public SHITEMID[] mkid;
		}
		#endregion
		#region SHITEMID
		[StructLayout(LayoutKind.Sequential)]
			public struct SHITEMID
		{
			public ushort cb;
			public byte abID;
		}
		#endregion
		#region SHFILEOPSTRUCT
		/// <summary>
		/// Contains information that the SHFileOperation function uses to perform file operations.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
		public struct SHFILEOPSTRUCT
		{
			/// <summary>
			/// Window handle to the dialog box to display information about the status of the file operation.
			/// </summary>
			public IntPtr hwnd;
			/// <summary>
			/// Value that indicates which operation to perform.
			/// </summary>
			public UInt32 wFunc;
			/// <summary>
			/// Address of a buffer to specify one or more source file names.
			/// </summary>
			public IntPtr pFrom;
			/// <summary>
			/// Address of a buffer to contain the name of the destination file or directory.
			/// </summary>
			public IntPtr pTo;
			/// <summary>
			/// Flags that control the file operation (should use APISEnums.FOF).
			/// </summary>
			public UInt16 fFlags;
			/// <summary>
			/// Value that receives TRUE if the user aborted any file operations before they were completed, or FALSE otherwise.
			/// </summary>
			public Int32 fAnyOperationsAborted;
			/// <summary>
			/// A handle to a name mapping object containing the old and new names of the renamed files.
			/// </summary>
			public IntPtr hNameMappings;
			/// <summary>
			/// Address of a string to use as the title of a progress dialog box.
			/// </summary>
			[MarshalAs(UnmanagedType.LPWStr)] public string lpszProgressTitle;
		}
		#endregion
		#region SHELLEXECUTEINFO
		[StructLayoutAttribute(LayoutKind.Sequential)]
			public struct SHELLEXECUTEINFO
		{
			public int cbSize;
			public APIsEnums.ShellExecuteFlags fMask;
			public IntPtr hWnd;
			public string lpVerb;
			public string lpFile;
			public string lpParameters;
			public string lpDirectory;
			public APIsEnums.ShowWindowStyles nShow;
			public IntPtr hInstApp;
			public IntPtr lpIDList;
			public int lpClass;
			public int hkeyClass;
			public int dwHotKey;
			public IntPtr hIcon;
			public IntPtr hProcess;
		}
		#endregion
		#region SHFILEINFO
		[StructLayout(LayoutKind.Sequential)]
		public struct SHFILEINFO
		{
			public SHFILEINFO(bool b)
			{
				hIcon=IntPtr.Zero;iIcon=0;dwAttributes=0;szDisplayName="";szTypeName="";
			}
			public IntPtr hIcon;
			public int iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.LPStr, SizeConst=260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.LPStr, SizeConst=80)]
			public string szTypeName;
		};
		#endregion

		#region STRRET
		[StructLayout(LayoutKind.Sequential)]
			public struct STRRET
		{
			public int uType;
			//		IntPtr pOleStr;
			//		uint uOffset;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=260)] public byte[] cStr;
		}
		#endregion
		#region MENUITMEINFO
		[StructLayoutAttribute(LayoutKind.Sequential)]
			public struct MENUITMEINFO
		{
			public int cbSize; 
			public APIsEnums.MenuItemMasks fMask; 
			public APIsEnums.MenuItemTypes fType; 
			public APIsEnums.MenuItemStates fState; 
			public int wID; 
			public IntPtr hSubMenu; 
			public IntPtr hbmpChecked; 
			public IntPtr hbmpUnchecked; 
			public int dwItemData; 
			public IntPtr dwTypeData; 
			public int cch; 
			public IntPtr hbmpItem;
		}
		#endregion
		#region StartupInfo
		[StructLayout(LayoutKind.Sequential)]
			public class StartupInfo
		{
			public int cb;
			public String lpReserved;
			public String lpDesktop;
			public String lpTitle;
			public int dwX;
			public int dwY;
			public int dwXSize;
			public int dwYSize;
			public int dwXCountChars;
			public int dwYCountChars;
			public int dwFillAttribute;
			public int dwFlags;
			public UInt16 wShowWindow;
			public UInt16 cbReserved2;
			public Byte  lpReserved2;
			public int hStdInput;
			public int hStdOutput;
			public int hStdError;
		}
		#endregion
		#region ProcessInformation
		[StructLayout(LayoutKind.Sequential)]
			public class ProcessInformation
		{
			public int hProcess;
			public int hThread;
			public int dwProcessId;
			public int dwThreadId;
		}
		#endregion
		#region MENUITEMINFO
		[StructLayout(LayoutKind.Sequential)]
			public struct MENUITEMINFO
		{
			public uint cbSize;
			public uint fMask;
			public uint fType;
			public uint fState;
			public int	wID;
			public int	/*HMENU*/	  hSubMenu;
			public int	/*HBITMAP*/   hbmpChecked;
			public int	/*HBITMAP*/	  hbmpUnchecked;
			public int	/*ULONG_PTR*/ dwItemData;
			public String dwTypeData;
			public uint cch;
			public int /*HBITMAP*/ hbmpItem;
		}
		#endregion
		#region FORMATETC
		[StructLayout(LayoutKind.Sequential)]
		public struct FORMATETC
		{
			public APIsEnums.ClipboardFormats	cfFormat;
			public uint ptd;
			public APIsEnums.TargetDevices		dwAspect;
			public int			lindex;
			public APIsEnums.StorageMediumTypes		tymed;
		}
		#endregion
		#region STGMEDIUM
		[StructLayout(LayoutKind.Sequential)]
			public struct STGMEDIUM
		{
			public uint tymed;
			public uint hGlobal;
			public uint pUnkForRelease;
		}
		#endregion
		#region CMINVOKECOMMANDINFO
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
			public struct CMINVOKECOMMANDINFO
		{
			public int cbSize;				// sizeof(CMINVOKECOMMANDINFO)
			public int fMask;				// any combination of CMIC_MASK_*
			public IntPtr hwnd;				// might be NULL (indicating no owner window)
			public IntPtr lpVerb;			// either a string or MAKEINTRESOURCE(idOffset)
			public IntPtr lpParameters;		// might be NULL (indicating no parameter)
			public IntPtr lpDirectory;		// might be NULL (indicating no specific directory)
			public int nShow;				// one of SW_ values for ShowWindow() API
			public int dwHotKey;
			public IntPtr hIcon;
		}
		#endregion

		#region LV_ITEM
		[StructLayoutAttribute(LayoutKind.Sequential)]
		public struct LV_ITEM
		{
			public APIsEnums.ListViewItemFlags mask;
			public Int32 iItem;
			public Int32 iSubItem;
			public APIsEnums.ListViewItemStates state;
			public APIsEnums.ListViewItemStates stateMask;
			public String pszText;
			public Int32 cchTextMax;
			public Int32 iImage;
			public IntPtr lParam;
			public Int32 iIndent;
		}
		#endregion
		#region LVCOLUMN
		[StructLayoutAttribute(LayoutKind.Sequential)]
			public struct LVCOLUMN
		{
			public Int32 mask;
			public Int32 fmt;
			public Int32 cx;
			public string pszText;
			public Int32 cchTextMax;
			public Int32 iSubItem;
			public Int32 iImage;
			public Int32 iOrder;
		}
		#endregion
		#region LVHITTESTINFO
		[StructLayoutAttribute(LayoutKind.Sequential)]
		public struct LVHITTESTINFO
		{
			public POINTAPI pt;
			public int flags;
			public Int32 iItem;
			public Int32 iSubItem;
		}
		#endregion
		#region NMLVDISPINFO
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		public struct NMLVDISPINFO
		{
			public NMHDR hdr;
			public LV_ITEM lvitem;
		}	
		#endregion
		#region NMLISTVIEW
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		public struct NMLISTVIEW
		{
			public NMHDR nmhdr;
			public int iItem;
			public int iSubItem;
			public uint uNewState;
			public uint uOldState;
			public uint uChanged;
			public POINTAPI ptAction;
			public IntPtr lParam;
			public bool NewSelected
			{
				get
				{
					return ((APIsEnums.ListViewItemStates)uNewState & APIsEnums.ListViewItemStates.SELECTED) == APIsEnums.ListViewItemStates.SELECTED;
				}
			}
			public bool OldSelected
			{
				get
				{
					return ((APIsEnums.ListViewItemStates)uOldState & APIsEnums.ListViewItemStates.SELECTED) == APIsEnums.ListViewItemStates.SELECTED;
				}
			}
			public bool NewCheck
			{
				get
				{
					try
					{
						return uNewState >= 0x1000 ? ((uNewState & (uint)APIsEnums.ListViewItemStates.STATEIMAGEMASK) >> 12) - 1 > 0 : false;
					}
					catch
					{
						return false;
					}
				}
			}
			public bool OldCheck
			{
				get
				{
					try
					{
						return uOldState >= 0x1000 ? ((uOldState & (uint)APIsEnums.ListViewItemStates.STATEIMAGEMASK) >> 12) - 1 > 0 : false;
					}
					catch
					{
						return false;
					}
				}
			}
			public bool NewFocused
			{
				get
				{
					return ((APIsEnums.ListViewItemStates)uNewState & APIsEnums.ListViewItemStates.FOCUSED) == APIsEnums.ListViewItemStates.FOCUSED;
				}
			}
			public bool OldFocused
			{
				get
				{
					return ((APIsEnums.ListViewItemStates)uOldState & APIsEnums.ListViewItemStates.FOCUSED) == APIsEnums.ListViewItemStates.FOCUSED;
				}
			}
			public bool Select
			{
				get
				{
					return !OldSelected && NewSelected;
				}
			}
			public bool UnSelect
			{
				get
				{
					return OldSelected && !NewSelected;
				}
			}
			public bool Focus
			{
				get
				{
					return !OldFocused && NewFocused;
				}
			}
			public bool UnFocus
			{
				get
				{
					return OldFocused && !NewFocused;
				}
			}
			public bool Check
			{
				get
				{
					return !OldCheck && NewCheck;
				}
			}
			public bool UnCheck
			{
				get
				{
					return OldCheck && !NewCheck;
				}
			}
		}
		#endregion
		#region HDITEM
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		public struct HDITEM
		{
			public	APIsEnums.HeaderItemFlags mask;
			public	int     cxy;
			public	IntPtr  pszText;
			public	IntPtr  hbm;
			public	int     cchTextMax;
			public	int     fmt;
			public	int     lParam;
			public	int     iImage;      
			public	int     iOrder;
		}	
		#endregion
		#region HD_HITTESTINFO
		[StructLayout(LayoutKind.Sequential)]
			public struct HD_HITTESTINFO 
		{  
			public POINTAPI pt;  
			public APIsEnums.HeaderControlHitTestFlags flags; 
			public int iItem; 
		}
		#endregion

		#region POINTAPI
		[StructLayoutAttribute(LayoutKind.Sequential)]
		public struct POINTAPI
		{
			public POINTAPI(System.Drawing.Point p) {x = p.X; y = p.Y;}
			public POINTAPI(Int32 X, Int32 Y) {x = X; y = Y;}
			public Int32 x;
			public Int32 y;
		}
		#endregion
		#region RECT
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public RECT(Drawing.Rectangle rectangle)
			{	left = rectangle.Left; top = rectangle.Top;
				right = rectangle.Right; bottom = rectangle.Bottom;}
			public int left;
			public int top;
			public int right;
			public int bottom;
		}
		#endregion
		#region SIZE
		[StructLayout(LayoutKind.Sequential)]
		public struct SIZE
		{
			public int cx;
			public int cy;
		}
		#endregion

		#region NMHDR
//		[StructLayout(LayoutKind.Sequential)]
			public struct NMHDR
		{
			public IntPtr hwndFrom;
			public int idFrom;
			public int code;
		}
		#endregion
		#region NMCUSTOMDRAW
		[StructLayout(LayoutKind.Sequential)]
		public struct NMCUSTOMDRAW
		{
			public NMHDR hdr;
			public int dwDrawStage;
			public IntPtr hdc;
			public RECT rc;
			public uint dwItemSpec;
			public uint uItemState;
			public IntPtr lItemlParam;
		}
		#endregion
		#region NMLVCUSTOMDRAW
		[StructLayout(LayoutKind.Sequential)]
		public struct NMLVCUSTOMDRAW 
		{
			public NMCUSTOMDRAW nmcd;
			public int clrText;
			public int clrTextBk;
			public int iSubItem;
			public int dwItemType;
			public int clrFace;
			public int iIconEffect;
			public int iIconPhase;
			public int iPartId;
			public int iStateId;
			public RECT rcText;
			public uint uAlign;
		} 
		#endregion
		#region NMTVCUSTOMDRAW
		[StructLayout(LayoutKind.Sequential)]
		public struct NMTVCUSTOMDRAW 
		{
			public NMCUSTOMDRAW nmcd;
			public uint clrText;
			public uint clrTextBk;
			public int iLevel;
		}
		#endregion

		#region BITMAPINFO_FLAT
		[StructLayout(LayoutKind.Sequential)]
			public struct BITMAPINFO_FLAT 
		{
			public int      bmiHeader_biSize;
			public int      bmiHeader_biWidth;
			public int      bmiHeader_biHeight;
			public short    bmiHeader_biPlanes;
			public short    bmiHeader_biBitCount;
			public int      bmiHeader_biCompression;
			public int      bmiHeader_biSizeImage;
			public int      bmiHeader_biXPelsPerMeter;
			public int      bmiHeader_biYPelsPerMeter;
			public int      bmiHeader_biClrUsed;
			public int      bmiHeader_biClrImportant;
			[MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst=1024)]
			public byte[] bmiColors; 
		}
		#endregion
		#region BITMAPINFOHEADER
		[StructLayout(LayoutKind.Sequential)]
			public class BITMAPINFOHEADER 
		{
			public int      biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
			public int      biWidth;
			public int      biHeight;
			public short    biPlanes;
			public short    biBitCount;
			public int      biCompression;
			public int      biSizeImage;
			public int      biXPelsPerMeter;
			public int      biYPelsPerMeter;
			public int      biClrUsed;
			public int      biClrImportant;
		}
		#endregion

		#region MSG
		[StructLayout(LayoutKind.Sequential)]
		public struct MSG 
		{
			public IntPtr hwnd;
			public int message;
			public IntPtr wParam;
			public IntPtr lParam;
			public int time;
			public int pt_x;
			public int pt_y;
		}
		#endregion
		#region PAINTSTRUCT
		[StructLayout(LayoutKind.Sequential)]
		public struct PAINTSTRUCT
		{
			public IntPtr hdc;
			public int fErase;
			public System.Drawing.Rectangle rcPaint;
			public int fRestore;
			public int fIncUpdate;
			public int Reserved1;
			public int Reserved2;
			public int Reserved3;
			public int Reserved4;
			public int Reserved5;
			public int Reserved6;
			public int Reserved7;
			public int Reserved8;
		}
		#endregion
		#region TRACKMOUSEEVENTS
		[StructLayout(LayoutKind.Sequential)]
			public struct TRACKMOUSEEVENTS
		{
			public uint cbSize;
			public APIsEnums.TrackerEventFlags dwFlags;
			public IntPtr hWnd;
			public uint dwHoverTime;
		}
		#endregion
		#region WINDOWPLACEMENT
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		public struct WINDOWPLACEMENT
		{	
			public uint length; 
			public uint flags; 
			public uint showCmd; 
			public APIsStructs.POINTAPI ptMinPosition; 
			public APIsStructs.POINTAPI ptMaxPosition; 
			public APIsStructs.RECT  rcNormalPosition; 
		}
		#endregion

		#region SCROLLINFO
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		public struct SCROLLINFO
		{
			public 	uint   cbSize;
			public 	uint   fMask;
			public 	int    nMin;
			public 	int    nMax;
			public 	uint   nPage;
			public 	int    nPos;
			public 	int    nTrackPos;
		}
		#endregion
		#region SCROLLBARINFO
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		public struct SCROLLBARINFO
		{
			public uint  cbSize;
			public APIsStructs.RECT  rcScrollBar;
			public int   dxyLineButton;
			public int   xyThumbTop;
			public int   xyThumbBottom;
			public int   reserved;
			[MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst=6)]
			public uint[] rgstate;
		}
		#endregion

		#region PCOMBOBOXINFO
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		public struct PCOMBOBOXINFO
		{
			public uint  cbSize;
			public APIsStructs.RECT  rcItem;
			public APIsStructs.RECT  rcButton;
			public int   stateButton;
			public IntPtr hwndCombo;
			public IntPtr hwndItem;
			public IntPtr hwndList;
		}
		#endregion

		#region BLENDFUNCTION
		[StructLayout(LayoutKind.Sequential, Pack=1)]
		public struct BLENDFUNCTION
		{
			public byte BlendOp;
			public byte BlendFlags;
			public byte SourceConstantAlpha;
			public byte AlphaFormat;
		}
		#endregion

		#region NMHEADER
		[StructLayout(LayoutKind.Sequential, Pack=1)]
		public struct NMHEADER
		{
			public NMHDR nmhdr;
			public int iItem;
			public int iButton;
			public HDITEM pItem;
		}
		#endregion

		[StructLayout(LayoutKind.Sequential, Pack=1)]
		public struct INPUT
		{
			public uint type;
			public int dx;
			public int dy;
			public uint mouseData;
			public uint dwFlags;
			public uint time;
			public uint dwExtra;
		}
	}
}

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Win32;
using NoGoop.Util;

namespace NoGoop.Win32
{
	public delegate bool EnumChildProc(int hwnd, int lParam);

	public delegate int MessageProc(int nCode, int wParam, int lParam);
	public delegate int GetMsgProc(int nCode, int wParam, int lParam);
	public delegate int CallWndProc(int nCode, int wParam, int lParam);
	//public delegate int CallWndProc(int nCode, int wParam, Windows.CWPSTRUCT lParam);
	
		public struct NMHDR
		{
			public IntPtr hwndFrom;   // HWND
			public uint idFrom;     // UINT PTR
			public int code;        // NM  code
		}  

		public struct NMCUSTOMDRAW
		{
			public NMHDR nmcd;
			public int dwDrawStage;
			public IntPtr hdc;
			public RECT rc;
			public int dwItemSpec;        // For TV the node
			public int uItemState;
			public int lItemlParam;
		}

		public struct NMTVCUSTOMDRAW
		{
			public NMCUSTOMDRAW nmcd;
			public uint clrText;
			public uint clrTextBk;
			public uint iLevel;
		}

		public enum RegKind
		{
			RegKind_Default = 0,
			RegKind_Register = 1,
			RegKind_None = 2
		}

		public struct POINT
		{
			public int x;
			public int y;
		}

		public struct RECT
		{
			public int left; 
			public int top; 
			public int right; 
			public int bottom; 
		}

		public struct WINDOWINFO 
		{
			public int cbSize;
			public RECT  rcWindow;
			public RECT  rcClient;
			public int dwStyle;
			public int dwExStyle;
			public int dwWindowStatus;
			public int cxWindowBorders;
			public int cyWindowBorders;
			public int atomWindowType;
			public int wCreatorVersion;
		}

		public struct WINDOWPLACEMENT
		{ 
			public int  length; 
			public int  flags; 
			public int  showCmd; 
			public POINT ptMinPosition; 
			public POINT ptMaxPosition; 
			public RECT  rcNormalPosition; 
		}

		public struct CWPSTRUCT
		{
			public int  lParam; 
			public int  wParam; 
			public int  message; 
			public int  hWnd; 
		}

		public struct SCROLLINFO
		{
			public int  cbSize;
			public int  fMask;
			public int  nMin;
			public int  nMax;
			public int  nPage;
			public int  nPos;
			public int  nTrackPos;
		}

		public struct HH_WINTYPE
		{
			public int             cbStruct;
			public bool            fUniCodeStrings;
			public String          pszType;
			public uint            fsValidMembers;
			public uint            fsWinProperties;
			public String          pszCaption;
			public uint            dwStyles;
			public uint            dwExStyles;
			public RECT            rcWindowPos;
			public int             nShowState;
			public IntPtr          hwndHelp;
			public IntPtr          hwndCaller;
			public IntPtr          hwndToolBar;
			public IntPtr          hwndNavigation;
			public IntPtr          hwndHTML;
			public int             iNavWidth;
			public RECT            rcHTML;
			public String          pszToc;
			public String          pszIndex;
			public String          pszFile;
			public String          pszHome;
			public uint            fsToolBarFlags;
			public bool            fNotExpanded;
			public int             curNavType;
			public int             idNotify;
			public String          pszJump1;
			public String          pszJump2;
			public String          pszUrlJump1;
			public String          pszUrlJump2;
		}

		public struct HH_LAST_ERROR
		{
			public int      cbStruct;
			public int      hr;
			public String   description;
		}

		public class WindowInfo : IComparable
		{
			public int 				hWnd;
			public String 			Name;
			public String 			ClassName;
			public ArrayList 		Children;

			public override String ToString()
			{
				return "0x" + hWnd.ToString("X") + ":" + ClassName + ":" + Name;
			}
			
			public int CompareTo(Object other)
			{
				return hWnd.CompareTo(((WindowInfo)other).hWnd);
			}
		}

	//
	// This class is the interface to native win32 functions
	// and should eventually be replaced by something provided by MS
	// if they ever do it.
	// 
	public class Windows
	{ 
		protected static RegistryKey    _regKeyClassRoot = Registry.ClassesRoot;
		protected static RegistryKey _regKeyComponentCategories = _regKeyClassRoot.OpenSubKey("Component Categories");
		protected static RegistryKey _regKeyAppId = _regKeyClassRoot.OpenSubKey("AppId");
		protected static RegistryKey _regKeyCLSID = _regKeyClassRoot.OpenSubKey("CLSID");
		protected static RegistryKey _regKeyInterface = _regKeyClassRoot.OpenSubKey("Interface");
		protected static RegistryKey _regKeyTypeLib = _regKeyClassRoot.OpenSubKey("TypeLib");

		public static RegistryKey KeyClassRoot {
			get {
				return _regKeyClassRoot;
			}
		}

		public static RegistryKey KeyComponentCategories {
			get {
				return _regKeyComponentCategories;
			}
		}

		public static RegistryKey KeyCLSID {
			get {
				return _regKeyCLSID;
			}
		}

		public static RegistryKey KeyAppId {
			get {
				return _regKeyAppId;
			}
		}

		public static RegistryKey KeyInterface {
			get {
				return _regKeyInterface;
			}
		}

		public static RegistryKey KeyTypeLib {
			get {
				return _regKeyTypeLib;
			}
		}

		public const String COM_ROOT_TYPE_NAME = "System.__ComObject";
		public static Type COM_ROOT_TYPE = Type.GetType(COM_ROOT_TYPE_NAME);
		
		public const int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
		public const int FORMAT_MESSAGE_IGNORE_INSERTS  = 0x00000200;
		public const int FORMAT_MESSAGE_FROM_STRING     = 0x00000400;
		public const int FORMAT_MESSAGE_FROM_HMODULE    = 0x00000800;
		public const int FORMAT_MESSAGE_FROM_SYSTEM     = 0x00001000;
		public const int FORMAT_MESSAGE_ARGUMENT_ARRAY  = 0x00002000;
		public const int FORMAT_MESSAGE_MAX_WIDTH_MASK  = 0x000000FF;
		public const int WH_MSGFILTER = -1;
		public const int WH_GETMESSAGE = 3;
		public const int WH_CALLWNDPROC = 4;

		// From WinUser.h
		public const int WM_CLOSE  = 0x0010;
		public const int WM_NOTIFY = 0x004E;
		public const int WM_HSCROLL = 0x0114;
		public const int WM_VSCROLL = 0x0115;
		public const int WM_LBUTTONDOWN = 0x0201;
		public const int WM_RBUTTONDOWN = 0x0204;

		// From CommCtrl.h
		public const int NM_FIRST = 0;
		public const int NM_CUSTOMDRAW = (NM_FIRST - 12);

		public const int CDDS_PREPAINT =      0x00001;
		public const int CDDS_POSTPAINT =     0x00002;
		public const int CDDS_ITEM =          0x10000;
		public const int CDDS_ITEMPREPAINT =  (CDDS_ITEM | CDDS_PREPAINT);
		public const int CDDS_ITEMPOSTPAINT = (CDDS_ITEM | CDDS_POSTPAINT);
		public const int CDRF_NOTIFYITEMDRAW =    0x00000020;
		public const int CDRF_NOTIFYPOSTPAINT =   0x00000010;

		[DllImport("gdi32.dll")]
		public static extern bool Rectangle(IntPtr hDc, int left, int top, int right, int bottom);

		public const int HOLLOW_BRUSH = 5;
		public const int WHITE_PEN = 6;
		public const int DC_PEN = 19;

		[DllImport("gdi32.dll")]
		public static extern IntPtr GetStockObject(int obj);

		public const int GREY = 0xc0c0c0;
		public const int PS_SOLID = 0;
		public const int PS_DOT = 2;

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreatePen(int style, int width, uint color);

		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject(IntPtr hDc, IntPtr hObj);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObj);

		[DllImport("gdi32.dll")]
		public static extern bool MoveToEx(IntPtr hdc, int x,  int y, out IntPtr oldPos);

		[DllImport("gdi32.dll")]
		public static extern bool LineTo(IntPtr hdc,  int x, int y);

		public const int SB_HORZ =              0;
		public const int SB_VERT =              1;

		[DllImport("user32.dll")]
		public static extern int GetScrollPos(IntPtr hWnd, int nBar);

		public const int SYSCOLOR_BACKGROUND = 1;
		public const int SYSCOLOR_WINDOW = 5;

		[DllImport("user32.dll")]
		public static extern IntPtr GetSysColorBrush(int sysColor);

		[DllImport("user32.dll")]
		public static extern bool ScrollWindow(IntPtr hWnd, int x, int y, ref String clientRect, ref String clipRect);

		[DllImport("user32.dll")]
		public static extern bool UpdateWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern int DrawText(IntPtr hDc, String text, int textLen, ref RECT rect, int options);

		[DllImport("user32.dll")]
		public static extern bool EnumChildWindows(int hWnd, EnumChildProc function, int lParam);

		[DllImport("user32.dll")]
		public static extern bool GetWindowInfo(int hWnd, ref WINDOWINFO wi);

		[DllImport("user32.dll")]
		public static extern int GetDesktopWindow();

		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(int hwnd);

		public const int SW_SHOW_NORMAL    =  1;
		public const int SW_RESTORE        =  9;

		[DllImport("user32.dll")]
		public static extern bool SetWindowPlacement(IntPtr hwnd, ref WINDOWPLACEMENT wp);

		[DllImport("user32.dll")]
		public static extern bool GetWindowPlacement(IntPtr hwnd, ref WINDOWPLACEMENT wp);

		[DllImport("user32.dll")]
		public static extern bool GetWindowText(int hWnd, StringBuilder title, int maxBufSize);

		[DllImport("user32.dll")]
		public static extern bool GetClassName(int hWnd, StringBuilder name, int bufSize);

		public const int HWND_TOP        = 0;
		public const int HWND_BOTTOM     = 1;
		public const int HWND_TOPMOST    = -1;
		public const int HWND_NOTOPMOST  = -2;

		[DllImport("user32.dll")]
		public static extern bool SetWindowPos(int hWnd, int hWndAfter, int x, int y, int width, int height, int flags);

		public static String GetWindowText(int hWnd)
		{
			StringBuilder sb = new StringBuilder(1000);
			if (GetWindowText(hWnd, sb, sb.Capacity)) {
				return sb.ToString();
			} else {
				return null;
			}
		}
		
		public static String GetWindowClass(int hWnd)
		{
			StringBuilder sb = new StringBuilder(1000);
			if (GetClassName(hWnd, sb, sb.Capacity)) {
				return sb.ToString();
			} else {
				return null;
			}
		}

		public static WindowInfo GetWindowInfo(int hWnd)
		{
			WindowInfo wi = new WindowInfo();
			wi.hWnd = hWnd;
			wi.Name = Windows.GetWindowText(hWnd);
			wi.ClassName = Windows.GetWindowClass(hWnd);
			return wi;
		}

		[DllImport("user32.dll")]
		public static extern bool SendMessage(IntPtr hWnd, int message, uint wparam, uint lparam);

		[DllImport("user32.dll")]
		public static extern int CallNextHookEx(int hhk, int nCode, int wParam, int lParam);

		[DllImport("user32.dll")]
		public static extern bool UnhookWindowsHookEx(int idHook); 

		[DllImport("kernel32.dll")]
		public static extern bool SetEnvironmentVariable(String variable, String value);

		[DllImport("kernel32.dll")]
		public static extern int GetCurrentProcessId();

		[DllImport("kernel32.dll")]
		public static extern int GetCurrentProcess();

		[DllImport("kernel32.dll")]
		public static extern int GetCurrentThreadId();

		[DllImport("kernel32.dll")]
		public static extern int GetCurrentThread();

		[DllImport("kernel32.dll")]
		public static extern int GetModuleHandle(string modName);

		[DllImport("kernel32.dll")]
		public static extern int GetLastError();


		[DllImport("kernel32.dll")]
		public static extern int FormatMessage(int dwFlags, int lpSource, int dwMessageId, int dwLanguageId, StringBuilder msgBuffer, int bufSize, int args);

		public const int HH_DISPLAY_TOPIC =  0x0000;
		public const int HH_HELP_CONTEXT =   0x000F;
		public const int HH_SET_WIN_TYPE =   0x0004;
		public const int HH_GET_WIN_TYPE =   0x0005;
		public const int HH_GET_LAST_ERROR = 0x0014;

		[DllImport("HHCtrl.ocx")]
		public static extern int HtmlHelp(IntPtr hWnd,  String helpFile, int command, int data);

		public const int HELP_CONTEXT = 0x0001;

		[DllImport("user32.dll")]
		public static extern int WinHelp(IntPtr hWnd, String helpFile, int command, int data);

		public static String GetLastErrorText()
		{
			StringBuilder sb = new StringBuilder(2000);
			int error = GetLastError();
			if (FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM,
							  0, error, 0, 
							  sb, sb.Capacity, 0) != 0)
			{
				return "Error: " + error + " - " + sb.ToString();
			}
			else
			{
				TraceUtil.WriteLineError(typeof(Win32.Windows),
										 "Error getting message text: " 
										 + GetLastError());
				return null;
			}
		}
	}
}


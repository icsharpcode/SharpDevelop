using System;
using System.Runtime.InteropServices;

namespace System.Runtime.InteropServices.APIs
{
	public class APIsEnums
	{
		#region Window messages / WM
		/// <summary>
		/// Window messages / WM
		/// </summary>
		public enum WindowMessages
		{
			APP = 32768,
			ACTIVATE = 6,
			ACTIVATEAPP = 28,
			AFXFIRST = 864,
			AFXLAST = 895,
			ASKCBFORMATNAME = 780,
			CANCELJOURNAL = 75,
			CANCELMODE = 31,
			CAPTURECHANGED = 533,
			CHANGECBCHAIN = 781,
			CHAR = 258,
			CHARTOITEM = 47,
			CHILDACTIVATE = 34,
			CLEAR = 771,
			CLOSE = 16,
			COMMAND = 273,
			COMMNOTIFY = 68,
			COMPACTING = 65,
			COMPAREITEM = 57,
			CONTEXTMENU = 123,
			COPY = 769,
			COPYDATA = 74,
			CREATE = 1,
			CTLCOLOR = 0x0019,
			CTLCOLORBTN = 309,
			CTLCOLORDLG = 310,
			CTLCOLOREDIT = 307,
			CTLCOLORLISTBOX = 308,
			CTLCOLORMSGBOX = 306,
			CTLCOLORSCROLLBAR = 311,
			CTLCOLORSTATIC = 312,
			CUT = 768,
			DEADCHAR = 259,
			DELETEITEM = 45,
			DESTROY = 2,
			DESTROYCLIPBOARD = 775,
			DEVICECHANGE = 537,
			DEVMODECHANGE = 27,
			DISPLAYCHANGE = 126,
			DRAWCLIPBOARD = 776,
			DRAWITEM = 43,
			DROPFILES = 563,
			ENABLE = 10,
			ENDSESSION = 22,
			ENTERIDLE = 289,
			ENTERMENULOOP = 529,
			ENTERSIZEMOVE = 561,
			ERASEBKGND = 20,
			EXITMENULOOP = 530,
			EXITSIZEMOVE = 562,
			FONTCHANGE = 29,
			GETDLGCODE = 135,
			GETFONT = 49,
			GETHOTKEY = 51,
			GETICON = 127,
			GETMINMAXINFO = 36,
			GETTEXT = 13,
			GETTEXTLENGTH = 14,
			HANDHELDFIRST = 856,
			HANDHELDLAST = 863,
			HELP = 83,
			HOTKEY = 786,
			HSCROLL = 276,
			HSCROLLCLIPBOARD = 782,
			ICONERASEBKGND = 39,
			INITDIALOG = 272,
			INITMENU = 278,
			INITMENUPOPUP = 279,
			UNINITMENUPOPUP = 293,
			INPUTLANGCHANGE = 81,
			INPUTLANGCHANGEREQUEST = 80,
			KEYDOWN = 256,
			KEYUP = 257,
			KILLFOCUS = 8,
			MDIACTIVATE = 546,
			MDICASCADE = 551,
			MDICREATE = 544,
			MDIDESTROY = 545,
			MDIGETACTIVE = 553,
			MDIICONARRANGE = 552,
			MDIMAXIMIZE = 549,
			MDINEXT = 548,
			MDIREFRESHMENU = 564,
			MDIRESTORE = 547,
			MDISETMENU = 560,
			MDITILE = 550,
			MEASUREITEM = 44,
			MENUCHAR = 288,
			MENUSELECT = 287,
			MENUCOMMAND = 294,
			NEXTMENU = 531,
			MOVE = 3,
			MOVING = 534,
			NCACTIVATE = 134,
			NCCALCSIZE = 131,
			NCCREATE = 129,
			NCDESTROY = 130,
			NCHITTEST = 132,
			NCLBUTTONDBLCLK = 163,
			NCLBUTTONDOWN = 161,
			NCLBUTTONUP = 162,
			NCMBUTTONDBLCLK = 169,
			NCMBUTTONDOWN = 167,
			NCMBUTTONUP = 168,
			NCMOUSEMOVE = 160,
			NCPAINT = 133,
			NCRBUTTONDBLCLK = 166,
			NCRBUTTONDOWN = 164,
			NCRBUTTONUP = 165,
			NEXTDLGCTL = 40,
			NOTIFY = 78,
			NOTIFYFORMAT = 85,
			NULL = 0,
			PAINT = 15,
			PAINTCLIPBOARD = 777,
			PAINTICON = 38,
			PALETTECHANGED = 785,
			PALETTEISCHANGING = 784,
			PARENTNOTIFY = 528,
			PASTE = 770,
			PENWINFIRST = 896,
			PENWINLAST = 911,
			POWER = 72,
			POWERBROADCAST = 536,
			PRINT = 791,
			PRINTCLIENT = 792,
			QUERYDRAGICON = 55,
			QUERYENDSESSION = 17,
			QUERYNEWPALETTE = 783,
			QUERYOPEN = 19,
			QUEUESYNC = 35,
			QUIT = 18,
			RENDERALLFORMATS = 774,
			RENDERFORMAT = 773,
			SETCURSOR = 32,
			SETFOCUS = 7,
			SETFONT = 48,
			SETHOTKEY = 50,
			SETICON = 128,
			SETREDRAW = 11,
			SETTEXT = 12,
			SETTINGCHANGE = 26,
			SHOWWINDOW = 24,
			SIZE = 5,
			SIZECLIPBOARD = 779,
			SIZING = 532,
			SPOOLERSTATUS = 42,
			STYLECHANGED = 125,
			STYLECHANGING = 124,
			SYSCHAR = 262,
			SYSCOLORCHANGE = 21,
			SYSCOMMAND = 274,
			SYSDEADCHAR = 263,
			SYSKEYDOWN = 260,
			SYSKEYUP = 261,
			TCARD = 82,
			TIMECHANGE = 30,
			TIMER = 275,
			UNDO = 772,
			USER = 1024,
			USERCHANGED = 84,
			VKEYTOITEM = 46,
			VSCROLL = 277,
			VSCROLLCLIPBOARD = 778,
			WINDOWPOSCHANGED = 71,
			WINDOWPOSCHANGING = 70,
			WININICHANGE = 26,
			KEYFIRST = 256,
			KEYLAST = 264,
			SYNCPAINT = 136,
			MOUSEACTIVATE = 33,
			MOUSEMOVE = 512,
			LBUTTONDOWN = 513,
			LBUTTONUP = 514,
			LBUTTONDBLCLK = 515,
			RBUTTONDOWN = 516,
			RBUTTONUP = 517,
			RBUTTONDBLCLK = 518,
			MBUTTONDOWN = 519,
			MBUTTONUP = 520,
			MBUTTONDBLCLK = 521,
			MOUSEWHEEL = 522,
			MOUSEFIRST = 512,
			MOUSELAST = 522,
			MOUSEHOVER = 0x2A1,
			MOUSELEAVE = 0x2A3,
			SHNOTIFY = 0x0401,
			UNICHAR = 0x0109,
			THEMECHANGED = 0x031A,
		}
		#endregion

		#region Key State Masks / MK
		/// <summary>
		/// Key State Masks / MK
		/// </summary>
		public enum KeyStatesMasks
		{
			LBUTTON          = 0x0001,
			RBUTTON          = 0x0002,
			SHIFT            = 0x0004,
			CONTROL          = 0x0008,
			MBUTTON          = 0x0010,
			XBUTTON1         = 0x0020,
			XBUTTON2         = 0x0040,
		}
		#endregion

		#region Edit Control Notification Codes / EN
		/// <summary>
		/// Edit Control Notification Codes / EN
		/// </summary>
		public enum EditControlNotificationCodes
		{
			SETFOCUS         = 0x0100,
			KILLFOCUS        = 0x0200,
			CHANGE           = 0x0300,
			UPDATE           = 0x0400,
			ERRSPACE         = 0x0500,
			MAXTEXT          = 0x0501,
			HSCROLL          = 0x0601,
			VSCROLL          = 0x0602,
			ALIGN_LTR_EC     = 0x0700,
			ALIGN_RTL_EC     = 0x0701,
		}
		#endregion

		#region Combo Box Notification Codes / CBN
		/// <summary>
		/// Combo Box Notification Codes / CBN
		/// </summary>
		public enum ComboBoxNotificationCodes
		{
			ERRSPACE        = (-1),
			SELCHANGE       = 1,
			DBLCLK          = 2,
			SETFOCUS        = 3,
			KILLFOCUS       = 4,
			EDITCHANGE      = 5,
			EDITUPDATE      = 6,
			DROPDOWN        = 7,
			CLOSEUP         = 8,
			SELENDOK        = 9,
			SELENDCANCEL    = 10,
		}
		#endregion
		#region Combo Box Messages / CB
		/// <summary>
		/// Combo Box Messages / CB
		/// </summary>
		public enum ComboBoxMessages
		{
			GETEDITSEL               = 0x0140,
			LIMITTEXT                = 0x0141,
			SETEDITSEL               = 0x0142,
			ADDSTRING                = 0x0143,
			DELETESTRING             = 0x0144,
			DIR                      = 0x0145,
			GETCOUNT                 = 0x0146,
			GETCURSEL                = 0x0147,
			GETLBTEXT                = 0x0148,
			GETLBTEXTLEN             = 0x0149,
			INSERTSTRING             = 0x014A,
			RESETCONTENT             = 0x014B,
			FINDSTRING               = 0x014C,
			SELECTSTRING             = 0x014D,
			SETCURSEL                = 0x014E,
			SHOWDROPDOWN             = 0x014F,
			GETITEMDATA              = 0x0150,
			SETITEMDATA              = 0x0151,
			GETDROPPEDCONTROLRECT    = 0x0152,
			SETITEMHEIGHT            = 0x0153,
			GETITEMHEIGHT            = 0x0154,
			SETEXTENDEDUI            = 0x0155,
			GETEXTENDEDUI            = 0x0156,
			GETDROPPEDSTATE          = 0x0157,
			FINDSTRINGEXACT          = 0x0158,
			SETLOCALE                = 0x0159,
			GETLOCALE                = 0x015A,
			GETTOPINDEX              = 0x015b,
			SETTOPINDEX              = 0x015c,
			GETHORIZONTALEXTENT      = 0x015d,
			SETHORIZONTALEXTENT      = 0x015e,
			GETDROPPEDWIDTH          = 0x015f,
			SETDROPPEDWIDTH          = 0x0160,
			INITSTORAGE              = 0x0161,
			MULTIPLEADDSTRING        = 0x0163,
			GETCOMBOBOXINFO          = 0x0164,
			FIRST                    = 0x1700,
			SETMINVISIBLE            = FIRST + 1,
			GETMINVISIBLE            = FIRST + 2,
		}
		#endregion

		#region ScrollBar flags / SB
		/// <summary>
		/// ScrollBar flags / SB
		/// </summary>
		public enum ScrollBarFlags
		{
			/// <summary>
			/// Scrolls one line up
			/// </summary>
			LINEUP = 0,
			/// <summary>
			/// Scrolls one line left
			/// </summary>
			LINELEFT = 0,
			/// <summary>
			/// Scrolls one line down
			/// </summary>
			LINEDOWN = 1,
			/// <summary>
			/// Scrolls one page right
			/// </summary>
			LINERIGHT = 1,
			/// <summary>
			/// Scrolls one page up
			/// </summary>
			PAGEUP = 2,
			/// <summary>
			/// Scrolls one page left
			/// </summary>
			PAGELEFT = 2,
			/// <summary>
			/// Scrolls one page down
			/// </summary>
			PAGEDOWN = 3,
			/// <summary>
			/// Scrolls one page right
			/// </summary>
			PAGERIGHT = 3,
			/// <summary>
			/// Scrolls to the upper left
			/// </summary>
			TOP = 6,
			/// <summary>
			/// Scrolls to the lower right
			/// </summary>
			BOTTOM = 7,
			/// <summary>
			/// Ends scroll
			/// </summary>
			ENDSCROLL = 8,
		}
		#endregion
		#region Edit Control Messages / EM
		/// <summary>
		/// Edit Control Messages / EM
		/// </summary>
		public enum EditControlMessages
		{
			GETSEL               = 0x00B0,
			SETSEL               = 0x00B1,
			GETRECT              = 0x00B2,
			SETRECT              = 0x00B3,
			SETRECTNP            = 0x00B4,
			SCROLL               = 0x00B5,
			LINESCROLL           = 0x00B6,
			SCROLLCARET          = 0x00B7,
			GETMODIFY            = 0x00B8,
			SETMODIFY            = 0x00B9,
			GETLINECOUNT         = 0x00BA,
			LINEINDEX            = 0x00BB,
			SETHANDLE            = 0x00BC,
			GETHANDLE            = 0x00BD,
			GETTHUMB             = 0x00BE,
			LINELENGTH           = 0x00C1,
			REPLACESEL           = 0x00C2,
			GETLINE              = 0x00C4,
			LIMITTEXT            = 0x00C5,
			CANUNDO              = 0x00C6,
			UNDO                 = 0x00C7,
			FMTLINES             = 0x00C8,
			LINEFROMCHAR         = 0x00C9,
			SETTABSTOPS          = 0x00CB,
			SETPASSWORDCHAR      = 0x00CC,
			EMPTYUNDOBUFFER      = 0x00CD,
			GETFIRSTVISIBLELINE  = 0x00CE,
			SETREADONLY          = 0x00CF,
			SETWORDBREAKPROC     = 0x00D0,
			GETWORDBREAKPROC     = 0x00D1,
			GETPASSWORDCHAR      = 0x00D2,
			SETMARGINS           = 0x00D3,
			GETMARGINS           = 0x00D4,
			SETLIMITTEXT         = LIMITTEXT,
			GETLIMITTEXT         = 0x00D5,
			POSFROMCHAR          = 0x00D6,
			CHARFROMPOS          = 0x00D7,
			SETIMESTATUS         = 0x00D8,
			GETIMESTATUS         = 0x00D9,
		}
		#endregion

		#region MenuItem Masks / MIIM
		/// <summary>
		/// MenuItem Masks / MIIM
		/// </summary>
		public	enum MenuItemMasks : uint
		{
			STATE =			0x00000001,
			ID =	        0x00000002,
			SUBMENU	=		0x00000004,
			CHECKMARKS =	0x00000008,
			TYPE =			0x00000010,
			DATA =			0x00000020,
			STRING =		0x00000040,
			BITMAP =		0x00000080,
			FTYPE =			0x00000100
		}
		#endregion
		#region MenuItem Flags / MF
		/// <summary>
		/// MenuItem Flags / MF
		/// </summary>
		public	enum	MenuItemFlags : uint
		{
			INSERT =        0x00000000,
			CHANGE =        0x00000080,
			APPEND =        0x00000100,
			DELETE =        0x00000200,
			REMOVE =        0x00001000,
			BYCOMMAND =     0x00000000,
			BYPOSITION =    0x00000400,
			SEPARATOR =     0x00000800,
			ENABLED =       0x00000000,
			GRAYED =        0x00000001,
			DISABLED =      0x00000002,
			UNCHECKED =     0x00000000,
			CHECKED =       0x00000008,
			USECHECKBITMAPS=0x00000200,
			STRING =        0x00000000,
			BITMAP =        0x00000004,
			OWNERDRAW =     0x00000100,
			POPUP =         0x00000010,
			MENUBARBREAK =  0x00000020,
			MENUBREAK =     0x00000040,
			UNHILITE =      0x00000000,
			HILITE =        0x00000080,
			DEFAULT =       0x00001000,
			SYSMENU =       0x00002000,
			HELP =          0x00004000,
			RIGHTJUSTIFY =  0x00004000,
			MOUSESELECT =   0x00008000
		}
		#endregion
		#region MenuItem States / MFS
		/// <summary>
		/// MenuItem States / MFS
		/// </summary>
		public	enum MenuItemStates : uint
		{
			GRAYED =        0x00000003,
			DISABLED =      GRAYED,
			CHECKED =       MenuItemFlags.CHECKED,
			HILITE =        MenuItemFlags.HILITE,
			ENABLED =       MenuItemFlags.ENABLED,
			UNCHECKED =     MenuItemFlags.UNCHECKED,
			UNHILITE =      MenuItemFlags.UNHILITE,
			DEFAULT =       MenuItemFlags.DEFAULT,
			MASK =          0x0000108B,
			HOTTRACKDRAWN = 0x10000000,
			CACHEDBMP =     0x20000000,
			BOTTOMGAPDROP = 0x40000000,
			TOPGAPDROP =    0x80000000,
			GAPDROP =       0xC0000000
		}
		#endregion
		#region Query ContextMenu Flags / CMF
		/// <summary>
		/// QueryContextMenuFlags / CMF
		/// </summary>
		public enum QueryContextMenuFlags: uint
		{
			NORMAL		= 0x00000000,
			DEFAULTONLY	= 0x00000001,
			VERBSONLY	= 0x00000002,
			EXPLORE		= 0x00000004,
			NOVERBS		= 0x00000008,
			CANRENAME	= 0x00000010,
			NODEFAULT	= 0x00000020,
			INCLUDESTATIC= 0x00000040,
			RESERVED	= 0xffff0000      // View specific
		}
		#endregion
		#region Track PopupMenu Flags / TPM
		/// <summary>
		/// TrackPopupMenuFlags / TPM
		/// </summary>
		public enum TrackPopupMenuFlags :uint
		{
			LEFTBUTTON     = 0x0000,
			RIGHTBUTTON    = 0x0002,
			LEFTALIGN      = 0x0000,
			CENTERALIGN    = 0x0004,
			RIGHTALIGN     = 0x0008,
			TOPALIGN       = 0x0000,
			VCENTERALIGN   = 0x0010,
			BOTTOMALIGN    = 0x0020,
			HORIZONTAL     = 0x0000,
			VERTICAL       = 0x0040,
			NONOTIFY       = 0x0080,     /* Don't send any notification msgs */
			RETURNCMD      = 0x0100,
			RECURSE        = 0x0001,
			HORPOSANIMATION = 0x0400,
			HORNEGANIMATION = 0x0800,
			VERPOSANIMATION = 0x1000,
			VERNEGANIMATION = 0x2000,
			NOANIMATION     = 0x4000,
			LAYOUTRTL       = 0x8000,
		}
		#endregion
		#region MenuItem Types / MFT
		/// <summary>
		/// MenuItemTypes / MFT
		/// </summary>
		public enum MenuItemTypes :long
		{
			STRING          = 0x00000000L,
			BITMAP          = 0x00000004L,
			MENUBARBREAK    = 0x00000020L,
			MENUBREAK       = 0x00000040L,
			OWNERDRAW       = 0x00000100L,
			RADIOCHECK      = 0x00000200L,
			SEPARATOR       = 0x00000800L,
			RIGHTORDER      = 0x00002000L,
			RIGHTJUSTIFY    = 0x00004000L,
		}
		#endregion

		#region Clipboard Formats / CLIPFORMAT
		/// <summary>
		/// ClipboardFormats / CLIPFORMAT
		/// </summary>
		public	enum ClipboardFormats : uint
		{
			TEXT =		1,
			BITMAP =		2,
			METAFILEPICT= 3,
			SYLK =		4,
			DIF =		5,
			TIFF =		6,
			OEMTEXT =	7,
			DIB =		8,
			PALETTE =	9,
			PENDATA =	10,
			RIFF =		11,
			WAVE =		12,
			UNICODETEXT= 13,
			ENHMETAFILE= 14,
			HDROP =		15,
			LOCALE =		16,
			MAX =		17,

			OWNERDISPLAY=0x0080,
			DSPTEXT =	0x0081,
			DSPBITMAP =	0x0082,
			DSPMETAFILEPICT= 0x0083,
			DSPENHMETAFILE = 0x008E,

			PRIVATEFIRST=0x0200,
			PRIVATELAST=	0x02FF,

			GDIOBJFIRST =0x0300,
			GDIOBJLAST =	0x03FF
		}
		#endregion
		#region Target Devices / DVASPECT
		/// <summary>
		/// TargetDevices / DVASPECT
		/// </summary>
		public	enum TargetDevices: uint
		{
			CONTENT = 1,
			THUMBNAIL = 2,
			ICON = 4,
			DOCPRINT = 8
		}
		#endregion
		#region Storage Medium Types / TYMED
		/// <summary>
		/// StorageMediumTypes / TYMED
		/// </summary>
		public	enum StorageMediumTypes: uint
		{
			HGLOBAL = 1,
			FILE =	2,
			ISTREAM = 4,
			ISTORAGE= 8,
			GDI =		16,
			MFPICT =	32,
			ENHMF	=	64,
			NULL=		0
		}
		#endregion

		#region Shell Special Folders / CSIDL
		/// <summary>
		/// ShellSpecialFolders / CSIDL
		/// </summary>
		[Flags()]
			public enum ShellSpecialFolders
		{
			DESKTOP                   = 0x0000,		 // <desktop>
			INTERNET                  = 0x0001,
			PROGRAMS                  = 0x0002,        // Start Menu\Programs
			CONTROLS                  = 0x0003,        // My Computer\Control Panel
			PRINTERS                  = 0x0004,        // My Computer\Printers
			PERSONAL                  = 0x0005,        // My Documents
			FAVORITES                 = 0x0006,        // <user name>\Favorites
			STARTUP                   = 0x0007,        // Start Menu\Programs\Startup
			RECENT                    = 0x0008,        // <user name>\Recent
			SENDTO                    = 0x0009,        // <user name>\SendTo
			BITBUCKET                 = 0x000a,        // <desktop>\Recycle Bin
			STARTMENU                 = 0x000b,        // <user name>\Start Menu
			MYDOCUMENTS               = 0x000c,        // logical "My Documents" desktop icon
			MYMUSIC                   = 0x000d,        // "My Music" folder
			MYVIDEO                   = 0x000e,        // "My Videos" folder
			DESKTOPDIRECTORY          = 0x0010,        // <user name>\Desktop
			DRIVES                    = 0x0011,        // My Computer
			NETWORK                   = 0x0012,        // Network Neighborhood (My Network Places)
			NETHOOD                   = 0x0013,        // <user name>\nethood
			FONTS                     = 0x0014,        // windows\fonts
			TEMPLATES                 = 0x0015,
			COMMON_STARTMENU          = 0x0016,        // All Users\Start Menu
			COMMON_PROGRAMS           = 0X0017,        // All Users\Start Menu\Programs
			COMMON_STARTUP            = 0x0018,        // All Users\Startup
			COMMON_DESKTOPDIRECTORY   = 0x0019,        // All Users\Desktop
			APPDATA                   = 0x001a,        // <user name>\Application Data
			PRINTHOOD                 = 0x001b,        // <user name>\PrintHood
			LOCAL_APPDATA             = 0x001c,        // <user name>\Local Settings\Applicaiton Data (non roaming)
			ALTSTARTUP                = 0x001d,        // non localized startup
			COMMON_ALTSTARTUP         = 0x001e,        // non localized common startup
			COMMON_FAVORITES          = 0x001f,
			INTERNET_CACHE            = 0x0020,
			COOKIES                   = 0x0021,
			HISTORY                   = 0x0022,
			COMMON_APPDATA            = 0x0023,        // All Users\Application Data
			WINDOWS                   = 0x0024,        // GetWindowsDirectory()
			SYSTEM                    = 0x0025,        // GetSystemDirectory()
			PROGRAM_FILES             = 0x0026,        // C:\Program Files
			MYPICTURES                = 0x0027,        // C:\Program Files\My Pictures
			PROFILE                   = 0x0028,        // USERPROFILE
			SYSTEMX86                 = 0x0029,        // x86 system directory on RISC
			PROGRAM_FILESX86          = 0x002a,        // x86 C:\Program Files on RISC
			PROGRAM_FILES_COMMON      = 0x002b,        // C:\Program Files\Common
			PROGRAM_FILES_COMMONX86   = 0x002c,        // x86 Program Files\Common on RISC
			COMMON_TEMPLATES          = 0x002d,        // All Users\Templates
			COMMON_DOCUMENTS          = 0x002e,        // All Users\Documents
			COMMON_ADMINTOOLS         = 0x002f,        // All Users\Start Menu\Programs\Administrative Tools
			ADMINTOOLS                = 0x0030,        // <user name>\Start Menu\Programs\Administrative Tools
			CONNECTIONS               = 0x0031,        // Network and Dial-up Connections
			COMMON_MUSIC              = 0x0035,        // All Users\My Music
			COMMON_PICTURES           = 0x0036,        // All Users\My Pictures
			COMMON_VIDEO              = 0x0037,        // All Users\My Video
			RESOURCES                 = 0x0038,        // Resource Direcotry
			RESOURCES_LOCALIZED       = 0x0039,        // Localized Resource Direcotry
			COMMON_OLINKS          = 0x003a,        // Links to All Users OEM specific apps
			CDBURN_AREA               = 0x003b,        // USERPROFILE\Local Settings\Application Data\Microsoft\CD Burning
			COMPUTERSNEARME           = 0x003d,        // Computers Near Me (computered from Workgroup membership)
			FLAG_CREATE               = 0x8000,        // combine with  value to force folder creation in SHGetFolderPath()
			FLAG_DONT_VERIFY          = 0x4000,        // combine with  value to return an unverified folder path
			FLAG_NO_ALIAS             = 0x1000,        // combine with  value to insure non-alias versions of the pidl
			FLAG_PER_USER_INIT        = 0x0800,        // combine with  value to indicate per-user init (eg. upgrade)
			FLAG_MASK                 = 0xFF00,        // mask for all possible flag values
		}
		#endregion
		#region Shell Folder GetaFromIDList / SHGDFIL
		/// <summary>
		/// ShellFolderGetaFromIDList / SHGDFIL
		/// </summary>
		[Flags()]
			public enum ShellFolderGetaFromIDList :int
		{
			FINDDATA = 1,
			NETRESOURCE = 2,
			DESCRIPTIONID = 3
		}
		#endregion
		#region Shell Folder EnumObjects Types / SHCONTF
		/// <summary>
		/// ShellFolderEnumObjectsTypes / SHCONTF
		/// </summary>
		[Flags()]
			public enum ShellFolderEnumObjectsTypes
		{
			FOLDERS = 0x20,
			NONFOLDERS = 0x40,
			INCLUDEHIDDEN = 0x80,
			INIT_ON_FIRST_NEXT = 0x100,
			NETPRINTERSRCH = 0x200,
			SHAREABLE = 0x400,
			STORAGE = 0x800
		}
		#endregion
		#region Shell Folder Attributes / SFGAOF
		/// <summary>
		/// ShellFolderAttributes / SFGAOF
		/// </summary>
		[Flags()]
			public enum ShellFolderAttributes
		{
			CANCOPY = 0x1,
			CANMOVE = 0x2,
			CANLINK = 0x4,
			STORAGE = 0x8,
			CANRENAME = 0x10,
			CANDELETE = 0x20,
			HASPROPSHEET = 0x40,
			DROPTARGET = 0x100,
			CAPABILITYMASK = 0x177,
			ENCRYPTED = 0x2000,
			ISSLOW = 0x4000,
			GHOSTED = 0x8000,
			LINK = 0x10000,
			SHARE = 0x20000,
			READONLY = 0x40000,
			HIDDEN = 0x80000,
			DISPLAYATTRMASK = 0xFC000,
			FILESYSANCESTOR = 0x10000000,
			FOLDER = 0x20000000,
			FILESYSTEM = 0x40000000,
			HASSUBFOLDER = unchecked( (int) 0x80000000 ),
			CONTENTSMASK = unchecked( (int) 0x80000000 ),
			VALIDATE = 0x1000000,
			REMOVABLE = 0x2000000,
			COMPRESSED = 0x4000000,
			BROWSABLE = 0x8000000,
			NONENUMERATED = 0x100000,
			NEWCONTENT = 0x200000,
			CANMONIKER = 0x400000,
			HASSTORAGE = 0x400000,
			STREAM = 0x400000,
			STORAGEANCESTOR = 0x800000,
			STORAGECAPMASK = 0x70C50008
		}
		#endregion
		#region Shell Folder Display Names / SHGNO
		/// <summary>
		/// ShellFolderDisplayNames / SHGNO
		/// </summary>
		[Flags()]
			public enum ShellFolderDisplayNames
		{  
			NORMAL = 0x0,
			INFOLDER = 0x1,
			FOREDITING = 0x1000,
			FORADDRESSBAR = 0x4000, 
			FORPARSING = 0x8000,
		}
		#endregion
		#region Shell Execute Flags / SEE
		/// <summary>
		/// Shell Execute Flags / SEE
		/// </summary>
		public enum ShellExecuteFlags
		{
			CLASSNAME        = 0x00000001,
			CLASSKEY         = 0x00000003,
			IDLIST           = 0x00000004,
			INVOKEIDLIST     = 0x0000000c,
			ICON             = 0x00000010,
			HOTKEY           = 0x00000020,
			NOCLOSEPROCESS   = 0x00000040,
			CONNECTNETDRV    = 0x00000080,
			FLAG_DDEWAIT     = 0x00000100,
			DOENVSUBST       = 0x00000200,
			FLAG_NO_UI       = 0x00000400,
			UNICODE          = 0x00004000,
			NO_CONSOLE       = 0x00008000,
			ASYNCOK          = 0x00100000,
			HMONITOR         = 0x00200000,
			NOQUERYCLASSSTORE= 0x01000000,
			WAITFORINPUTIDLE = 0x02000000,
			FLAG_LOG_USAGE   = 0x04000000,
		}
		#endregion
		#region Shell Get File Information Flags / SHGFI
		/// <summary>
		/// Shell Get File Information Flags / SHGFI
		/// </summary>
		public enum ShellGetFileInformationFlags
		{
			SmallIcon   = 0x00000001,
			OpenIcon   = 0x00000002,
			LargeIcon   = 0x00000000,
			Icon    = 0x00000100,
			DisplayName   = 0x00000200,
			Typename   = 0x00000400,
			SysIconIndex = 0x00004000,
			LinkOverlay = 0x00008000,
			UseFileAttributes = 0x00000010
		}
		#endregion

		#region Form HitTest
		public enum FormHitTest : int
		{
			MinButton = 8,
			MaxButton = 9,
			CloseButton = 20
		}
		#endregion
		#region System Commands / SC
		/// <summary>
		/// System Commands / SC
		/// </summary>
		public enum SystemCommands
		{
			/// <summary>
			/// Sizes the window
			/// </summary>
			SIZE         = 0xF000,
			/// <summary>
			/// Moves the window
			/// </summary>
			MOVE         = 0xF010,
			/// <summary>
			/// Minimizes the window
			/// </summary>
			MINIMIZE     = 0xF020,
			/// <summary>
			/// Maximizes the window
			/// </summary>
			MAXIMIZE     = 0xF030,
			/// <summary>
			/// Moves to the next window
			/// </summary>
			NEXTWINDOW   = 0xF040,
			/// <summary>
			/// Moves to the previous window
			/// </summary>
			PREVWINDOW   = 0xF050,
			/// <summary>
			/// Closes the window
			/// </summary>
			CLOSE        = 0xF060,
			/// <summary>
			/// Scrolls vertically
			/// </summary>
			VSCROLL      = 0xF070,
			/// <summary>
			/// Scrolls horizontally
			/// </summary>
			HSCROLL      = 0xF080,
			/// <summary>
			/// Retrieves the window menu as a result of a mouse click
			/// </summary>
			MOUSEMENU    = 0xF090,
			/// <summary>
			/// Retrieves the window menu as a result of a keystroke
			/// </summary>
			KEYMENU      = 0xF100,
			ARRANGE      = 0xF110,
			/// <summary>
			/// Restores the window to its normal position and size
			/// </summary>
			RESTORE      = 0xF120,
			/// <summary>
			/// Activates the Start menu
			/// </summary>
			TASKLIST     = 0xF130,
			/// <summary>
			/// Executes the screen saver application specified in the [boot] section of the System.ini file
			/// </summary>
			SCREENSAVE   = 0xF140,
			/// <summary>
			/// Activates the window associated with the application-specified hot key. The lParam parameter identifies the window to activate
			/// </summary>
			HOTKEY       = 0xF150,
			/// <summary>
			/// Selects the default item; the user double-clicked the window menu
			/// </summary>
			DEFAULT      = 0xF160,
			/// <summary>
			/// Sets the state of the display. This command supports devices that have power-saving features, such as a battery-powered personal computer. 
			/// The lParam parameter can have the following values:
			///     1 - the display is going to low power
			///     2 - the display is being shut off
			/// </summary>
			MONITORPOWER = 0xF170,
			/// <summary>
			/// Changes the cursor to a question mark with a pointer. If the user then clicks a control in the dialog box, the control receives a WM_HELP message
			/// </summary>
			CONTEXTHELP  = 0xF180,
			SEPARATOR    = 0xF00F,
			ICON         = MINIMIZE,
			ZOOM         = MAXIMIZE
		}
		#endregion

		#region GetCommandString informations / GCS
		/// <summary>
		/// GetCommandString informations / GCS
		/// </summary>
		public enum GetCommandStringInformations
		{
			VERB        = 0x00000004,
			HELPTEXT    = 0x00000005,
			VALIDATE    = 0x00000006,
		}
		#endregion
		#region FileOperations / FO
		/// <summary>
		/// File Operations / FO
		/// </summary>
		public enum FileOperations: int
		{
			Move           = 0x0001,
			Copy           = 0x0002,
			Delete         = 0x0003,
			Rename         = 0x0004,
		}
		#endregion
		#region FileOperation Flags / FOF
		/// <summary>
		/// FileOperation Flag / FOF
		/// </summary>
		public enum FileOperationFlags: short
		{
			MULTIDESTFILES         = 0x0001,
			CONFIRMMOUSE           = 0x0002,
			/// <summary>
			/// Don't create progress/report
			/// </summary>
			SILENT                 = 0x0004,
			RENAMEONCOLLISION      = 0x0008,
			/// <summary>
			/// Don't prompt the user.
			/// </summary>
			NOCONFIRMATION         = 0x0010,
			/// <summary>
			/// Fill in SHFILEOPSTRUCT.hNameMappings
			/// </summary>
			WANTMAPPINGHANDLE      = 0x0020,
			ALLOWUNDO              = 0x0040,
			/// <summary>
			/// On *.*, do only files
			/// </summary>
			FILESONLY              = 0x0080,
		}
		#endregion

		#region ListViewMessages / LVM
		/// <summary>
		/// ListView Messages / LVM
		/// </summary>
		public enum ListViewMessages : int
		{
			FIRST				= 0x1000,
			SCROLL				= FIRST + 20,
			GETITEM				= FIRST + 75,
			SETITEM				= FIRST + 76,
			GETITEMTEXTW		= FIRST + 115,
			SETCOLUMNWIDTH		= FIRST + 30,
			LVSCW_AUTOSIZE			= -1,
			LVSCW_AUTOSIZE_USEHEADER= -2,
			SETITEMSTATE		= FIRST + 43,
			INSERTITEMA			= FIRST + 77,
			DELETEITEM			= FIRST + 8,
			GETITEMCOUNT		= FIRST + 4,
			GETCOUNTPERPAGE		= FIRST + 40,
			GETSUBITEMRECT		= FIRST + 56,
			SUBITEMHITTEST		= FIRST + 57,
			GETCOLUMN			= FIRST + 25,
			SETCOLUMN			= FIRST + 26,
			GETCOLUMNORDERARRAY	= FIRST + 59,
			SETCOLUMNORDERARRAY	= FIRST + 58,
			SETEXTENDEDLISTVIEWSTYLE= FIRST + 54,
			GETEXTENDEDLISTVIEWSTYLE= FIRST + 55,
			EDITLABELW			= FIRST + 118,
			GETITEMRECT        = FIRST + 14,
			HITTEST            = FIRST + 18,
			GETEDITCONTROL     = FIRST + 24,
			CANCELEDITLABEL	   = FIRST + 179,
			GETHEADER          = FIRST + 31,
			REDRAWITEMS        = FIRST + 21,
			GETSELECTIONMARK   = FIRST + 66,
			SETSELECTIONMARK   = FIRST + 67,
			ENSUREVISIBLE       = (FIRST + 19),
		}
		#endregion
		#region ListView Notifications / LVN
		/// <summary>
		/// ListView Notifications / LVN
		/// </summary>
		public enum ListViewNotifications
		{
			FIRST               = (0-100),
			LAST                = (0-199),
			ITEMCHANGING        = (FIRST-0),
			ITEMCHANGED         = (FIRST-1),
			INSERTITEM          = (FIRST-2),
			DELETEITEM          = (FIRST-3),
			DELETEALLITEMS      = (FIRST-4),
			BEGINLABELEDITW     = (FIRST-75),
			ENDLABELEDITW       = (FIRST-76),
			COLUMNCLICK         = (FIRST-8),
			BEGINDRAG           = (FIRST-9),
			BEGINRDRAG          = (FIRST-11),
			ODCACHEHINT         = (FIRST-13),
			ODFINDITEMA         = (FIRST-52),
			ODFINDITEMW         = (FIRST-79),
			ITEMACTIVATE        = (FIRST-14),
			ODSTATECHANGED      = (FIRST-15),
			ODFINDITEM          = ODFINDITEMW,
			HOTTRACK            = (FIRST-21),
			GETDISPINFOA        = (FIRST-50),
			GETDISPINFOW        = (FIRST-77),
			SETDISPINFOA        = (FIRST-51),
			MARQUEEBEGIN        = (FIRST-56),
			SETDISPINFOW        = (FIRST-78),
			BEGINLABELEDIT      = BEGINLABELEDITW,
			ENDLABELEDIT        = ENDLABELEDITW,
			GETDISPINFO         = GETDISPINFOW,
			SETDISPINFO         = SETDISPINFOW,
			BEGINSCROLL         = (FIRST-80),
			ENDSCROLL           = (FIRST-81),
		}
		#endregion
		#region ListViewItem Flags / LVIF
		/// <summary>
		/// ListViewItem Flags / LVIF
		/// </summary>
		public enum ListViewItemFlags : int
		{
			TEXT               = 0x0001,
			IMAGE              = 0x0002,
			PARAM              = 0x0004,
			STATE              = 0x0008,
			INDENT             = 0x0010,
			NORECOMPUTE        = 0x0800,
			GROUPID            = 0x0100,
			COLUMNS            = 0x0200,
		}
		#endregion
		#region ListViewItem States / LVIS
		/// <summary>
		/// ListViewItemState / LVIS
		/// </summary>
		public enum ListViewItemStates : int
		{
			FOCUSED            = 0x0001,
			SELECTED           = 0x0002,
			CUT                = 0x0004,
			DROPHILITED        = 0x0008,
			GLOW               = 0x0010,
			ACTIVATING         = 0x0020,
			OVERLAYMASK        = 0x0F00,
			STATEIMAGEMASK     = 0xF000,
		}
		#endregion
		#region ListView Extended Styles / LVS_EX
		public enum ListViewExtendedStyles
		{
			GRIDLINES        =0x00000001,
			SUBITEMIMAGES    =0x00000002,
			CHECKBOXES       =0x00000004,
			TRACKSELECT      =0x00000008,
			HEADERDRAGDROP   =0x00000010,
			FULLROWSELECT    =0x00000020, 
			ONECLICKACTIVATE =0x00000040,
			TWOCLICKACTIVATE =0x00000080,
			FLATSB           =0x00000100,
			REGIONAL         =0x00000200,
			INFOTIP          =0x00000400,
			UNDERLINEHOT     =0x00000800,
			UNDERLINECOLD    =0x00001000,
			MULTIWORKAREAS   =0x00002000,
			LABELTIP         =0x00004000,
			BORDERSELECT     =0x00008000, 
			DOUBLEBUFFER     =0x00010000,
			HIDELABELS       =0x00020000,
			SINGLEROW        =0x00040000,
			SNAPTOGRID       =0x00080000,  
			SIMPLESELECT     =0x00100000  
		}
		#endregion
		#region List View sub item portion / LVIR
		/// <summary>
		/// List View sub item portion / LVIR
		/// </summary>
		public enum ListViewSubItemPortion
		{
			BOUNDS = 0,
			ICON   = 1,
			LABEL  = 2
		}
		#endregion
		#region ListView HitTest Flags / LVHT
		/// <summary>
		/// ListView HitTest Flags / LVHT
		/// </summary>
		public enum ListViewHitTestFlags
		{
			NOWHERE           = 0x0001,
			ONITEMICON        = 0x0002,
			ONITEMLABEL       = 0x0004,
			ONITEMSTATEICON   = 0x0008,
			ONITEM            = ONITEMICON | ONITEMLABEL | ONITEMSTATEICON,
			ABOVE             = 0x0008,
			BELOW             = 0x0010,
			TORIGHT           = 0x0020,
			TOLEFT            = 0x0040,
		}
		#endregion
		#region Reflected Messages / OCM
		/// <summary>
		/// Reflected Messages / OCM
		/// </summary>
		public enum ReflectedMessages
		{
			_BASE           = (WindowMessages.USER+0x1c00),
			COMMAND         = (_BASE + WindowMessages.COMMAND),
			CTLCOLORBTN     = (_BASE + WindowMessages.CTLCOLORBTN),
			CTLCOLOREDIT    = (_BASE + WindowMessages.CTLCOLOREDIT),
			CTLCOLORDLG     = (_BASE + WindowMessages.CTLCOLORDLG),
			CTLCOLORLISTBOX = (_BASE + WindowMessages.CTLCOLORLISTBOX),
			CTLCOLORMSGBOX  = (_BASE + WindowMessages.CTLCOLORMSGBOX),
			CTLCOLORSCROLLBAR  = (_BASE + WindowMessages.CTLCOLORSCROLLBAR),
			CTLCOLORSTATIC  = (_BASE + WindowMessages.CTLCOLORSTATIC),
			CTLCOLOR        = (_BASE + WindowMessages.CTLCOLOR),
			DRAWITEM        = (_BASE + WindowMessages.DRAWITEM),
			MEASUREITEM     = (_BASE + WindowMessages.MEASUREITEM),
			DELETEITEM      = (_BASE + WindowMessages.DELETEITEM),
			VKEYTOITEM      = (_BASE + WindowMessages.VKEYTOITEM),
			CHARTOITEM      = (_BASE + WindowMessages.CHARTOITEM),
			COMPAREITEM     = (_BASE + WindowMessages.COMPAREITEM),
			HSCROLL         = (_BASE + WindowMessages.HSCROLL),
			VSCROLL         = (_BASE + WindowMessages.VSCROLL),
			PARENTNOTIFY    = (_BASE + WindowMessages.PARENTNOTIFY),
			NOTIFY           = (_BASE + WindowMessages.NOTIFY),
		}
		#endregion

		#region HeaderItem flags / HDI
		/// <summary>
		/// HeaderItem flags / HDI
		/// </summary>
		public enum HeaderItemFlags
		{
			WIDTH               = 0x0001,
			HEIGHT              = WIDTH,
			TEXT                = 0x0002,
			FORMAT              = 0x0004,
			LPARAM              = 0x0008,
			BITMAP              = 0x0010,
			IMAGE               = 0x0020,
			DI_SETITEM          = 0x0040,
			ORDER               = 0x0080
		}
		#endregion
		#region Header Control Messages / HDM
		/// <summary>
		/// Header Control Messages / HDM
		/// </summary>
		public enum HeaderControlMessages : int
		{
			FIRST        =  0x1200,
			GETITEMRECT  = (FIRST + 7),
			HITTEST      = (FIRST + 6),
			SETIMAGELIST = (FIRST + 8),
			GETITEMW     = (FIRST + 11),
			ORDERTOINDEX = (FIRST + 15),
			SETITEM      = (FIRST + 12),
			SETORDERARRAY= (FIRST + 18),
		}
		#endregion
		#region Header Control Notifications / HDN
		/// <summary>
		/// Header Control Notifications / HDN
		/// </summary>
		public enum HeaderControlNotifications
		{
			FIRST              = (0-300),
			ITEMCHANGING       = (FIRST-20),
			ITEMCHANGED        = (FIRST-21),
			ITEMCLICK          = (FIRST-22),
			ITEMDBLCLICK       = (FIRST-23),
			DIVIDERDBLCLICK    = (FIRST-25),
			BEGINTRACK         = (FIRST-26),
			ENDTRACK           = (FIRST-27),
			TRACK              = (FIRST-28),
			GETDISPINFO        = (FIRST-29),
			BEGINDRAG          = (FIRST-10),
			ENDDRAG            = (FIRST-11),
			FILTERCHANGE       = (FIRST-12),
			FILTERBTNCLICK     = (FIRST-13),
		}
		#endregion
		#region Header Control HitTest Flags / HHT
		/// <summary>
		/// Header Control HitTest Flags / HHT
		/// </summary>
		public enum HeaderControlHitTestFlags : uint
		{
			NOWHERE             = 0x0001,
			ONHEADER            = 0x0002,
			ONDIVIDER           = 0x0004,
			ONDIVOPEN           = 0x0008,
			ABOVE               = 0x0100,
			BELOW               = 0x0200,
			TORIGHT             = 0x0400,
			TOLEFT              = 0x0800
		}
		#endregion

		#region Custom Draw Return Flags / CDRF
		/// <summary>
		/// Custom Draw Return Flags / CDRF
		/// </summary>
		public enum CustomDrawReturnFlags
		{
			DODEFAULT          = 0x00000000,
			NEWFONT            = 0x00000002,
			SKIPDEFAULT        = 0x00000004,
			NOTIFYPOSTPAINT    = 0x00000010,
			NOTIFYITEMDRAW     = 0x00000020,
			NOTIFYSUBITEMDRAW  = 0x00000020, 
			NOTIFYPOSTERASE    = 0x00000040
		}
		#endregion
		#region Custom Draw Item State Flags / CDIS
		/// <summary>
		/// CustomDrawItemStateFlags / CDIS
		/// </summary>
		public enum CustomDrawItemStateFlags : uint 
		{
			SELECTED       = 0x0001,
			GRAYED         = 0x0002,
			DISABLED       = 0x0004,
			CHECKED        = 0x0008,
			FOCUS          = 0x0010,
			DEFAULT        = 0x0020,
			HOT            = 0x0040,
			MARKED         = 0x0080,
			INDETERMINATE  = 0x0100
		}
		#endregion
		#region Custom Draw Draw State Flags / CDDS
		/// <summary>
		/// CustomDrawDrawStateFlags / CDDS
		/// </summary>
		public enum CustomDrawDrawStateFlags
		{
			PREPAINT           = 0x00000001,
			POSTPAINT          = 0x00000002,
			PREERASE           = 0x00000003,
			POSTERASE          = 0x00000004,
			ITEM               = 0x00010000,
			ITEMPREPAINT       = (ITEM | PREPAINT),
			ITEMPOSTPAINT      = (ITEM | POSTPAINT),
			ITEMPREERASE       = (ITEM | PREERASE),
			ITEMPOSTERASE      = (ITEM | POSTERASE),
			SUBITEM            = 0x00020000
		}
		#endregion

		#region PatBlt Types
		/// <summary>
		/// PatBlt Types
		/// </summary>
		public enum PatBltTypes
		{
			SRCCOPY          =   0x00CC0020,
			SRCPAINT         =   0x00EE0086,
			SRCAND           =   0x008800C6,
			SRCINVERT        =   0x00660046,
			SRCERASE         =   0x00440328,
			NOTSRCCOPY       =   0x00330008,
			NOTSRCERASE      =   0x001100A6,
			MERGECOPY        =   0x00C000CA,
			MERGEPAINT       =   0x00BB0226,
			PATCOPY          =   0x00F00021,
			PATPAINT         =   0x00FB0A09,
			PATINVERT        =   0x005A0049,
			DSTINVERT        =   0x00550009,
			BLACKNESS        =   0x00000042,
			WHITENESS        =   0x00FF0062
		}
		#endregion
		#region Background Mode
		/// <summary>
		/// Background Mode
		/// </summary>
		public enum BackgroundMode
		{
			TRANSPARENT = 1,
			OPAQUE = 2
		}
		#endregion
		#region StrechModeFlags
		public enum StrechModeFlags
		{
			BLACKONWHITE		= 1,
			WHITEONBLACK        = 2,
			COLORONCOLOR        = 3,
			HALFTONE            = 4,
			MAXSTRETCHBLTMODE   = 4
		}
		#endregion
		#region ShowWindow Styles / SW
		/// <summary>
		/// ShowWindow Styles / SW
		/// </summary>
		public enum ShowWindowStyles : short
		{
			HIDE             = 0,
			SHOWNORMAL       = 1,
			NORMAL           = 1,
			SHOWMINIMIZED    = 2,
			SHOWMAXIMIZED    = 3,
			MAXIMIZE         = 3,
			SHOWNOACTIVATE   = 4,
			SHOW             = 5,
			MINIMIZE         = 6,
			SHOWMINNOACTIVE  = 7,
			SHOWNA           = 8,
			RESTORE          = 9,
			SHOWDEFAULT      = 10,
			FORCEMINIMIZE    = 11,
			MAX              = 11
		}
		#endregion

		#region Windows Hook Codes / WH
		/// <summary>
		/// Windows Hook Codes / WH
		/// </summary>
		public enum WindowsHookCodes
		{
			MSGFILTER        = (-1),
			JOURNALRECORD    = 0,
			JOURNALPLAYBACK  = 1,
			KEYBOARD         = 2,
			GETMESSAGE       = 3,
			CALLWNDPROC      = 4,
			CBT              = 5,
			SYSMSGFILTER     = 6,
			MOUSE            = 7,
			HARDWARE         = 8,
			DEBUG            = 9,
			SHELL            = 10,
			FOREGROUNDIDLE   = 11,
			CALLWNDPROCRET   = 12,
			KEYBOARD_LL      = 13,
			MOUSE_LL         = 14
		}
		#endregion

		#region Cursor Types / IDC
		/// <summary>
		/// Cursor Types / IDC
		/// </summary>
		public enum CursorTypes : uint
		{
			ARROW		= 32512U,
			IBEAM       = 32513U,
			WAIT        = 32514U,
			CROSS       = 32515U,
			UPARROW     = 32516U,
			SIZE        = 32640U,
			ICON        = 32641U,
			SIZENWSE    = 32642U,
			SIZENESW    = 32643U,
			SIZEWE      = 32644U,
			SIZENS      = 32645U,
			SIZEALL     = 32646U,
			NO          = 32648U,
			HAND        = 32649U,
			APPSTARTING = 32650U,
			HELP        = 32651U
		}
		#endregion
		#region System Metrics Codes / SM
		/// <summary>
		/// System Metrics Codes / SM
		/// </summary>
		public enum SystemMetricsCodes
		{
			CXSCREEN             = 0,
			CYSCREEN             = 1,
			CXVSCROLL            = 2,
			CYHSCROLL            = 3,
			CYCAPTION            = 4,
			CXBORDER             = 5,
			CYBORDER             = 6,
			CXDLGFRAME           = 7,
			CYDLGFRAME           = 8,
			CYVTHUMB             = 9,
			CXHTHUMB             = 10,
			CXICON               = 11,
			CYICON               = 12,
			CXCURSOR             = 13,
			CYCURSOR             = 14,
			CYMENU               = 15,
			CXFULLSCREEN         = 16,
			CYFULLSCREEN         = 17,
			CYKANJIWINDOW        = 18,
			MOUSEPRESENT         = 19,
			CYVSCROLL            = 20,
			CXHSCROLL            = 21,
			DEBUG                = 22,
			SWAPBUTTON           = 23,
			RESERVED1            = 24,
			RESERVED2            = 25,
			RESERVED3            = 26,
			RESERVED4            = 27,
			CXMIN                = 28,
			CYMIN                = 29,
			CXSIZE               = 30,
			CYSIZE               = 31,
			CXFRAME              = 32,
			CYFRAME              = 33,
			CXMINTRACK           = 34,
			CYMINTRACK           = 35,
			CXDOUBLECLK          = 36,
			CYDOUBLECLK          = 37,
			CXICONSPACING        = 38,
			CYICONSPACING        = 39,
			MENUDROPALIGNMENT    = 40,
			PENWINDOWS           = 41,
			DBCSENABLED          = 42,
			CMOUSEBUTTONS        = 43,
			CXFIXEDFRAME         = CXDLGFRAME, 
			CYFIXEDFRAME         = CYDLGFRAME,  
			CXSIZEFRAME          = CXFRAME,    
			CYSIZEFRAME          = CYFRAME,    
			SECURE               = 44,
			CXEDGE               = 45,
			CYEDGE               = 46,
			CXMINSPACING         = 47,
			CYMINSPACING         = 48,
			CXSMICON             = 49,
			CYSMICON             = 50,
			CYSMCAPTION          = 51,
			CXSMSIZE             = 52,
			CYSMSIZE             = 53,
			CXMENUSIZE           = 54,
			CYMENUSIZE           = 55,
			ARRANGE              = 56,
			CXMINIMIZED          = 57,
			CYMINIMIZED          = 58,
			CXMAXTRACK           = 59,
			CYMAXTRACK           = 60,
			CXMAXIMIZED          = 61,
			CYMAXIMIZED          = 62,
			NETWORK              = 63,
			CLEANBOOT            = 67,
			CXDRAG               = 68,
			CYDRAG               = 69,
			SHOWSOUNDS           = 70,
			CXMENUCHECK          = 71,  
			CYMENUCHECK          = 72,
			SLOWMACHINE          = 73,
			MIDEASTENABLED       = 74,
			MOUSEWHEELPRESENT    = 75,
			XVIRTUALSCREEN       = 76,
			YVIRTUALSCREEN       = 77,
			CXVIRTUALSCREEN      = 78,
			CYVIRTUALSCREEN      = 79,
			CMONITORS            = 80,
			SAMEDISPLAYFORMAT    = 81,
			CMETRICS             = 83
		}
		#endregion
		#region Windows System Objects / OBJID
		/// <summary>
		/// Windows System Objects / OBJID
		/// </summary>
		public enum SystemObjects : uint
		{
			// Reserved IDs for system objects
			WINDOW        = 0x00000000,
			SYSMENU       = 0xFFFFFFFF,
			TITLEBAR      = 0xFFFFFFFE,
			MENU          = 0xFFFFFFFD,
			CLIENT        = 0xFFFFFFFC,
			VSCROLL       = 0xFFFFFFFB,
			HSCROLL       = 0xFFFFFFFA,
			SIZEGRIP      = 0xFFFFFFF9,
			CARET         = 0xFFFFFFF8,
			CURSOR        = 0xFFFFFFF7,
			ALERT         = 0xFFFFFFF6,
			SOUND         = 0xFFFFFFF5
		}
		#endregion
		#region Tracker Event Flags / TME
		/// <summary>
		/// Tracker Event Flags / TME
		/// </summary>
		public enum TrackerEventFlags : uint
		{
			HOVER	= 0x00000001,
			LEAVE	= 0x00000002,
			QUERY	= 0x40000000,
			CANCEL	= 0x80000000
		}
		#endregion
		#region Draw Text format flags / DT
		/// <summary>
		/// Draw Text format flags / DT
		/// </summary>
		public enum DrawTextFormatFlags
		{
			TOP              = 0x00000000,
			LEFT             = 0x00000000,
			CENTER           = 0x00000001,
			RIGHT            = 0x00000002,
			VCENTER          = 0x00000004,
			BOTTOM           = 0x00000008,
			WORDBREAK        = 0x00000010,
			SINGLELINE       = 0x00000020,
			EXPANDTABS       = 0x00000040,
			TABSTOP          = 0x00000080,
			NOCLIP           = 0x00000100,
			EXTERNALLEADING  = 0x00000200,
			CALCRECT         = 0x00000400,
			NOPREFIX         = 0x00000800,
			INTERNAL         = 0x00001000,
			EDITCONTROL      = 0x00002000,
			PATH_ELLIPSIS    = 0x00004000,
			END_ELLIPSIS     = 0x00008000,
			MODIFYSTRING     = 0x00010000,
			RTLREADING       = 0x00020000,
			WORD_ELLIPSIS    = 0x00040000
		}
		#endregion
		#region Update Layered Windows Flags / ULW
		/// <summary>
		/// Update Layered Windows Flags / ULW
		/// </summary>
		public enum UpdateLayeredWindowFlags
		{
			COLORKEY = 0x00000001,
			ALPHA    = 0x00000002,
			OPAQUE   = 0x00000004
		}
		#endregion
 
		#region Peek Message Flags / PM
		/// <summary>
		/// Peek Message Flags / PM
		/// </summary>
		public enum PeekMessageFlags
		{
			NOREMOVE	= 0,
			REMOVE		= 1,
			NOYIELD		= 2
		}
		#endregion

		#region Notification Messages / NM
		/// <summary>
		/// Notification Messages / NM
		/// </summary>
		public enum NotificationMessages
		{
			FIRST      = (0-0),
			CUSTOMDRAW = (FIRST-12),
			NCHITTEST  = (FIRST-14) 
		}
		#endregion

		#region System Colors / COLOR
		/// <summary>
		/// System Colors / COLOR
		/// </summary>
		public enum SystemColors : int
		{
			SCROLLBAR         = 0,
			BACKGROUND        = 1,
			ACTIVECAPTION     = 2,
			INACTIVECAPTION   = 3,
			MENU              = 4,
			WINDOW            = 5,
			WINDOWFRAME       = 6,
			MENUTEXT          = 7,
			WINDOWTEXT        = 8,
			CAPTIONTEXT       = 9,
			ACTIVEBORDER      = 10,
			INACTIVEBORDER    = 11,
			APPWORKSPACE      = 12,
			HIGHLIGHT         = 13,
			HIGHLIGHTTEXT     = 14,
			BTNFACE           = 15,
			BTNSHADOW         = 16,
			GRAYTEXT          = 17,
			BTNTEXT           = 18,
			INACTIVECAPTIONTEXT = 19,
			BTNHIGHLIGHT      = 20,
		}
		#endregion

		#region Draw Frame Control Flags / DFC
		/// <summary>
		/// Draw Frame Control Flags / DFC
		/// </summary>
		public enum DrawFrameControlFlags : uint
		{
			CAPTION             = 1,
			MENU                = 2,
			SCROLL              = 3,
			BUTTON              = 4,
			POPUPMENU           = 5,
		}
		#endregion
		#region Draw Frame Control State Flags / DFC
		/// <summary>
		/// Draw Frame Control State Flags / DFC
		/// </summary>
		public enum DrawFrameControlStateFlags : uint
		{
			CAPTIONCLOSE       = 0x0000,
			CAPTIONMIN         = 0x0001,
			CAPTIONMAX         = 0x0002,
			CAPTIONRESTORE     = 0x0003,
			CAPTIONHELP        = 0x0004,

			MENUARROW          = 0x0000,
			MENUCHECK          = 0x0001,
			MENUBULLET         = 0x0002,
			MENUARROWRIGHT     = 0x0004,
			SCROLLUP           = 0x0000,
			SCROLLDOWN         = 0x0001,
			SCROLLLEFT         = 0x0002,
			SCROLLRIGHT        = 0x0003,
			SCROLLCOMBOBOX     = 0x0005,
			SCROLLSIZEGRIP     = 0x0008,
			SCROLLSIZEGRIPRIGHT = 0x0010,

			BUTTONCHECK        = 0x0000,
			BUTTONRADIOIMAGE   = 0x0001,
			BUTTONRADIOMASK    = 0x0002,
			BUTTONRADIO        = 0x0004,
			BUTTON3STATE       = 0x0008,
			BUTTONPUSH         = 0x0010,

			INACTIVE           = 0x0100,
			PUSHED             = 0x0200,
			CHECKED            = 0x0400,

			TRANSPARENT        = 0x0800,
			HOT                = 0x1000,
			ADJUSTRECT         = 0x2000,
			FLAT               = 0x4000,
			MONO               = 0x8000,
		}
		#endregion

		#region UxTheme MinButton States / MINBS
		/// <summary>
		/// UxTheme MinButton States / MINBS
		/// </summary>
		public enum UxThemeMinButtonStates
		{
			NORMAL = 1,
			HOT = 2,
			PUSHED = 3,
			DISABLED = 4,
		}
		#endregion
		#region UxTheme Window Parts / WP
		/// <summary>
		/// UxTheme Window Parts / WP
		/// </summary>
		public enum UxThemeWindowParts
		{
			CAPTION = 1,
			SMALLCAPTION = 2,
			MINCAPTION = 3,
			SMALLMINCAPTION = 4,
			MAXCAPTION = 5,
			SMALLMAXCAPTION = 6,
			FRAMELEFT = 7,
			FRAMERIGHT = 8,
			FRAMEBOTTOM = 9,
			SMALLFRAMELEFT = 10,
			SMALLFRAMERIGHT = 11,
			SMALLFRAMEBOTTOM = 12,
			SYSBUTTON = 13,
			MDISYSBUTTON = 14,
			MINBUTTON = 15,
			MDIMINBUTTON = 16,
			MAXBUTTON = 17,
			CLOSEBUTTON = 18,
			SMALLCLOSEBUTTON = 19,
			MDICLOSEBUTTON = 20,
			RESTOREBUTTON = 21,
			MDIRESTOREBUTTON = 22,
			HELPBUTTON = 23,
			MDIHELPBUTTON = 24,
			HORZSCROLL = 25,
			HORZTHUMB = 26,
			VERTSCROLL = 27,
			VERTTHUMB = 28,
			DIALOG = 29,
			CAPTIONSIZINGTEMPLATE = 30,
			SMALLCAPTIONSIZINGTEMPLATE = 31,
			FRAMELEFTSIZINGTEMPLATE = 32,
			SMALLFRAMELEFTSIZINGTEMPLATE = 33,
			FRAMERIGHTSIZINGTEMPLATE = 34,
			SMALLFRAMERIGHTSIZINGTEMPLATE = 35,
			FRAMEBOTTOMSIZINGTEMPLATE = 36,
			SMALLFRAMEBOTTOMSIZINGTEMPLATE = 37,
		}
		#endregion

		public enum MouseEventFlags
		{
			MOVE        = 0x0001,
			LEFTDOWN    = 0x0002,
			LEFTUP      = 0x0004,
			RIGHTDOWN   = 0x0008,
			RIGHTUP     = 0x0010,
			MIDDLEDOWN  = 0x0020,
			MIDDLEUP    = 0x0040,
			XDOWN       = 0x0080,
			XUP         = 0x0100,
			WHEEL       = 0x0800,
			VIRTUALDESK = 0x4000,
			ABSOLUTE    = 0x8000,
		}
	}
}

using System;

namespace System.Runtime.InteropServices.APIs
{
	public class APIsGdi
	{
		[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
		static public extern bool TransparentBlt(
			IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int hHeightDest,
			IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int hHeightSrc,
			int crTransparent);
		[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
		static public extern IntPtr GetDC(IntPtr hWnd);
		[DllImport("gdi32.dll")]
		static public extern bool StretchBlt(IntPtr hDCDest, int XOriginDest, int YOriginDest, int WidthDest, int HeightDest,
			IntPtr hDCSrc,  int XOriginScr, int YOriginSrc, int WidthScr, int HeightScr, APIsEnums.PatBltTypes Rop);
		[DllImport("gdi32.dll")]
		static public extern IntPtr CreateCompatibleDC(IntPtr hDC);
		[DllImport("gdi32.dll")]
		static public extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int Width, int Heigth);
		[DllImport("gdi32.dll")]
		static public extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
		[DllImport("gdi32.dll")]
		static public extern bool BitBlt(IntPtr hDCDest, int XOriginDest, int YOriginDest, int WidthDest, int HeightDest,
			IntPtr hDCSrc,  int XOriginScr, int YOriginSrc, APIsEnums.PatBltTypes flags);
		[DllImport("gdi32.dll")]
		static public extern IntPtr DeleteDC(IntPtr hDC);
		[DllImport("gdi32.dll")]
		static public extern bool PatBlt(IntPtr hDC, int XLeft, int YLeft, int Width, int Height, uint Rop);
		[DllImport("gdi32.dll")]
		static public extern bool DeleteObject(IntPtr hObject);
		[DllImport("gdi32.dll")]
		static public extern uint GetPixel(IntPtr hDC, int XPos, int YPos);
		[DllImport("gdi32.dll")]
		static public extern int SetMapMode(IntPtr hDC, int fnMapMode);
		[DllImport("gdi32.dll")]
		static public extern int GetObjectType(IntPtr handle);
		[DllImport("gdi32")]
		public static extern IntPtr CreateDIBSection(IntPtr hdc, ref APIsStructs.BITMAPINFO_FLAT bmi, 
			int iUsage, ref int ppvBits, IntPtr hSection, int dwOffset);
		[DllImport("gdi32")]
		public static extern int GetDIBits(IntPtr hDC, IntPtr hbm, int StartScan, int ScanLines, int lpBits, APIsStructs.BITMAPINFOHEADER bmi, int usage);
		[DllImport("gdi32")]
		public static extern int GetDIBits(IntPtr hdc, IntPtr hbm, int StartScan, int ScanLines, int lpBits, ref APIsStructs.BITMAPINFO_FLAT bmi, int usage);
		[DllImport("gdi32")]
		public static extern IntPtr GetPaletteEntries(IntPtr hpal, int iStartIndex, int nEntries, byte[] lppe);
		[DllImport("gdi32")]
		public static extern IntPtr GetSystemPaletteEntries(IntPtr hdc, int iStartIndex, int nEntries, byte[] lppe);
		[DllImport("gdi32")]
		public static extern uint SetDCBrushColor(IntPtr hdc,  uint crColor);
		[DllImport("gdi32")]
		public static extern IntPtr CreateSolidBrush(uint crColor);
		[DllImport("gdi32")]
		public static extern int SetBkMode(IntPtr hDC, APIsEnums.BackgroundMode mode);
		[DllImport("gdi32")]
		public static extern int SetViewportOrgEx(IntPtr hdc,  int x, int y,  int param);
		[DllImport("gdi32")]
		public static extern uint SetTextColor(IntPtr hDC, uint colorRef);
		[DllImport("gdi32")]
		public static extern int SetStretchBltMode(IntPtr hDC, APIsEnums.StrechModeFlags StrechMode);
		[DllImport("gdi32")]
		public static extern uint SetPixel(IntPtr hDC, int x, int y, uint color);
	}
}

// *****************************************************************************
// 
//  Copyright 2004, Weifen Luo
//  All rights reserved. The software and associated documentation 
//  supplied hereunder are the proprietary information of Weifen Luo
//  and are supplied subject to licence terms.
// 
//  WinFormsUI Library Version 1.0
// *****************************************************************************

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI
{
	/// <summary>
	/// Summary description for DrawHelper.
	/// </summary>
	internal class DrawHelper
	{
		private static IntPtr m_halfToneBrush = IntPtr.Zero;

		public static Region CreateDragOutline(Rectangle rect, int indent)
		{
			return CreateRectangleDragOutline(rect, indent);
		}

		public static Region CreateDragOutline(Rectangle[] rects, int indent)
		{
			Region region = CreateRectangleDragOutline(rects[0], indent);

			for (int i=1; i<rects.Length; i++)
			{
				using (Region newRegion = CreateRectangleDragOutline(rects[i], indent))
				{
					region.Xor(newRegion);
				}
			}

			return region;
		}

		private static Region CreateRectangleDragOutline(Rectangle rect, int indent)
		{
			// Create region for whole of the new rectangle
			Region region = new Region(rect);

			// If the rectangle is to small to make an inner object from, then just use the outer
			if ((indent <= 0) || (rect.Width <= 2 * indent) || (rect.Height <= 2 * indent))
				return region;

			rect.X += indent;
			rect.Y += indent;
			rect.Width -= 2 * indent;
			rect.Height -= 2 * indent;

			region.Xor(rect);

			return region;
		}

		public static void DrawDragOutline(Region region)
		{
			if (region == null)
				return;

			// Get hold of the DC for the desktop
			IntPtr hDC = User32.GetDC(IntPtr.Zero);

			// Define the area we are allowed to draw into
			IntPtr hRegion = region.GetHrgn(Graphics.FromHdc(hDC));
			Gdi32.SelectClipRgn(hDC, hRegion);

			Win32.RECT rectBox = new Win32.RECT();
				 
			// Get the smallest rectangle that encloses region
			Gdi32.GetClipBox(hDC, ref rectBox);

			IntPtr brushHandler = GetHalfToneBrush();

			// Select brush into the device context
			IntPtr oldHandle = Gdi32.SelectObject(hDC, brushHandler);

			// Blit to screen using provided pattern brush and invert with existing screen contents
			Gdi32.PatBlt(hDC, 
				rectBox.left, 
				rectBox.top, 
				rectBox.right - rectBox.left, 
				rectBox.bottom - rectBox.top, 
				(uint)Win32.RasterOperations.PATINVERT);

			// Put old handle back again
			Gdi32.SelectObject(hDC, oldHandle);

			// Reset the clipping region
			Gdi32.SelectClipRgn(hDC, IntPtr.Zero);

			Gdi32.DeleteObject(hRegion);

			// Must remember to release the HDC resource!
			User32.ReleaseDC(IntPtr.Zero, hDC);
		}

		private static IntPtr GetHalfToneBrush()
		{
			if (m_halfToneBrush == IntPtr.Zero)
			{	
				Bitmap bitmap = new Bitmap(8,8,PixelFormat.Format32bppArgb);

				Color white = Color.FromArgb(255,255,255,255);
				Color black = Color.FromArgb(255,0,0,0);

				bool flag=true;

				// Alternate black and white pixels across all lines
				for(int x=0; x<8; x++, flag = !flag)
					for(int y=0; y<8; y++, flag = !flag)
						bitmap.SetPixel(x, y, (flag ? white : black));

				IntPtr hBitmap = bitmap.GetHbitmap();

				Win32.LOGBRUSH brush = new Win32.LOGBRUSH();

				brush.lbStyle = (uint)Win32.BrushStyles.BS_PATTERN;
				brush.lbHatch = (uint)hBitmap;

				m_halfToneBrush = Gdi32.CreateBrushIndirect(ref brush);
				Gdi32.DeleteObject(hBitmap);
			}

			return m_halfToneBrush;
		}
	}
}

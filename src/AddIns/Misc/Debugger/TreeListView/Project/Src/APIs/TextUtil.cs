using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Runtime.InteropServices.APIs
{
	/// <summary>
	/// Summary description for TextUtil.
	/// </summary>
	public class TextUtil
	{
		#region Constructor
		// No need to construct this object
		private TextUtil()
		{
		}
		#endregion

		#region Methods
		public static Size GetTextSize(Graphics graphics, string text, Font font)
		{
			IntPtr hdc = IntPtr.Zero;
			if ( graphics != null )
			{
				// Get device context from the graphics passed in
				hdc = graphics.GetHdc();
			}
			else
			{
				// Get screen device context
                hdc = APIsGdi.GetDC(IntPtr.Zero);
			}

			IntPtr fontHandle = font.ToHfont();
			IntPtr currentFontHandle = APIsGdi.SelectObject(hdc, fontHandle);
			
			APIsStructs.RECT rect = new APIsStructs.RECT();
			rect.left = 0;
			rect.right = 0;
			rect.top = 0;
			rect.bottom = 0;
		
			APIsUser32.DrawText(hdc, text, text.Length, ref rect, 
				APIsEnums.DrawTextFormatFlags.SINGLELINE | APIsEnums.DrawTextFormatFlags.LEFT | APIsEnums.DrawTextFormatFlags.CALCRECT);
			APIsGdi.SelectObject(hdc, currentFontHandle);
			APIsGdi.DeleteObject(fontHandle);

			if(graphics != null)
				graphics.ReleaseHdc(hdc);
			else
				APIsUser32.ReleaseDC(IntPtr.Zero, hdc);
							
			return new Size(rect.right - rect.left, rect.bottom - rect.top);
		}

		public static Size GetTextSize(Graphics graphics, string text, Font font, ref Rectangle rc, APIsEnums.DrawTextFormatFlags drawFlags)
		{
			IntPtr hdc = IntPtr.Zero;
			if ( graphics != null )
			{
				// Get device context from the graphics passed in
				hdc = graphics.GetHdc();
			}
			else
			{
				// Get screen device context
				hdc = APIsGdi.GetDC(IntPtr.Zero);
			}

			IntPtr fontHandle = font.ToHfont();
			IntPtr currentFontHandle = APIsGdi.SelectObject(hdc, fontHandle);
			
			APIsStructs.RECT rect = new APIsStructs.RECT();
			rect.left = rc.Left;
			rect.right = rc.Right;
			rect.top = rc.Top;
			rect.bottom = rc.Bottom;
		
			APIsUser32.DrawText(hdc, text, text.Length, ref rect, drawFlags);
			APIsGdi.SelectObject(hdc, currentFontHandle);
			APIsGdi.DeleteObject(fontHandle);

			if(graphics != null)
				graphics.ReleaseHdc(hdc);
			else
				APIsUser32.ReleaseDC(IntPtr.Zero, hdc);
							
			return new Size(rect.right - rect.left, rect.bottom - rect.top);
			
		}

		public static void DrawText(Graphics graphics, string text, Font font, Rectangle rect)
		{
			IntPtr hdc = graphics.GetHdc();
			IntPtr fontHandle = font.ToHfont();
			IntPtr currentFontHandle = APIsGdi.SelectObject(hdc, fontHandle);
			APIsGdi.SetBkMode(hdc, APIsEnums.BackgroundMode.TRANSPARENT);
           						
            APIsStructs.RECT rc = new APIsStructs.RECT();
			rc.left = rect.Left;
			rc.top = rect.Top;
			rc.right = rc.left + rect.Width;
			rc.bottom = rc.top + rect.Height;
			
			APIsUser32.DrawText(hdc, text, text.Length, ref rc, 
				APIsEnums.DrawTextFormatFlags.SINGLELINE | APIsEnums.DrawTextFormatFlags.LEFT 
				| APIsEnums.DrawTextFormatFlags.MODIFYSTRING | APIsEnums.DrawTextFormatFlags.WORD_ELLIPSIS);
			APIsGdi.SelectObject(hdc, currentFontHandle);
			APIsGdi.DeleteObject(fontHandle);
			graphics.ReleaseHdc(hdc);
		}

		public static void DrawText(Graphics graphics, string text, Font font, Rectangle rect, Color textColor)
		{
			IntPtr hdc = graphics.GetHdc();
			IntPtr fontHandle = font.ToHfont();
			IntPtr currentFontHandle = APIsGdi.SelectObject(hdc, fontHandle);
			uint colorRef = ColorUtil.GetCOLORREF(textColor);
			APIsGdi.SetTextColor(hdc, colorRef);
			APIsGdi.SetBkMode(hdc, APIsEnums.BackgroundMode.TRANSPARENT);
           						
			APIsStructs.RECT rc = new APIsStructs.RECT();
			rc.left = rect.Left;
			rc.top = rect.Top;
			rc.right = rc.left + rect.Width;
			rc.bottom = rc.top + rect.Height;
			
			APIsUser32.DrawText(hdc, text, text.Length, ref rc, 
				APIsEnums.DrawTextFormatFlags.SINGLELINE | APIsEnums.DrawTextFormatFlags.LEFT
				| APIsEnums.DrawTextFormatFlags.MODIFYSTRING| APIsEnums.DrawTextFormatFlags.WORD_ELLIPSIS);
			APIsGdi.SelectObject(hdc, currentFontHandle);
			APIsGdi.DeleteObject(fontHandle);
			graphics.ReleaseHdc(hdc);
		}

		public static void DrawReverseString(Graphics g, 
			String drawText, 
			Font drawFont, 
			Rectangle drawRect,
			Brush drawBrush,
			StringFormat drawFormat)
		{
			GraphicsContainer container = g.BeginContainer();

			// The text will be rotated around the origin (0,0) and so needs moving
			// back into position by using a transform
			g.TranslateTransform(drawRect.Left * 2 + drawRect.Width, 
				drawRect.Top * 2 + drawRect.Height);

			// Rotate the text by 180 degress to reverse the direction 
			g.RotateTransform(180);

			// Draw the string as normal and let then transforms do the work
			g.DrawString(drawText, drawFont, drawBrush, drawRect, drawFormat);

			g.EndContainer(container);
		}

		#endregion
	}
}

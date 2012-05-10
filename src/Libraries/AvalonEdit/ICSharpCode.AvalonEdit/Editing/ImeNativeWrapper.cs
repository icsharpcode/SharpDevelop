// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using Draw = System.Drawing;

namespace ICSharpCode.AvalonEdit.Editing
{
	/// <summary>
	/// Native API required for IME support.
	/// </summary>
	static class ImeNativeWrapper
	{
		[StructLayout(LayoutKind.Sequential)]
		struct CompositionForm
		{
			public int dwStyle;
			public POINT ptCurrentPos;
			public RECT rcArea;
		}
		
		[StructLayout(LayoutKind.Sequential)]
		struct POINT
		{
			public int x;
			public int y;
		}
		
		[StructLayout(LayoutKind.Sequential)]
		struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}
		
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		class LOGFONT
		{
			public int lfHeight = 0;
			public int lfWidth = 0;
			public int lfEscapement = 0;
			public int lfOrientation = 0;
			public int lfWeight = 0;
			public byte lfItalic = 0;
			public byte lfUnderline = 0;
			public byte lfStrikeOut = 0;
			public byte lfCharSet = 0;
			public byte lfOutPrecision = 0;
			public byte lfClipPrecision = 0;
			public byte lfQuality = 0;
			public byte lfPitchAndFamily = 0;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)] public string lfFaceName = null;
		}
		
		const int CPS_CANCEL = 0x4;
		const int NI_COMPOSITIONSTR = 0x15;
		const int GCS_COMPSTR = 0x0008;
		
		public const int WM_IME_COMPOSITION = 0x10F;
		public const int WM_INPUTLANGCHANGE = 0x51;
		
		[DllImport("imm32.dll")]
		static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);
		[DllImport("imm32.dll")]
		static extern IntPtr ImmGetContext(IntPtr hWnd);
		[DllImport("imm32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool ImmNotifyIME(IntPtr hIMC, int dwAction, int dwIndex, int dwValue = 0);
		[DllImport("imm32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);
		[DllImport("imm32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool ImmSetCompositionWindow(IntPtr hIMC, ref CompositionForm form);
		[DllImport("imm32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool ImmSetCompositionFont(IntPtr hIMC, ref LOGFONT font);
		[DllImport("imm32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool ImmGetCompositionFont(IntPtr hIMC, out LOGFONT font);
		
		public static IntPtr AssociateContext(HwndSource source, IntPtr hIMC)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			return ImmAssociateContext(source.Handle, hIMC);
		}
		
		public static bool NotifyIme(IntPtr hIMC)
		{
			return ImmNotifyIME(hIMC, NI_COMPOSITIONSTR, CPS_CANCEL);
		}
		
		public static IntPtr GetContext(HwndSource source)
		{
			if (source == null)
				return IntPtr.Zero;
			return ImmGetContext(source.Handle);
		}
		
		public static bool ReleaseContext(HwndSource source, IntPtr hIMC)
		{
			return source != null && hIMC != IntPtr.Zero && ImmReleaseContext(source.Handle, hIMC);
		}
		
		public static bool SetCompositionWindow(HwndSource source, IntPtr hIMC, TextArea textArea)
		{
			if (textArea == null)
				throw new ArgumentNullException("textArea");
			Rect textViewBounds = textArea.TextView.GetBounds();
			Rect characterBounds = textArea.TextView.GetCharacterBounds(textArea.Caret.Position, source);
			if (source != null) {
				Matrix transformToDevice = source.CompositionTarget.TransformToDevice;
				textViewBounds.Transform(transformToDevice);
				characterBounds.Transform(transformToDevice);
			}
			CompositionForm form = new CompositionForm();
			form.dwStyle = 0x0020;
			form.ptCurrentPos.x = (int)Math.Max(characterBounds.Left, textViewBounds.Left);
			form.ptCurrentPos.y = (int)Math.Max(characterBounds.Top, textViewBounds.Top);
			form.rcArea.left = (int)textViewBounds.Left;
			form.rcArea.top = (int)textViewBounds.Top;
			form.rcArea.right = (int)textViewBounds.Right;
			form.rcArea.bottom = (int)textViewBounds.Bottom;
			return ImmSetCompositionWindow(hIMC, ref form);
		}
		
		public static bool SetCompositionFont(HwndSource source, IntPtr hIMC, TextArea textArea)
		{
			if (textArea == null)
				throw new ArgumentNullException("textArea");
//			LOGFONT font = new LOGFONT();
//			ImmGetCompositionFont(hIMC, out font);
			return false;
		}
		
		static Rect GetBounds(this TextView textView)
		{
			Point location = textView.TranslatePoint(new Point(0,0), textView);
			return new Rect(location, new Size(textView.ActualWidth, textView.ActualHeight));
		}
		
		static readonly Rect EMPTY_RECT = new Rect(0, 0, 0, 0);
		
		static Rect GetCharacterBounds(this TextView textView, TextViewPosition pos, HwndSource source)
		{
			VisualLine vl = textView.GetVisualLine(pos.Line);
			if (vl == null) return EMPTY_RECT;
			TextLine line = vl.GetTextLine(pos.VisualColumn);
			double offset = vl.GetTextLineVisualYPosition(line, VisualYPosition.LineTop) - textView.ScrollOffset.Y;
			Rect r = line.GetTextBounds(pos.VisualColumn, 1).First().Rectangle;
			r.Offset(-textView.ScrollOffset.X, offset);
			// this may happen during layout changes in AvalonDock, so we just return an empty rectangle
			// in those cases. It should be refreshed immediately.
			if (!source.RootVisual.IsAncestorOf(textView)) return EMPTY_RECT;
			Point pointOnRootVisual = textView.TransformToAncestor(source.RootVisual).Transform(r.Location);
			Point pointOnHwnd = pointOnRootVisual.TransformToDevice(source.RootVisual);
			r.Location = pointOnHwnd;
			return r;
		}
	}
}

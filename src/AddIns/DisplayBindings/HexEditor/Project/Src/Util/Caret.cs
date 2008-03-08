// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;

namespace HexEditor.Util
{
	/// <summary>
	/// Represents a caret
	/// </summary>
	public class Caret
	{
		int width;
		int height;
		int offset;
		Control control;
		
		public Caret()
		{
		}
		
		public Caret(Control control)
		{
			this.control = control;
		}
		
		public Caret(int width, int height)
		{
			this.width = width;
			this.height = height;
		}
		
		public Caret(int width, int height, int offset)
		{
			this.width = width;
			this.height = height;
			this.offset = offset;
		}
		
		public Caret(Control control, int width, int height, int offset)
		{
			this.width = width;
			this.height = height;
			this.offset = offset;
			this.control = control;
		}
		
		public int Width {
			get { return width; }
			set { width = value; }
		}
		
		public int Height {
			get { return height; }
			set { height = value; }
		}
		
		public int Offset {
			get { return offset; }
			set { offset = value; }
		}
		
		public Control Control {
			get { return control; }
			set { control = value; }
		}
		
		public void Create(Control control, int width, int height)
		{
			NativeMethods.CreateCaret(control.Handle, 0, width, height);
			this.Control = control;
			this.Width = width;
			this.Height = height;
		}
		
		public void Destroy()
		{
			NativeMethods.DestroyCaret();
			this.Control = null;
			this.Width = 0;
			this.Height = 0;
			this.Offset = -1;
		}
		
		public void Show()
		{
			NativeMethods.ShowCaret(control.Handle);
		}
		
		public void Hide()
		{
			NativeMethods.HideCaret(control.Handle);
		}
		
		public void SetToPosition(Point position)
		{
			NativeMethods.SetCaretPos(position.X, position.Y);
		}
	}
}

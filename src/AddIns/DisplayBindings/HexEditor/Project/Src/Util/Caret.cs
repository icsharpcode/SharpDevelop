// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
		Point position;
		Graphics g;
		
		public Caret(Graphics control, int width, int height, int offset)
		{
			this.width = width;
			this.height = height;
			this.offset = offset;
			this.g = control;
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
		
		public Graphics Graphics {
			get { return g; }
			set { g = value; }
		}
		
		public void Create(Graphics control, int width, int height)
		{
			this.Graphics = control;
			this.Width = width;
			this.Height = height;
		}
		
		public void SetToPosition(Point position)
		{
			this.position = position;
			DrawCaret(position, this.width, this.height);
		}
		
		private void DrawCaret(Point start, int width, int height)
		{
			if (width > 1)
				g.DrawRectangle(Pens.Black, start.X - 1, start.Y, width, height - 1);
			else
				g.DrawLine(Pens.Black, start.X - 1, start.Y, start.X - 1, start.Y + height - 1);
		}
		
		public void DrawCaret()
		{
			DrawCaret(this.position, this.width, this.height);
		}
	}
}

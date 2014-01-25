// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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

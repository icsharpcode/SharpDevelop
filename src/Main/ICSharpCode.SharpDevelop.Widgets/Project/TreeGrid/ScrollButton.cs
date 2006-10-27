// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Widgets.TreeGrid
{
	public class ScrollButtonControl : Control
	{
		public ScrollButtonControl()
		{
			this.BackColor = DynamicListColumn.DefaultBackColor;
			this.TabStop = false;
			this.SetStyle(ControlStyles.Selectable, false);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
		}
		
		protected override Size DefaultSize {
			get {
				return new Size(14, 14);
			}
		}
		
		ScrollButton arrow = ScrollButton.Down;
		
		public ScrollButton Arrow {
			get {
				return arrow;
			}
			set {
				arrow = value;
				Invalidate();
			}
		}
		
		bool drawSeparatorLine = true;
		
		public bool DrawSeparatorLine {
			get {
				return drawSeparatorLine;
			}
			set {
				drawSeparatorLine = value;
				Invalidate();
			}
		}
		
		Color highlightColor = SystemColors.Highlight;
		
		public Color HighlightColor {
			get {
				return highlightColor;
			}
			set {
				highlightColor = value;
				Invalidate();
			}
		}
		
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			const int margin = 2;
			int height = this.ClientSize.Height;
			int size = height - 2 * margin;
			int width = this.ClientSize.Width;
			int left = (width - size) / 2;
			int right = (width + size) / 2;
			Point[] triangle;
			switch (arrow) {
				case ScrollButton.Down:
					triangle = new Point[] {
						new Point(left, margin), new Point(right, margin), new Point(width / 2, margin + size)
					};
					if (drawSeparatorLine)
						e.Graphics.DrawLine(SystemPens.GrayText, 0, 0, width, 0);
					break;
				case ScrollButton.Up:
					triangle = new Point[] {
						new Point(left, margin + size), new Point(right, margin + size), new Point(width / 2, margin)
					};
					if (drawSeparatorLine)
						e.Graphics.DrawLine(SystemPens.GrayText, 0, height - 1, width, height - 1);
					break;
				default:
					return;
			}
			Color color;
			if (Enabled)
				color = cursorEntered ? HighlightColor : ForeColor;
			else
				color = SystemColors.GrayText;
			using (Brush b = new SolidBrush(color)) {
				e.Graphics.FillPolygon(b, triangle);
			}
		}
		
		bool cursorEntered = false;
		
		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			cursorEntered = true;
			Invalidate();
		}
		
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			cursorEntered = false;
			Invalidate();
		}
	}
}

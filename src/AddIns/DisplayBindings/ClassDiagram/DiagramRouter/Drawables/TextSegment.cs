/* Created by SharpDevelop.
 * User: itai
 * Date: 28/09/2006
 * Time: 19:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Tools.Diagrams.Drawables
{
	public class TextSegment : BaseRectangle, IDrawableRectangle, IDisposable
	{
		float tw;
		Font font;
		Brush brush = Brushes.Black;
		Graphics g;
		string text;
		StringFormat sf = new StringFormat();
		
		public TextSegment (Graphics graphics, string text)
			: this (graphics, text, new Font(System.Drawing.FontFamily.GenericSansSerif, 10.0f), false)
		{
		}
		
		public TextSegment (Graphics graphics, string text, Font font, bool resizable)
		{
			if (graphics == null) throw new ArgumentNullException("graphics");
			this.g = graphics;
			this.text = text;
			this.font = font;
			sf.Trimming = StringTrimming.EllipsisCharacter;
			MeasureString();
			if (resizable)
				Width = -1;
			else
				Width = float.NaN;
		}
			
		private void MeasureString ()
		{
			if (text != null && font != null && g != null)
				tw = g.MeasureString(text, font).Width;
			else
				tw = float.NaN;
		}
		
		public override float Width
		{
			get
			{
				if (float.IsNaN(base.Width)) return tw;
				return base.Width;
			}
			set { base.Width = value; }
		}

		public float TextWidth
		{
			get { return tw; }
		}
		
		public override float Height
		{
			get { return font.Size * 1.2f; }
			set {}
		}
		
		public override float ActualHeight
		{
			get { return Height; }
			set { base.ActualHeight = value; }
		}

		public string Text
		{
			get { return text; }
			set
			{
				text = value;
				MeasureString();
			}
		}
		
		public Font Font
		{
			get { return font; }
			set
			{
				font = value;
				MeasureString();
			}
		}
		
		public Brush Brush
		{
			get { return brush; }
			set { brush = value; }
		}
		
		public void DrawToGraphics (Graphics graphics)
		{
			if (graphics == null) return;
			RectangleF rect = new RectangleF(AbsoluteX, AbsoluteY, ActualWidth, ActualHeight);
			graphics.DrawString(Text, Font, Brush, rect, sf);
		}
		
		public void Dispose()
		{
			brush.Dispose();
			sf.Dispose();
		}
		
		public override float GetAbsoluteContentWidth()
		{
			return TextWidth + 20;
		}
		
		public override float GetAbsoluteContentHeight()
		{
			return Height;
		}
	}
}

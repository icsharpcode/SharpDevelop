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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.Reports.Addin.Designer
{
	/// <summary>
	/// Description of ctrlRuler.
	/// </summary>
	public class Ruler : System.Windows.Forms.UserControl
	{
		public enum RulerDirection 
		{
			Horizontal,
			Vertical
		}
		
		int leftMargin;
		int rightMargin;
		int startValue;
		int endValue;
		int bigStep ;
		int smallStep;
		bool drawFrame;
		
		Size paperSize;
		System.Drawing.GraphicsUnit scaleUnit;
		RulerDirection direction;
		
		System.Drawing.Color marginColor;
		
		public event EventHandler StartValueChange;
		public event EventHandler EndValueChange;
		
		public Ruler()
		{
			this.SetStyle(ControlStyles.DoubleBuffer |
			              ControlStyles.UserPaint |
			              ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.ResizeRedraw,
			              true);
			this.UpdateStyles();
			InitializeComponent();
			scaleUnit = GraphicsUnit.Millimeter;
		}
		
		#region privates 
		
		private void Line(Graphics g, int x1, int y1, int x2, int y2)
		{
			g.DrawLine(new Pen(new SolidBrush(this.ForeColor)), x1, y1, x2, y2);
		}
		
		private static void Line(Graphics g, Pen pen,int x1, int y1, int x2, int y2)
		{
			g.DrawLine(pen, x1, y1, x2, y2);
		}
		
		private static SizeF CalcReciprocals(Graphics g)	
		{
			switch(g.PageUnit)	
			{	
				case GraphicsUnit.World:	
				case GraphicsUnit.Pixel:
					
					return new SizeF(1f,1f);
					
				case GraphicsUnit.Inch:
					
					return new SizeF(1f/g.DpiX,1f/g.DpiY);
					
				case GraphicsUnit.Millimeter:
					
					return new SizeF(25.4f/g.DpiX,25.4f/g.DpiY);
					
				case GraphicsUnit.Point:
					
					return new SizeF(72f/g.DpiX,72f/g.DpiY);
					
				case GraphicsUnit.Display:
					
					return new SizeF(75f/g.DpiX,75f/g.DpiY);
					
				case GraphicsUnit.Document:
					
					return new SizeF(300f/g.DpiX,300f/g.DpiY);
					
			}
			return new SizeF(10,10);//never gets here...
		}
		
		
		
		private void SetSize (GraphicsUnit unit)
		{
			if (unit == GraphicsUnit.Millimeter) {
				bigStep = 10;
				smallStep = 5;
				paperSize = new Size (210,297);
			} else {
				throw new System.NotImplementedException ();
			}
		}
		
		
		void PaintVertical (Graphics g)
		{
			int bigStep = 0;
			Pen pen = new System.Drawing.Pen (System.Drawing.Color.Black,0.25f);
			Brush brush =  new SolidBrush(System.Drawing.Color.Black);
			
			switch (scaleUnit) {
				case GraphicsUnit.Millimeter:
					g.PageUnit = GraphicsUnit.Millimeter;
					g.PageScale = 1;
					SetSize (scaleUnit);
					bigStep = 10;
					smallStep = 5;
					break;
				case GraphicsUnit.Inch:
					break;
					
			}
			SizeF size = CalcReciprocals (g);
			int bLine,sLine;
			sLine = (int)(this.Width / 3 * size.Width);
			bLine = sLine * 2;
			
			int start = 0;
			if (startValue > 0) {
				start = (int)(startValue / 10);
			}

			endValue = paperSize.Height;
			
			int i = 0;
			int drawPos = 0;
			while (i < this.Height) {
				drawPos = i * bigStep;
				g.DrawString ((i + start).ToString(System.Globalization.CultureInfo.CurrentCulture),
				              this.Font,brush,3,drawPos);
				Line (g,pen,0,drawPos,bLine,drawPos);
				Line (g,pen,
				      0,drawPos - smallStep,
				      sLine,drawPos - smallStep);
				i ++;
			}
			pen.Dispose();
			brush.Dispose();
		}
		
		void PaintHorizontal (Graphics g)
		{
			Pen pen = new System.Drawing.Pen (System.Drawing.Color.Black,0.25f);
			Brush brush =  new SolidBrush(System.Drawing.Color.Black);
			
			switch (scaleUnit) {
				case GraphicsUnit.Millimeter:
					g.PageUnit = GraphicsUnit.Millimeter;
					g.PageScale = 1;
					SetSize (scaleUnit);
					break;
				case GraphicsUnit.Inch:
					break;
					
			}

			SizeF size = CalcReciprocals (g);
			int bLine,sLine;
			sLine = (int)(this.Height / 3 * size.Height);
			bLine = sLine * 2;
			
			int start = 0;
			if (startValue > 0) {
				start = (int)(startValue / 10);
			}

			endValue = paperSize.Width;

			if (drawFrame == true) {
				g.DrawRectangle (pen,0,0,(this.Width - 1) * size.Width,(this.Height - 1) * size.Width);
			}
			
			
			int i = 0;
			int drawPos = 0;
			
			while (i < (int)((endValue / 10) + 1)) {
				drawPos = i * bigStep;
				g.DrawString ((i + start).ToString(System.Globalization.CultureInfo.CurrentCulture),
				              this.Font,brush,drawPos,sLine);
				Line (g,pen,drawPos,0,i * bigStep,bLine);
				Line (g,pen,drawPos - smallStep,0,drawPos - smallStep,sLine);
				i ++;
			}
			// MarginMarker
			if (leftMargin > 0) {
				Line (g,
				      (int)(leftMargin),0,
				      (int)(leftMargin),bLine);
			}
			pen.Dispose();
			brush.Dispose();
		}
	
		#endregion
		
		#region overrides
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint (e);
			e.Graphics.Clear (this.BackColor);    
			if (this.direction == RulerDirection.Horizontal) {
				PaintHorizontal (e.Graphics);
			} else {
				PaintVertical (e.Graphics);
			}
		}
		
		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
		{
			pevent.Graphics.FillRectangle(Brushes.White,
			                         new Rectangle(0,
			                                       0,
			                                       Width,
			                                       Height));
			
		}
		
		
		protected override void OnResize(System.EventArgs e) 
		{
			base.OnResize (e);
			if (this.Width < this.Height) {
				direction = RulerDirection.Vertical;
			} else {
				direction = RulerDirection.Horizontal;
			}
			this.Invalidate();
		}
		
		#endregion
		
		#region Getter/Setter
		[Description("Left Margin")]
		[Category("Ruler")]
		public int LeftMargin {
			get {
				return leftMargin;
			}
			set {
				leftMargin = value;
				this.Invalidate();
			}
		}
		[Description("RightMargin")]
		[Category("Ruler")]
		public int RightMargin {
			get {
				return rightMargin;
			}
			set {
				rightMargin = value;
				this.Invalidate();
			}
		}
		[Description("A value from which the ruler marking should be shown.  Default is zero.")]
		[Category("Ruler")]
		public int StartValue {
			get {
				return startValue;
			}
			set {
				startValue = value;
				if (StartValueChange != null) {
					StartValueChange (this,new System.EventArgs());
				}
				this.Invalidate();
			}
		}
		[Description("A value to which the ruler marking should be shown.  Default is zero.")]
		[Category("Ruler")]
		public int EndValue {
			get {
				return endValue;
			}
			set {
				endValue = value;
				if (EndValueChange != null) {
					EndValueChange (this,new System.EventArgs());
				}
				this.Invalidate();
			}
		}
		[Description("The scale to use")]
		[Category("Ruler")]
		public System.Drawing.GraphicsUnit ScaleUnit {
			get {
				return scaleUnit;
			}
			set {
				scaleUnit = value;
				this.Invalidate();
			}
		}
		[Description("Horizontal or vertical layout")]
		[Category("Ruler")]
		public RulerDirection Direction {
			get {
				return direction;
			}
			set {
				direction = value;
				this.Invalidate();
			}
		}
		[Description("Frame around the Ruler")]
		[Category("Ruler")]
		public bool DrawFrame {
			get {
				return drawFrame;
			}
			set {
				drawFrame = value;
				this.Invalidate();
			}
		}
		[Description("Not used now Color with withch the left and right Margins are painted")]
		[Category("Ruler")]
		public System.Drawing.Color MarginColor {
			get {
				return marginColor;
			}
			set {
				marginColor = value;
				this.Invalidate();
			}
		}
		
		
		#endregion
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			// 
			// ctrlRuler
			// 
			this.DockPadding.All = 0;
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Name = "ctrlRuler";
			this.Size = new System.Drawing.Size(40, 32);
		}
		#endregion
	}
}

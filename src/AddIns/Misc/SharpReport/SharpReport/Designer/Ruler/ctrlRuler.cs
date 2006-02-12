/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 27.10.2004
 * Time: 14:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ruler
{
	/// <summary>
	/// Description of ctrlRuler.
	/// </summary>
	public class ctrlRuler : System.Windows.Forms.UserControl
	{
		public enum enmDirection {
			enmHorizontal,
			enmVertikal
		}
		
		int leftMargin;
		int rightMargin;
		int startValue;
		int endValue;
		int bigStep = 0;
		int smallStep = 0;
		bool drawFrame;
		
		Size paperSize;
		System.Drawing.GraphicsUnit scaleUnit;
		enmDirection direction;
		
		System.Drawing.Color marginColor;
		
		public event EventHandler StartValueChange;
		public event EventHandler EndValueChange;
		
		public ctrlRuler()
		{
			this.SetStyle(ControlStyles.DoubleBuffer |
			              ControlStyles.UserPaint |
			              ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.ResizeRedraw,
			              true);
			this.UpdateStyles();
			InitializeComponent();
			
//			this.ResizeRedraw = true;
			scaleUnit = GraphicsUnit.Millimeter;
		}
		
		#region privates 
		
		private void Line(Graphics g, int x1, int y1, int x2, int y2)
		{
			g.DrawLine(new Pen(new SolidBrush(this.ForeColor)), x1, y1, x2, y2);
		}
		
		private void Line(Graphics g, Pen pen,int x1, int y1, int x2, int y2)
		{
			g.DrawLine(pen, x1, y1, x2, y2);
		}
		
		private void SetSize (GraphicsUnit unit) {
			if (unit == GraphicsUnit.Millimeter) {
				bigStep = 10;
				smallStep = 5;
				paperSize = new Size (210,297);
			} else {
				throw new System.NotImplementedException ("SetStepSize");
			}
		}
		
		void PaintVertical (Graphics g){
			
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
			SizeF size = GdiHelper.CalcReciprocals (g);
			int bLine,sLine;
			sLine = (int)(this.Width / 3 * size.Width);
			bLine = sLine * 2;
			
			int start = 0;
			if (startValue > 0) {
				start = (int)(startValue / 10);
			}
			int ende =0;
			if (endValue > 0) {
				ende = paperSize.Height;
			} else {
				endValue = paperSize.Height;
			}

			
			int i = 0;
			int drawPos = 0;
			while (i < this.Height) {
				drawPos = i * bigStep;
				g.DrawString ((i + start).ToString(),this.Font,brush,3,drawPos);
				Line (g,pen,0,drawPos,bLine,drawPos);
				Line (g,pen,
				      0,drawPos - smallStep,
				      sLine,drawPos - smallStep);
				i ++;
			}
			pen.Dispose();
			brush.Dispose();
		}
		
		void PaintHorizontal (Graphics g){
			
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
			SizeF size = GdiHelper.CalcReciprocals (g);
			int bLine,sLine;
			sLine = (int)(this.Height / 3 * size.Height);
			bLine = sLine * 2;
			
			int start = 0;
			if (startValue > 0) {
				start = (int)(startValue / 10);
			}
			int ende =0;
			if (endValue > 0) {
				ende = paperSize.Width;
			} else {
				endValue = paperSize.Width;
			}
			if (drawFrame == true) {
				g.DrawRectangle (pen,0,0,(this.Width - 1) * size.Width,(this.Height - 1) * size.Width);
			}
			
			
			int i = 0;
			int drawPos = 0;
			
			while (i < (int)((endValue / 10) + 1)) {
				drawPos = i * bigStep;
				g.DrawString ((i + start).ToString(),this.Font,brush,drawPos,sLine);
				Line (g,pen,drawPos,0,i * bigStep,bLine);
				Line (g,pen,drawPos - smallStep,0,drawPos - smallStep,sLine);
				i ++;
			}
			// MarginMarker
			if (leftMargin > 0) {
//					g.DrawString ("L",this.Font,brush,leftMargin * size.Width,sLine);
//					Line (g,
//					      (int)(leftMargin * size.Width),0,
//					      (int)(leftMargin * size.Width),bLine);
				Line (g,
				      (int)(leftMargin),0,
				      (int)(leftMargin),bLine);
			}
			pen.Dispose();
			brush.Dispose();
		}
	
		#endregion
		
		#region overrides
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
			base.OnPaint (e);
			e.Graphics.Clear (this.BackColor);    
			if (this.direction == enmDirection.enmHorizontal) {
				PaintHorizontal (e.Graphics);
			} else {
				PaintVertical (e.Graphics);
			}
			
		}
		
		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
		{
			e.Graphics.FillRectangle(Brushes.White,
			                         new Rectangle(0,
			                                       0,
			                                       Width,
			                                       Height));
			
		}
		
		
		protected override void OnResize(System.EventArgs e) {
			base.OnResize (e);
			if (this.Width < this.Height) {
				direction = enmDirection.enmVertikal;
			} else {
				direction = enmDirection.enmHorizontal;
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
		public enmDirection Direction {
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

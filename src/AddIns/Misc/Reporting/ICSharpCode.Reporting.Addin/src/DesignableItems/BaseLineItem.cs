/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 23.03.2014
 * Time: 17:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using ICSharpCode.Reporting.Addin.Designer;
using ICSharpCode.Reporting.Addin.TypeProvider;

namespace ICSharpCode.Reporting.Addin.DesignableItems
{
	[Designer(typeof(LineDesigner))]
	public class BaseLineItem:AbstractItem
	{
		Point fromPoint;
		Point toPoint;
		LineCap startLineCap;
		LineCap endLineCap;
		DashCap dashLineCap;
		DashStyle dashStyle;
		float thickness;
		
		public BaseLineItem()
		{
			this.thickness = 1;
			this.dashStyle = DashStyle.Solid;
			this.Size = new Size(50,10);
			TypeDescriptor.AddProvider(new LineItemTypeProvider(), typeof(BaseLineItem));
			this.SetStartEndPoint();
		}
		
		void SetStartEndPoint ()
		{
			fromPoint = new Point(ClientRectangle.Left + 10,ClientRectangle.Height / 2);
			toPoint = new Point(ClientRectangle.Left + ClientRectangle.Width - 10,
			                         ClientRectangle.Height/ 2);
			Invalidate();
		}
		
		
		
//		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			base.OnPaint(e);
			Draw(e.Graphics);
		}
		
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			using (Pen p = new Pen(this.ForeColor,this.Thickness)) {
				p.SetLineCap(this.StartLineCap,this.EndLineCap,this.DashLineCap);
				graphics.DrawLine(p,this.fromPoint,this.toPoint);
			}
		}
		
		
		public Point FromPoint {
			get { return fromPoint; }
			set {
				Point x = value;
				if (!this.ClientRectangle.Contains(x)) {
					this.fromPoint = new Point(x.X - this.Location.X,
					                           x.Y - this.Location.Y);
				} else {
					this.fromPoint = x;
				}
				this.Invalidate();
			}
		}
		
		
		public Point ToPoint {
			get { return toPoint; }
			set {
				Point x = value;
				if (!ClientRectangle.Contains(x)) {
					this.toPoint = new Point(x.X - this.Location.X,
					                         x.Y - this.Location.Y);
				}
				else {
					toPoint = x;
				}
				Invalidate();
			}
		}
		
		
		
//		[Browsable(true),
//		 Category("Appearance"),
//		 Description("LineStyle")]
		public DashStyle DashStyle {
			get { return dashStyle; }
			set {
				dashStyle = value;
				this.Invalidate();
			}
		}
		
		
//		[Browsable(true),
//		 Category("Appearance"),
//		 Description("Thickness of Line")]
		public float Thickness {
			get { return thickness; }
			set {
				thickness = value;
				this.Invalidate();
			}
		}
		
//		[Browsable(true),
//		 Category("Appearance"),
//		 Description("LineCap at Startposition")]
		public LineCap StartLineCap {
			get { return startLineCap; }
			set {
				startLineCap = value;
				this.Invalidate();
			}
		}
		
//		[Browsable(true),
//		 Category("Appearance"),
//		 Description("Linecap at Endposition")]
		public LineCap EndLineCap {
			get { return endLineCap; }
			set {
				endLineCap = value;
				this.Invalidate();
			}
		}
		
//		[Browsable(true),
//		 Category("Appearance"),
//		 Description("Dashlinecap")]
		public DashCap DashLineCap {
			get { return dashLineCap; }
			set {
				dashLineCap = value;
				this.Invalidate();
			}
		}
		
	}
	
}

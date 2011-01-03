// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

using ICSharpCode.Reports.Addin.TypeProviders;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of BaseLine.
	/// </summary>
	[Designer(typeof(ICSharpCode.Reports.Addin.Designer.LineDesigner))]
	public class BaseLineItem:AbstractItem
	{
		private Point fromPoint;
		private Point toPoint;
		private LineCap startLineCap;
		private LineCap endLineCap;
		private DashCap dashLineCap;
		private DashStyle dashStyle;
		private float thickness;
		
		public BaseLineItem()
		{
			this.thickness = 1;
			this.dashStyle = DashStyle.Solid;
			this.Size = new Size(50,10);
			TypeDescriptor.AddProvider(new LineItemTypeProvider(), typeof(BaseLineItem));
			this.SetStartEndPoint();
		}
		
		private void SetStartEndPoint ()
		{
			this.fromPoint = new Point(ClientRectangle.Left + 10,ClientRectangle.Height / 2);
			this.toPoint = new Point(ClientRectangle.Left + ClientRectangle.Width - 10,
			                         ClientRectangle.Height/ 2);
			this.Invalidate();
		}
		
		
		
		[System.ComponentModel.EditorBrowsableAttribute()]
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
				if (!this.ClientRectangle.Contains(x)) {
					this.toPoint = new Point(x.X - this.Location.X,
					                         x.Y - this.Location.Y);
				}
				else {
					this.toPoint = x;
				}
				this.Invalidate();
			}
		}
		
		
		
		[Browsable(true),
		 Category("Appearance"),
		 Description("LineStyle")]
		public DashStyle DashStyle {
			get { return dashStyle; }
			set {
				dashStyle = value;
				this.Invalidate();
			}
		}
		
		
		[Browsable(true),
		 Category("Appearance"),
		 Description("Thickness of Line")]
		public float Thickness {
			get { return thickness; }
			set {
				thickness = value;
				this.Invalidate();
			}
		}
		
		[Browsable(true),
		 Category("Appearance"),
		 Description("LineCap at Startposition")]
		public LineCap StartLineCap {
			get { return startLineCap; }
			set {
				startLineCap = value;
				this.Invalidate();
			}
		}
		
		[Browsable(true),
		 Category("Appearance"),
		 Description("Linecap at Endposition")]
		public LineCap EndLineCap {
			get { return endLineCap; }
			set {
				endLineCap = value;
				this.Invalidate();
			}
		}
		
		[Browsable(true),
		 Category("Appearance"),
		 Description("Dashlinecap")]
		public DashCap DashLineCap {
			get { return dashLineCap; }
			set {
				dashLineCap = value;
				this.Invalidate();
			}
		}
		
	}
	
	
	
	
	
}

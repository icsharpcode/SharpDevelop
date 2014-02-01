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

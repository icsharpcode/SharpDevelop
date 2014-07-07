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
	public class BaseLineItem:AbstractGraphicItem
	{
		Point fromPoint;
		Point toPoint;
		LineCap startLineCap;
		LineCap endLineCap;
	
		
		public BaseLineItem()
		{
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
			using (var p = new Pen(ForeColor,Thickness)) {
				p.SetLineCap(StartLineCap,EndLineCap,DashCap.Flat);
				graphics.DrawLine(p,fromPoint,toPoint);
			}
		}
		
		
		public Point FromPoint {
			get { return fromPoint; }
			set {
				Point x = value;
				if (!ClientRectangle.Contains(x)) {
					fromPoint = new Point(x.X - Location.X,x.Y - Location.Y);
					                           
				} else {
					fromPoint = x;
				}
				Invalidate();
			}
		}
		
		
		public Point ToPoint {
			get { return toPoint; }
			set {
				Point x = value;
				if (!ClientRectangle.Contains(x)) {
					toPoint = new Point(x.X - Location.X,x.Y - Location.Y);
					                         
				}
				else {
					toPoint = x;
				}
				Invalidate();
			}
		}
		
		
		[ Category("Appearance")]
		public LineCap StartLineCap {
			get { return startLineCap; }
			set {
				startLineCap = value;
				Invalidate();
			}
		}
		
		
		[ Category("Appearance")]
		public LineCap EndLineCap {
			get { return endLineCap; }
			set {
				endLineCap = value;
				Invalidate();
			}
		}
	}
}

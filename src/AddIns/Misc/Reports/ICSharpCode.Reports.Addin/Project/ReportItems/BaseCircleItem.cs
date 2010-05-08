/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 31.01.2008
 * Zeit: 17:06
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of BaseCircleItem.
	/// </summary>
	
	[Designer(typeof(ICSharpCode.Reports.Addin.ShapeDesigner))]
	public class BaseCircleItem:AbstractItem
	{
		
		private DashStyle dashStyle;
		private float thickness;
	
		
		public BaseCircleItem()
		{
			this.thickness = 1;
			this.dashStyle = DashStyle.Solid;
			this.Size = new Size(GlobalValues.PreferedSize.Width,2* GlobalValues.PreferedSize.Height);
			TypeDescriptor.AddProvider(new RectangleItemTypeProvider(), typeof(BaseCircleItem));
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
			
			Rectangle rect = new Rectangle(this.ClientRectangle.Left,this.ClientRectangle.Top,
			                                this.ClientRectangle.Right -2,
			                                this.ClientRectangle.Bottom -2);
			

			using (Brush b = new SolidBrush(this.BackColor)) {
				graphics.FillEllipse(b,rect);
			 }
			
			using (Pen p = new Pen(this.ForeColor,this.thickness)) {
				graphics.DrawEllipse(p,rect);
			}
		}
		
		
		private BaseLine Baseline()
		{
			if (this.BackColor == GlobalValues.DefaultBackColor) {
				return new BaseLine (this.ForeColor,this.DashStyle,this.Thickness);
			} else {
				return new BaseLine (this.BackColor,this.DashStyle,this.Thickness);
			}
		}
		
		
		[Browsable(true),
		 Category("Appearance"),
		 Description("Linestyle")]
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
	}
}

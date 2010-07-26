/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 31.01.2008
 * Zeit: 15:58
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of BaseRectangleItem.
	/// </summary>
	
	[Designer(typeof(ICSharpCode.Reports.Addin.ShapeDesigner))]
	public class BaseRectangleItem:AbstractItem
	{
		private RectangleShape shape = new RectangleShape();
		private RectangleShape backgroundShape = new RectangleShape();
		
		private DashStyle dashStyle;
		private float thickness;
		
		
		public BaseRectangleItem()
		{
			this.thickness = 1;
			this.dashStyle = DashStyle.Solid;
			this.Size = new Size(GlobalValues.PreferedSize.Width,2* GlobalValues.PreferedSize.Height);
			TypeDescriptor.AddProvider(new RectangleItemTypeProvider(), typeof(BaseRectangleItem));
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
			                                this.ClientRectangle.Right -1,
			                                this.ClientRectangle.Bottom -1);
			backgroundShape.FillShape(graphics,
			                new SolidFillPattern(this.BackColor),
			                rect);
			Border b = new Border(new BaseLine (this.ForeColor,System.Drawing.Drawing2D.DashStyle.Solid,1));
			DrawFrame(graphics,b);
			
			shape.DrawShape (graphics,
			                 this.Baseline(),
			                 rect);
			
		}
		
		protected void DrawFrame (Graphics graphics,Border border) {
			if (this.DrawBorder == true) {
				border.DrawBorder(graphics,this.ClientRectangle);
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
			set { dashStyle = value; }
		}
		
		
		[Browsable(true),
		 Category("Appearance"),
		 Description("Thickness of Line")]
		
		public float Thickness {
			get { return thickness; }
			set { thickness = value; }
		}
		
		
	}
	
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

using ICSharpCode.Reports.Addin.TypeProviders;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of BaseRectangleItem.
	/// </summary>
	[Designer(typeof(ICSharpCode.Reports.Addin.Designer.ContainerItemDesigner))]
	public class BaseRectangleItem:AbstractItem
	{
		private RectangleShape shape = new RectangleShape();
		private RectangleShape backgroundShape = new RectangleShape();
		
		private DashStyle dashStyle;
		private float thickness;
		private int cornerRadius;
		
		public BaseRectangleItem()
		{
			this.thickness = 1;
			this.dashStyle = DashStyle.Solid;
			this.Size = new Size(GlobalValues.PreferedSize.Width,2* GlobalValues.PreferedSize.Height);
			cornerRadius = 1;
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
//			backgroundShape.FillShape(graphics,
//			                new SolidFillPattern(this.BackColor),
//			                rect);
			
			Border b = new Border(new BaseLine (this.ForeColor,System.Drawing.Drawing2D.DashStyle.Solid,1));
//			DrawFrame(graphics,b);
			BaseLine line = new BaseLine(base.ForeColor,DashStyle,Thickness,LineCap.Round,LineCap.Round,DashCap.Round);
			using (Pen pen = line.CreatePen(line.Thickness)){
				shape.CornerRadius = this.CornerRadius;
				GraphicsPath path1 = shape.CreatePath(rect);
				graphics.DrawPath(pen, path1);
				
			}
			
//			shape.DrawShape (graphics,
//			                 this.Baseline(),
//			                 rect);
			
		}
		
		
		[Browsable(true),
		 Category("Appearance"),
		 Description("Linestyle")]
		public DashStyle DashStyle {
			get { return dashStyle; }
			set { dashStyle = value;
				this.Invalidate();
			}
		}
		
		
		[Browsable(true),
		 Category("Appearance"),
		 Description("Thickness of Line")]
		public float Thickness {
			get { return thickness; }
			set { thickness = value;
				this.Invalidate();
			}
		}
		
		
		[Browsable(true),
		 Category("Appearance"),
		 Description("Radius of Corners")]
		public int CornerRadius
		{
			get{return this.cornerRadius;}
			set{this.cornerRadius = value;
				this.Invalidate();
			}
			
		}
		
	}
}

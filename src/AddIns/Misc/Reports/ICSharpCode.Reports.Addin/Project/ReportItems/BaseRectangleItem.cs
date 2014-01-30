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

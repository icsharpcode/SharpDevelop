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
using ICSharpCode.Reports.Core.Globals;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of BaseCircleItem.
	/// </summary>
	[Designer(typeof(ICSharpCode.Reports.Addin.Designer.ContainerItemDesigner))]
	//[Designer(typeof(ICSharpCode.Reports.Addin.Designer.ShapeDesigner))]
	public class BaseCircleItem:AbstractItem
	{
		
		private DashStyle dashStyle;
		private float thickness;
	
		
		public BaseCircleItem()
		{
			this.thickness = 1;
			this.dashStyle = DashStyle.Solid;
			this.Size = new Size(GlobalValues.PreferedSize.Width,2* GlobalValues.PreferedSize.Height);
			TypeDescriptor.AddProvider(new CircleItemTypeProvider(), typeof(BaseCircleItem));
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

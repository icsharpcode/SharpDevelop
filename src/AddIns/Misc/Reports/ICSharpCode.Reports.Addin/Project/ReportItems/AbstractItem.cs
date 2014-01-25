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

using ICSharpCode.Reports.Addin.TypeProviders;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of ReportItem.
	/// </summary>
	/// 
	[TypeDescriptionProvider(typeof(AbstractItemTypeProvider))]
	public abstract class AbstractItem:System.Windows.Forms.Control
	{
		private Color frameColor = Color.Black;
		

		protected AbstractItem()
		{
			InitializeComponent();
			TypeDescriptor.AddProvider(new AbstractItemTypeProvider(), typeof(AbstractItem));
//			VisibleInReport = true;
		}
		
		
		protected void DrawControl (Graphics graphics,Rectangle borderRectangle)
		{
			if (this.DrawBorder == true) {
				graphics.DrawRectangle(new Pen(this.frameColor),borderRectangle);
			} 
			System.Windows.Forms.ControlPaint.DrawBorder3D(graphics, this.ClientRectangle,
				                                               System.Windows.Forms.Border3DStyle.Etched);
		}
		
		
		#region Property's
		
		protected Rectangle DrawingRectangle {
			get {
				
				return new Rectangle(this.ClientRectangle.Left ,
				                            this.ClientRectangle.Top ,
				                            this.ClientRectangle.Width -1,
				                            this.ClientRectangle.Height -1);
			}
		}
		
		
		[Category("Border")]
		public Color FrameColor {
			get { return frameColor; }
			set {
				frameColor = value;
				this.Invalidate();
			}
		}
		
		
		[Category("Border"),
		Description("Draw a Border around the Item")]
		public bool DrawBorder {get;set;}
		
		protected new Size DefaultSize {get;set;}
			
//		public  bool VisibleInReport {get;set;}
		
		#endregion
		
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);
		}
		
		public abstract void Draw(Graphics graphics);
		
		private void InitializeComponent()
		{
			this.SuspendLayout();
			this.ResumeLayout(false);
		}
	}
	
	
	
}

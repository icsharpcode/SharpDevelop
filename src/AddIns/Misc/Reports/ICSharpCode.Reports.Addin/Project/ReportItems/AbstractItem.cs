// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

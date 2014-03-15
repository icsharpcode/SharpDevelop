/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 15.03.2014
 * Time: 19:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;

namespace ICSharpCode.Reporting.Addin.DesignableItems
{
	/// <summary>
	/// Description of AbstractItem.
	/// </summary>
//	[TypeDescriptionProvider(typeof(AbstractItemTypeProvider))]
	public abstract class AbstractItem:System.Windows.Forms.Control
	{
		private Color frameColor = Color.Black;
		

		protected AbstractItem()
		{
			InitializeComponent();
//			TypeDescriptor.AddProvider(new AbstractItemTypeProvider(), typeof(AbstractItem));
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

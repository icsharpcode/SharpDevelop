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
using ICSharpCode.Reporting.Addin.TypeProvider;

namespace ICSharpCode.Reporting.Addin.DesignableItems
{
	/// <summary>
	/// Description of AbstractItem.
	/// </summary>
//	[TypeDescriptionProvider(typeof(AbstractItemTypeProvider))]
	public abstract class AbstractItem:System.Windows.Forms.Control
	{
		Color frameColor = Color.Black;
		

		protected AbstractItem()
		{
			InitializeComponent();
			TypeDescriptor.AddProvider(new AbstractItemTypeProvider(), typeof(AbstractItem));
		}
		
		
		protected void DrawControl (Graphics graphics,Rectangle borderRectangle)
		{
			if (DrawBorder == true) {
				graphics.DrawRectangle(new Pen(frameColor),borderRectangle);
			} 
			System.Windows.Forms.ControlPaint.DrawBorder3D(graphics, this.ClientRectangle,
				                                               System.Windows.Forms.Border3DStyle.Etched);
		}
		
		
		#region Property's
		
		protected Rectangle DrawingRectangle {
			get {
				
				return new Rectangle(this.ClientRectangle.Left ,
				                            ClientRectangle.Top ,
				                            ClientRectangle.Width -1,
				                            ClientRectangle.Height -1);
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
			
		
		#endregion
		
		
		public abstract void Draw(Graphics graphics);
		
		private void InitializeComponent()
		{
			SuspendLayout();
			ResumeLayout(false);
		}
	}
}

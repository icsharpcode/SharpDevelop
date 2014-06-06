/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 15.03.2014
 * Time: 19:24
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
	/// Description of BaseSection.
	/// </summary>
	[TypeDescriptionProvider(typeof(SectionItemTypeProvider))]
	[Designer(typeof(ICSharpCode.Reporting.Addin.Designer.SectionDesigner))]
	public class BaseSection:AbstractItem
	{
		
		public BaseSection():base()
		{
			FrameColor = Color.Black;
			TypeDescriptor.AddProvider(new SectionItemTypeProvider(), typeof(BaseSection));
		}
		
		
//		[EditorBrowsableAttribute()]
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);
			Draw(e.Graphics);
		}
		
		
		public  override void Draw(Graphics graphics)
		{
			DrawControl(graphics, Rectangle.Inflate(this.ClientRectangle, -2, -2));
		}
	
		
		#region Propertys
//		
//		[Browsable(false)]
//		public int SectionOffset {get;set;}
		
//		[Browsable(false)]	
//		public int SectionMargin {get;set;}
			
//		public bool PageBreakAfter {get;set;}
			
		public bool CanGrow {get;set;}
			
		public bool CanShrink {get;set;}
			
		#endregion
	}
}

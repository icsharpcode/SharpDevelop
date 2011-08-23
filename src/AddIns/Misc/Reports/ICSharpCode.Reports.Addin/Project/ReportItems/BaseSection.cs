// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Drawing;

using ICSharpCode.Reports.Addin.TypeProviders;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of Section.
	/// </summary>
	/// 
	[TypeDescriptionProvider(typeof(SectionItemTypeProvider))]
	[Designer(typeof(ICSharpCode.Reports.Addin.Designer.SectionDesigner))]
	public class BaseSection:AbstractItem
	{

		public BaseSection():base()
		{
			base.FrameColor = Color.Black;
			TypeDescriptor.AddProvider(new SectionItemTypeProvider(), typeof(BaseSection));
		}
		
		
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);
			Draw(e.Graphics);
		}
		
		
		public  override void Draw(Graphics graphics)
		{
			base.DrawControl(graphics,Rectangle.Inflate(this.ClientRectangle,-2,-2));
		}
	
		
		#region Propertys
		
		[Browsable(false)]
		public int SectionOffset {get;set;}
		
		[Browsable(false)]	
		public int SectionMargin {get;set;}
			
		public bool PageBreakAfter {get;set;}
			
		public bool CanGrow {get;set;}
			
		public bool CanShrink {get;set;}
			
		#endregion
	}
	
	
}

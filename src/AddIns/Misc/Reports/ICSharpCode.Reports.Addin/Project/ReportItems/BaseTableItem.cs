// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Drawing;

using ICSharpCode.Reports.Addin.TypeProviders;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of BaseTableItem.
	/// </summary>
	[Designer(typeof(ICSharpCode.Reports.Addin.Designer.TableDesigner))]
	public class BaseTableItem:AbstractItem
	{
		
		
		public BaseTableItem():base()
		{
			Size s = new Size((GlobalValues.PreferedSize.Width * 3) + 10,
			                     GlobalValues.PreferedSize.Height * 2 + 10);
			base.DefaultSize = s;
			base.Size = s;
			base.BackColor = Color.White;
			TypeDescriptor.AddProvider(new TableItemTypeProvider(), typeof(BaseTableItem));
		}
		
		
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);
			this.Draw (e.Graphics);
		}
		
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			using (Brush b = new SolidBrush(this.BackColor)){
				graphics.FillRectangle(b, base.DrawingRectangle);
			}
			base.DrawControl(graphics,base.DrawingRectangle);
		}
		
	}
	
	
	
}

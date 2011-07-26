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
	/// Description of BaseRowItem.
	/// </summary>
	[Designer(typeof(ICSharpCode.Reports.Addin.Designer.ContainerItemDesigner))]
	public class BaseRowItem:AbstractItem
	{

		private Color alternateBackColor;
		private int changeBackColorEveryNRow;
		private RectangleShape backgroundShape = new RectangleShape();
		
		public BaseRowItem():base()
		{
			Size s = new Size((GlobalValues.PreferedSize.Width * 3) + 10,
			                     GlobalValues.PreferedSize.Height + 10);
			base.DefaultSize = s;
			base.Size = s;
			base.BackColor = Color.White;
			TypeDescriptor.AddProvider(new RowItemTypeProvider(), typeof(BaseRowItem));
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
		
		#region Properties
		
		public Color AlternateBackColor {
			get { return alternateBackColor; }
			set { alternateBackColor = value; }
		}
	
		
		public int ChangeBackColorEveryNRow {
			get { return changeBackColorEveryNRow; }
			set { changeBackColorEveryNRow = value; }
		}
		
		public RectangleShape BackgroundShape {
			get { return backgroundShape; }
			set { backgroundShape = value; }
		}
		
	
		#endregion
		
	}
}

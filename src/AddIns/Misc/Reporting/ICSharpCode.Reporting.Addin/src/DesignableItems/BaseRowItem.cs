/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 04.05.2014
 * Time: 17:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Addin.Designer;
using ICSharpCode.Reporting.Addin.TypeProvider;

namespace ICSharpCode.Reporting.Addin.DesignableItems
{
	/// <summary>
	/// Description of BaseRowItem.
	/// </summary>
	/// 
	[Designer(typeof(ContainerDesigner))]
	public class BaseRowItem:AbstractItem
	{

		public BaseRowItem():base()
		{
			var size = new Size((GlobalValues.PreferedSize.Width * 3) + 10,
			                     GlobalValues.PreferedSize.Height + 10);
			DefaultSize = size;
			Size = size;
			BackColor = Color.White;
			TypeDescriptor.AddProvider(new RowItemTypeProvider(), typeof(BaseRowItem));
		}
		
		
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);
			Draw(e.Graphics);
		}
		
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			using (Brush b = new SolidBrush(this.BackColor)){
				graphics.FillRectangle(b, DrawingRectangle);
			}
			DrawControl(graphics, base.DrawingRectangle);
		}
		
	}
}

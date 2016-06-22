/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 17.04.2014
 * Time: 19:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using ICSharpCode.Reporting.Addin.Designer;
using ICSharpCode.Reporting.Addin.TypeProvider;


namespace ICSharpCode.Reporting.Addin.DesignableItems
{
	/// <summary>
	/// Description of BaseCircleItem.
	/// </summary>
	/// 
	[Designer(typeof(ContainerDesigner))]
	class BaseCircleItem:AbstractGraphicItem
	{
		public BaseCircleItem()
		{
			TypeDescriptor.AddProvider(new CircleItemTypeProvider(), typeof(BaseCircleItem));
		}
		
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			
			var rect = new Rectangle(ClientRectangle.Left,
				ClientRectangle.Top,
				ClientRectangle.Right -1,
				ClientRectangle.Bottom -1);
			
			using (var pen = new Pen(ForeColor,Thickness)) {
				graphics.DrawEllipse(pen,rect);
			}
			
		}
	}
}

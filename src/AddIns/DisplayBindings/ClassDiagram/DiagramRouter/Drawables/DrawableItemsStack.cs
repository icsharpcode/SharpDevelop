/* Created by SharpDevelop.
 * User: itai
 * Date: 28/09/2006
 * Time: 19:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;

using System.Reflection;

using System.Drawing;
using System.Drawing.Drawing2D;


using System.Xml;
using Tools.Diagrams;

namespace Tools.Diagrams.Drawables
{
	public class DrawableItemsStack : DrawableItemsStack<IDrawableRectangle> {}
	
	public class DrawableItemsStack<T>
		: ItemsStack<T>, IDrawableRectangle
		where T : IDrawable, IRectangle
	{
		public void DrawToGraphics(Graphics graphics)
		{
			Recalculate();
			foreach (IDrawable d in this)
				d.DrawToGraphics(graphics);
		}
	}
}

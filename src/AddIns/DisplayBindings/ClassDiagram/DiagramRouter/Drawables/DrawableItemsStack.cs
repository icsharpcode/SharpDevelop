// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Itai Bar-Haim"/>
//     <version>$Revision$</version>
// </file>

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

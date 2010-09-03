// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

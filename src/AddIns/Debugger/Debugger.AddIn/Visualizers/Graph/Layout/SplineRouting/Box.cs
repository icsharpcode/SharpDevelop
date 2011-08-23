// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Debugger.AddIn.Visualizers.Graph.SplineRouting
{
	/// <summary>
	/// Simple implementation of IRect.
	/// </summary>
	public class Box : IRect, ICloneable
	{
		public Rectangle rect;
		Box inflatedCache;
		
		public Box(double left, double top, double width, double height)
		{
			this.rect = new Rectangle();
			this.Left = left;
			this.Top = top;
			this.Width = width;
			this.Height = height;
		}
		
		public Box(IRect original)
		{
			this.rect = new Rectangle();
			this.Left = original.Left;
			this.Top = original.Top;
			this.Width = original.Width;
			this.Height = original.Height;
		}
		
		public double Left { get { return Canvas.GetLeft(rect); } set {Canvas.SetLeft(rect, value); } }
		public double Top { get { return Canvas.GetTop(rect); } set {Canvas.SetTop(rect, value); } }
		public double Width { get { return rect.Width; } set { rect.Width = value; } }
		public double Height { get { return rect.Height; } set { rect.Height = value; } }
		
		public IRect Inflated(double padding)
		{
			//if (inflatedCache.ContainsKey(padding))
			if (inflatedCache == null)
			{
				inflatedCache = GeomUtils.InflateRect(this, padding);
			}
			return inflatedCache;
		}
		
		public object Clone()
		{
			return new Box(this.Left, this.Top, this.Width, this.Height);
		}
	}
}

// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
			if (inflatedCache == null) {
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

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
using System.Drawing;
using System.Drawing.Drawing2D;

using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.Printing.Shapes
{
	[TestFixture]
	public class LineShapeFixture
	{
		[Test]
		public void CreateLineShape()
		{
			LineShape ls = new LineShape();
			Assert.NotNull(ls);
		}
		
		[Test]
		public void CheckGraphicsPathBounds()
		{
			LineShape ls = new LineShape();
			Point from = new Point (1,1);
			Point to = new Point (10,10);
			GraphicsPath p = ls.CreatePath(from,to);
			RectangleF r = p.GetBounds();
			Assert.AreEqual(from.X,r.Location.X);
			Assert.AreEqual(from.Y,r.Location.Y);
			Assert.AreEqual(to.X - from.X, r.Width);
			Assert.AreEqual(to.Y - from.Y,r.Height);
		}
		
		
		[Test]
		public void CheckLastPointFromLine()
		{
			LineShape ls = new LineShape();
			Point from = new Point (1,1);
			Point to = new Point (10,10);
			GraphicsPath p = ls.CreatePath(from,to);
			PointF last = p.GetLastPoint();
			Assert.AreEqual(to,Point.Truncate(last));
		}
	}
}

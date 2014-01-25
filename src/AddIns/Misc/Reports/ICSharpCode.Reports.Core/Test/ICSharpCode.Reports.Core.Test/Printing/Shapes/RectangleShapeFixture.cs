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

using ICSharpCode.Reports.Core.Test.TestHelpers;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.Printing.Shapes
{
	[TestFixture]
	public class RectangleShapeFixture:ConcernOf<RectangleShape>
	{
		[Test]
		public void CreateLineShape()
		{
			RectangleShape ls = new RectangleShape();
			Assert.NotNull(ls);
		}
		
		
		[Test]
		[ExpectedException(typeof(NotImplementedException))]
		public void RectangleThrow()
		{
			Point from = new Point (1,1);
			Point to = new Point (10,10);
			GraphicsPath p = Sut.CreatePath(from,to);
		}
		
		
		[Test]
		public void CheckGraphicsPathBounds()
		{
			Point from = new Point(1,1);
			Size size = new Size (10,10);
			Rectangle rect = new Rectangle (from,size);
			GraphicsPath p = Sut.CreatePath(rect);
			RectangleF r = p.GetBounds();
			Assert.AreEqual(from.X,r.Left);
			Assert.AreEqual(from.Y,r.Top);
			Assert.AreEqual(r.Size.Width + from.X, r.Right);
			Assert.AreEqual(r.Size.Height + from.Y, r.Bottom);
		}
		
		
		[Test]
		public void CheckLastPointFromPath()
		{
			Point from = new Point(1,1);
			Size size = new Size (10,10);
			Rectangle rect = new Rectangle (from,size);
			GraphicsPath p = Sut.CreatePath(rect);
			PointF last = p.GetLastPoint();
			Assert.AreEqual(new Point (from.X ,from.Y + size.Height),  Point.Truncate(last));              
		}
		
		public override void Setup()
		{
			Sut = new RectangleShape();
		}
	}
}

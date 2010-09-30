// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

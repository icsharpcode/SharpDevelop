/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 03.10.2009
 * Zeit: 11:35
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

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

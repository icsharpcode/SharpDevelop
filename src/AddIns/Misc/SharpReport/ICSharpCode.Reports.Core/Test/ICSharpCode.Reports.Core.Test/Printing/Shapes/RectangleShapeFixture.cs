/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 03.10.2009
 * Zeit: 12:08
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
	public class RectangleShapeFixture
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
			
			RectangleShape ls = new RectangleShape();
			Point from = new Point (1,1);
			Point to = new Point (10,10);
			GraphicsPath p = ls.CreatePath(from,to);
		}
		
		
		[Test]
		public void CheckGraphicsPathBounds()
		{
			RectangleShape ls = new RectangleShape();
			Point from = new Point(1,1);
			Size size = new Size (10,10);
			Rectangle rect = new Rectangle (from,size);
			GraphicsPath p = ls.CreatePath(rect);
			RectangleF r = p.GetBounds();
			Assert.AreEqual(from.X,r.Left);
			Assert.AreEqual(from.Y,r.Top);
			Assert.AreEqual(r.Size.Width + from.X, r.Right);
			Assert.AreEqual(r.Size.Height + from.Y, r.Bottom);
		}
		
		
		[Test]
		public void CheckLastPointFromPath()
		{
			RectangleShape ls = new RectangleShape();
			Point from = new Point(1,1);
			Size size = new Size (10,10);
			Rectangle rect = new Rectangle (from,size);
			GraphicsPath p = ls.CreatePath(rect);
			PointF last = p.GetLastPoint();
			Assert.AreEqual(new Point (from.X ,from.Y + size.Height),  Point.Truncate(last));              
		}
	}
}

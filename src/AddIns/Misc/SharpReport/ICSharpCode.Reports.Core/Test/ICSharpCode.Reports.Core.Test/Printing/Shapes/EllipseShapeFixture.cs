/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 04.10.2009
 * Zeit: 11:36
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
	public class EllipseShapeFixture
	{
		[Test]
		public void CreateEllipseShape()
		{
			EllipseShape ls = new EllipseShape();
			Assert.NotNull(ls);
		}
		
		
		[Test]
		[ExpectedException(typeof(NotImplementedException))]
		public void EllipseThrow()
		{
			EllipseShape ls = new EllipseShape();
			Point from = new Point (1,1);
			Point to = new Point (10,10);
			GraphicsPath p = ls.CreatePath(from,to);
		}
		
		
		[Test]
		public void CheckGraphicsPathBounds()
		{
			EllipseShape ls = new EllipseShape();
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
	}
}

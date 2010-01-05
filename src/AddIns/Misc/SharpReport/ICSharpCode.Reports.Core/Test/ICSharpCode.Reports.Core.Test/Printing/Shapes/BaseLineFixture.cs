/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 04.10.2009
 * Zeit: 11:46
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
	public class BaseLineFixture
	{
		[Test]
		public void CreateBaseline_1()
		{
			BaseLine bl = new BaseLine(Color.Black,DashStyle.DashDot,2);
			Assert.NotNull(bl);
		}
		
		[Test]
		public void CreateBaseline_2()
		{
			BaseLine bl = new BaseLine(Color.Black,DashStyle.DashDot,1,LineCap.DiamondAnchor,LineCap.DiamondAnchor,DashCap.Flat);
			Assert.NotNull(bl);
		}
		
		[Test]
		public void CheckDefault_1()
		{
			BaseLine bl = new BaseLine(Color.Black,DashStyle.DashDot,2);
			Assert.AreEqual(Color.Black,bl.Color);
			Assert.AreEqual(DashStyle.DashDot,bl.DashStyle);
			Assert.AreEqual(2,bl.Thickness);
			Assert.AreEqual(LineCap.Flat,bl.StartLineCap);
			Assert.AreEqual(LineCap.Flat,bl.EndLineCap);
			Assert.AreEqual(DashCap.Flat,bl.DashLineCap);
		}
		
		[Test]
		public void CheckDefault_2()
		{
			BaseLine bl = new BaseLine(Color.Black,DashStyle.DashDot,2,LineCap.DiamondAnchor,LineCap.ArrowAnchor,DashCap.Round);
			Assert.AreEqual(Color.Black,bl.Color);
			Assert.AreEqual(DashStyle.DashDot,bl.DashStyle);
			Assert.AreEqual(2,bl.Thickness);
			Assert.AreEqual(LineCap.DiamondAnchor,bl.StartLineCap);
			Assert.AreEqual(LineCap.ArrowAnchor,bl.EndLineCap);
			Assert.AreEqual(DashCap.Round,bl.DashLineCap);
		}
		
		[Test]
		public void CreateCustomPen()
		{
			BaseLine bl = new BaseLine(Color.Red,DashStyle.DashDot,3,LineCap.DiamondAnchor,LineCap.ArrowAnchor,DashCap.Round);
			Pen pen = bl.CreatePen(3);
			Assert.AreEqual(3,pen.Width);
			Assert.AreEqual(DashStyle.DashDot,pen.DashStyle);
			Assert.AreEqual(Color.Red,pen.Color);
		}
		
		
		[Test]
		public void CreateCustomPenThicknessEqualsZero()
		{
			BaseLine bl = new BaseLine(Color.Red,DashStyle.DashDot,3,LineCap.DiamondAnchor,LineCap.ArrowAnchor,DashCap.Round);
			Pen pen = bl.CreatePen(0);
			Assert.AreEqual(1,pen.Width);
			Assert.AreEqual(DashStyle.DashDot,pen.DashStyle);
			Assert.AreEqual(Color.Red,pen.Color);
		}
		
		
		[Test]
		public void CreateDefaultPen()
		{
			BaseLine bl = new BaseLine(Color.Red,DashStyle.DashDot,2,LineCap.DiamondAnchor,LineCap.ArrowAnchor,DashCap.Round);
			Pen pen = bl.CreatePen();
			Assert.AreEqual(2,pen.Width);
			Assert.AreEqual(DashStyle.DashDot,pen.DashStyle);
			Assert.AreEqual(Color.Red,pen.Color);
		}
		
		[Test]
		public void CreatePenThicknessZeroReturns1()
		{
			BaseLine bl = new BaseLine(Color.Red,DashStyle.DashDot,0,LineCap.DiamondAnchor,LineCap.ArrowAnchor,DashCap.Round);
			Pen pen = bl.CreatePen();
			Assert.AreEqual(1,pen.Width);
		}
		
		
	}
}

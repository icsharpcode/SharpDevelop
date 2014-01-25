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

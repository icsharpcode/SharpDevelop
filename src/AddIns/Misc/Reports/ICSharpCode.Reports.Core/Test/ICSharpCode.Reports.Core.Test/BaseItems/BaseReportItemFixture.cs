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
using ICSharpCode.Reports.Core.Globals;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.BaseItems
{
	[TestFixture]
	public class BaseReportItemFixture
	{
		[Test]
		public void DefaultConstructor()
		{
			BaseReportItem bri = new BaseReportItem();
			Assert.IsNotNull(bri);
			Assert.AreEqual(System.Drawing.Color.Black,bri.ForeColor);
		}
		
		[Test]
		public void DefaultForeColor ()
		{
			BaseReportItem bri = new BaseReportItem();
			Assert.AreEqual(System.Drawing.Color.Black,bri.ForeColor,"Forecolor should be black");
		}
		
		
		[Test]
		public void DefaultBackColor ()
		{
			BaseReportItem bri = new BaseReportItem();
			Assert.AreEqual(GlobalValues.DefaultBackColor,bri.BackColor);
		}
		
		[Test]
		public void DrawBorder()
		{
			BaseReportItem bri = new BaseReportItem();
			Assert.IsFalse(bri.DrawBorder,"DrawBorder should BeforePrintEventArgs false");
		}
		
		
		[Test]
		public void DefaultFont ()
		{
			BaseReportItem bri = new BaseReportItem();
			Assert.AreEqual (GlobalValues.DefaultFont,bri.Font);
		}
		
		
	}
}

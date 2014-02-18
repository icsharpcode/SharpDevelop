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
using System.IO;
using System.Reflection;
using System.Drawing;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.PageBuilder
{
	[TestFixture]
	public class PageFixture
	{
		
		private IReportCreator reportCreator;
		
		[Test]
		public void CreateGraphicsFromPageSize () {
			reportCreator.BuildExportList();
			var page = reportCreator.Pages[0];
			Graphics g = CreateGraphics.FromSize(page.Size);
			Assert.That(g,Is.Not.Null);
		}
//	http://www.dev102.com/2008/10/09/measure-string-size-in-pixels-c/
		//http://www.codeproject.com/Articles/2118/Bypass-Graphics-MeasureString-limitations
		//http://codebetter.com/patricksmacchia/2009/08/31/reveal-hidden-api-usage-tricks-from-any-net-application/
		

		[Test]
		public void GraphicsIsSameSizeAsPage() {
			reportCreator.BuildExportList();
			var page = reportCreator.Pages[0];
			var graphics = CreateGraphics.FromSize(page.Size);
			Assert.That(graphics.VisibleClipBounds.Width,Is.EqualTo(page.Size.Width));
			Assert.That(graphics.VisibleClipBounds.Height,Is.EqualTo(page.Size.Height));
		}
		
		#region PageInfo
		
		[Test]
		public void PageInfoPageNumberIsOne() {
			reportCreator.BuildExportList();
			var pageInfo = reportCreator.Pages[0].PageInfo;
			Assert.That(pageInfo.PageNumber,Is.EqualTo(1));
		}

		
		[Test]
		public void PageInfoReportName() {
			reportCreator.BuildExportList();
			var pi = reportCreator.Pages[0].PageInfo;
			Assert.That(pi.ReportName,Is.EqualTo("Report1"));
		}
		
		
		#endregion
		
		[SetUp]
		public void LoadFromStream()
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			var stream = asm.GetManifestResourceStream(TestHelper.RepWithTwoItems);
			var reportingFactory = new ReportingFactory();
			reportCreator = reportingFactory.ReportCreator(stream);
		}
	}
}

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
using System.Reflection;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.PageBuilder;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.PageBuilder
{
	[TestFixture]
	public class BaseConvertFixture
	{
		private IReportCreator reportCreator;
		
		
		[Test]
		public void CurrentPageContainFiveItems() {
			reportCreator.BuildExportList();
			var page = reportCreator.Pages[0];
			Assert.That(page.ExportedItems.Count, Is.EqualTo(5));
		}
		
		
		[Test]
		public void PageItemIsBaseExportContainer() {
			reportCreator.BuildExportList();
			var page = reportCreator.Pages[0];
			Assert.That(page.ExportedItems[0],Is.InstanceOf(typeof(ExportContainer)));
		}
		
		
		[Test]
		public void ExportContainerContainsExportText() {
			reportCreator.BuildExportList();
			var page = reportCreator.Pages[0];
			var firstItem = (ExportContainer)page.ExportedItems[0];
			var result = firstItem.ExportedItems[0];
			Assert.That(result,Is.InstanceOf(typeof(ExportText)));
		}
		
		
		[SetUp]
		public void LoadFromStream()
		{
			System.Reflection.Assembly asm = Assembly.GetExecutingAssembly();
			var stream = asm.GetManifestResourceStream(TestHelper.RepWithTwoItems);
			var reportingFactory = new ReportingFactory();
			reportCreator = reportingFactory.ReportCreator(stream);
		}
	}
}

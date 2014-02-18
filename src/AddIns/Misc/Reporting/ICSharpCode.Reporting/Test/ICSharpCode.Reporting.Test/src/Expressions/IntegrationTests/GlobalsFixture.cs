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

using System.Collections.ObjectModel;
using System.Reflection;

using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.Expressions.InterationTests
{
	[TestFixture]
//	[Ignore]
	public class GlobalsFixture
	{
		IReportCreator reportCreator;
		Collection<ExportText> collection;
		
		[Test]
		public void CanReadPageNumber()
		{
			reportCreator.BuildExportList();
			var page = reportCreator.Pages[0];
			var sec = (ExportContainer)page.ExportedItems[3];
			var s = (ExportText)sec.ExportedItems[1];
			Assert.That (s.Text.Contains("1"));
			Assert.That(s.Text.Contains("Page"));
		}
		
		
		[Test]
		public void CanEvaluateTotalPages () {
			reportCreator.BuildExportList();
			var page = reportCreator.Pages[0];
			var sec = (ExportContainer)page.ExportedItems[1];
			var s = (ExportText)sec.ExportedItems[0];
			Assert.That (s.Text.Contains("1"));
			Assert.That(s.Text.Contains("Pages"));
		}
		
		
		[Test]
		public void CanEvaluateReportName() {
			reportCreator.BuildExportList();
			var page = reportCreator.Pages[0];
			var sec = (ExportContainer)page.ExportedItems[2];
			var s = (ExportText)sec.ExportedItems[0];
			Assert.That (s.Text.Contains("Report1"));
			Assert.That(s.Text.Contains("ReportName"));
		}
		

		[Test]
		public void CanEvaluateReportFolder() {
			reportCreator.BuildExportList();
			var page = reportCreator.Pages[0];
			var sec = (ExportContainer)page.ExportedItems[0];
			var s = (ExportText)sec.ExportedItems[1];
			Assert.That (s.Text.Contains(@"\UnitTests"));
			Assert.That(s.Text.Contains("ReportFolder"));
		}
		
		
		[Test]
		public void CanEvaluateReportFileName () {
			reportCreator.BuildExportList();
			var page = reportCreator.Pages[0];
			var sec = (ExportContainer)page.ExportedItems[0];
			var s = (ExportText)sec.ExportedItems[0];
			Assert.That (s.Text.Contains(@"\UnitTests\Report1.srd"));
			Assert.That(s.Text.Contains("ReportFileName"));
		}
		
		
		[Test]
		public void SyntaxErrorInGlobals () {
			reportCreator.BuildExportList();
			var page = reportCreator.Pages[0];
			var sec = (ExportContainer)page.ExportedItems[3];
			var s = (ExportText)sec.ExportedItems[0];
			Assert.That (s.Text.Contains("Syntaxerror in Globals"));
			Assert.That(s.Text.Contains("SyntaxError"));
		}
			
		
		[SetUp]
		public void CreateExportlist() {
		collection = new Collection<ExportText>();
			collection.Add(new ExportText()
			       {
			       	 Text = "myExporttextColumn"
			       });
		}
			
		
		[SetUp]
		public void LoadModelFromStream()
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			var stream = asm.GetManifestResourceStream(TestHelper.TestForGlobals);
			var rf = new ReportingFactory();
			var reportingFactory = new ReportingFactory();
			reportCreator = reportingFactory.ReportCreator(stream);
		}
	}
}

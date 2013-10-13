// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
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

	public class GlobalsFixture
	{
		IReportCreator reportCreator;
		Collection<ExportText> collection;
		ExpressionVisitor expressionVisitor;
		
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
		
		
		[TestFixtureSetUp]
		public void Setup() {
			
			expressionVisitor = new ExpressionVisitor(new ReportSettings());
		}
	}
}

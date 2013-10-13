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
	[Ignore]
	public class GlobalsFixture
	{
		IReportCreator reportCreator;
		Collection<ExportText> collection;
		ExpressionVisitor expressionVisitor;
		
		[Test]
		public void TestMethod()
		{
			reportCreator.BuildExportList();
			/*
			var script ="=Globals!PageNumber";
			collection[0].Text = script;
			var visitor = new ExpressionVisitor(new ReportSettings());
			var exportContainer = new ExportContainer();
			exportContainer.ExportedItems.Add(collection[0]);
			visitor.Visit(exportContainer);	
			Assert.That (collection[0].Text,Is.EqualTo("Sharpdevelop is great"));
			*/
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
			var stream = asm.GetManifestResourceStream(TestHelper.RepWithTwoItems);
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

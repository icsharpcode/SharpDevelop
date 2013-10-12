// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.Expressions.InterationTests
{
	[TestFixture]
	public class FieldsFixture
	{
		Collection<ExportText> collection;
		ExpressionVisitor expressionVisitor;
		
		
		[Test]
		public void FieldsInContainer() {
			var script = "=Fields!myfield1 + Fields!myfield2";
			collection[0].Text = script;
			collection.Add(new ExportText() 
			               {
			               	Text = "Sharpdevelop",
			               	Name = "myfield1"
			               });
			collection.Add(new ExportText() 
			               {
			               	Text = " is great",
			               	Name = "myfield2"
			               });               
			var visitor = new ExpressionVisitor(new ReportSettings());
			var exportContainer = new ExportContainer();
			exportContainer.ExportedItems.Add(collection[0]);
			exportContainer.ExportedItems.Add(collection[1]);
			exportContainer.ExportedItems.Add(collection[2]);
			visitor.Visit(exportContainer);	
			Assert.That (collection[0].Text,Is.EqualTo("Sharpdevelop is great"));
		}
		
		
		[Test]
		public void FieldNotExist() {
			var script = "=Fields!myfieldNotExist";
			collection[0].Text = script;
			collection.Add(new ExportText() 
			               {
			               	Text = "Sharpdevelop",
			               	Name = "myfield1"
			               });
			var visitor = new ExpressionVisitor(new ReportSettings());
			var exportContainer = new ExportContainer();
			exportContainer.ExportedItems.Add(collection[0]);
			exportContainer.ExportedItems.Add(collection[1]);
			visitor.Visit(exportContainer);	
			Assert.That (collection[0].Text.StartsWith("Missing"));
			Assert.That (collection[0].Text.Contains("myfieldNotExist"));
		}
		
		
		[SetUp]
		public void CreateExportlist() {
		collection = new Collection<ExportText>();
			collection.Add(new ExportText()
			       {
			       	 Text = "myExporttextColumn"
			       });
		}
		
		[TestFixtureSetUp]
		public void Setup() {
			expressionVisitor = new ExpressionVisitor(new ReportSettings());
		}
		
		
	}
}

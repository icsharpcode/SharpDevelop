// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.Expressions.InterationTests {
	
	[TestFixture]
	public class StandardTests
	{
		Collection<ExportText> collection;
		ExpressionVisitor expressionVisitor;
		
		[Test]
		public void ExpressionMustStartWithEqualChar()
		{
			collection[0].Text = "myText";
			var result = collection[0].Text;
			expressionVisitor.Visit(collection[0]);
			Assert.That(result,Is.EqualTo(collection[0].Text));
		}
		
		
		[Test]
		
		public void ReportSyntaxError() {
			collection[0].Text = "= myText";
			expressionVisitor.Visit(collection[0]);
		}
		
		
		
		[Test]
		public void SimpleStringHandling () {
			var script = "='Sharpdevelop' + ' is great'";
			collection[0].Text = script;
			expressionVisitor.Visit(collection[0]);
			Assert.That(collection[0].Text,Is.EqualTo("Sharpdevelop is great"));
		}
		
		
		#region Convert inside Container
		[Test]
		public void SimpleStringHandlingInContainer () {
			var script = "='Sharpdevelop' + ' is great'";
			
			collection[0].Text = script;
			var exportContainer = new ExportContainer();
			exportContainer.ExportedItems.Add(collection[0]);
			expressionVisitor.Visit(exportContainer);
			
			var resultColumn = (ExportText)exportContainer.ExportedItems[0];
			Assert.That(resultColumn.Text,Is.EqualTo("Sharpdevelop is great"));
		}
		
		#endregion
		
		
		#region System.Math
		
		[Test]
		public void CanRunSystemMath () {
			//Using methods imported from System.Math class
			var script = @"=abs(-1.0) + Log10(100.0) + sqrt(9) + floor(4.5) + sin(PI/2)";
			collection[0].Text = script;
			expressionVisitor.Visit(collection[0]);
			var res = Convert.ToDouble(collection[0].Text);
			Assert.That(collection[0].Text,Is.EqualTo("11"));		
		}
		#endregion
		
		
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

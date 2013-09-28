// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.Expressions
{
	[TestFixture]
	public class IntegrationTests
	{
		Collection<ExportText> collection;
		ExpressionVisitor expressionVisitor;
		
		[Test]
		public void ExpressionMustStartWithEqualChar()
		{
			var result = collection[0];
			expressionVisitor.Visit(collection[0]);
			Assert.That(result.Text,Is.EqualTo(collection[0].Text));
		}
		
		
		[Test]
		public void SimpleMath() {
			expressionVisitor.Visit(collection[1]);
			
			Assert.That(collection[1].Text,Is.EqualTo("8"));		
			var res = Convert.ToInt32(collection[1].Text);
			Assert.That(res is int);
		}
		
		
		[Test]
		public void SimpleStringHandling () {
		var script = "='Sharpdevelop' + ' is great'";		
			collection.Add(new ExportText()
			               {
			               	Text = script
			               });
			expressionVisitor.Visit(collection[2]);
			Assert.That(collection[2].Text,Is.EqualTo("Sharpdevelop is great"));	
		}
		
		#region System.Environment
		
		[Test]
		[Ignore]
		public void CanUserSystemEnvironment() {
			/*
			//Using methods imported from System.Environment
			var script = @"report = '#{MachineName}-#{OSVersion}-#{UserName}'";
			var result = evaluator.Evaluate(script);
			var expected = string.Format("{0}-{1}-{2}", Environment.MachineName, Environment.OSVersion, Environment.UserName);
			Assert.AreEqual(expected, result, "Unexpected computation result");
			*/
		}
		
		#endregion
		
		
		#region System.Math
		
		[Test]
		public void CanRunSystemMath () {
			//Using methods imported from System.Math class
			var script = @"=abs(-1.0) + Log10(100.0) + sqrt(9) + floor(4.5) + sin(PI/2)";
			collection[1].Text = script;
			expressionVisitor.Visit(collection[1]);
			var res = Convert.ToDouble(collection[1].Text);
			Assert.That(collection[1].Text,Is.EqualTo("11"));		
		}
		
		#endregion
		
		[SetUp]
		public void CreateExportlist() {
		collection = new Collection<ExportText>();
			collection.Add(new ExportText()
			       {
			       	 Text = "myExporttextColumn"
			       });
			collection.Add(new ExportText()
			       {
			       	Text ="= 3 + 5"
			
			       });
		
		}
			
		[TestFixtureSetUp]
		public void Setup() {
			expressionVisitor = new ExpressionVisitor();
		}
		 
	}
}

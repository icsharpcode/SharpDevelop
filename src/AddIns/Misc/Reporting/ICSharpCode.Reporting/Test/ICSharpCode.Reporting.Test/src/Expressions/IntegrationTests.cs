// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Items;
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
		public void SimpleMath() {
			collection[0].Text = "=3 + 6";
			expressionVisitor.Visit(collection[0]);
			Assert.That(collection[0].Text,Is.EqualTo("9"));		
			var res = Convert.ToInt32(collection[0].Text);
			Assert.That(res is int);
		}
		
		
		[Test]
		public void SimpleStringHandling () {
			var script = "='Sharpdevelop' + ' is great'";
			collection[0].Text = script;
			expressionVisitor.Visit(collection[0]);
			Assert.That(collection[0].Text,Is.EqualTo("Sharpdevelop is great"));
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
			collection[0].Text = script;
			expressionVisitor.Visit(collection[0]);
			var res = Convert.ToDouble(collection[0].Text);
			Assert.That(collection[0].Text,Is.EqualTo("11"));		
		}
		#endregion
		
		
		#region Parameters
		
		[Test]
		public void CanConcatParameter () {
			
			var parameters = new ParameterCollection();
			parameters.Add(new BasicParameter() {
			               	ParameterName = "param1",
			               	ParameterValue = "SharpDevelop"
			               }
			              );
			
			parameters.Add(new BasicParameter() {
			               	ParameterName = "param2",
			               	ParameterValue = " is "
			               }
			              );
			parameters.Add(new BasicParameter() {
			               	ParameterName = "param3",
			               	ParameterValue = "great"
			               }
			              );
			
			var reportSettings = CreateReportSettings(parameters);
			var visitor = new ExpressionVisitor(reportSettings);
			
			var script = "=Parameters!param1 + Parameters!param2 + Parameters!param3";
			collection[0].Text = script;
			visitor.Visit(collection[0]);
			Assert.That (collection[0].Text,Is.EqualTo("SharpDevelop is great"));
		}
		
		
		#endregion
		
		
		ReportSettings CreateReportSettings(ParameterCollection parameters)
		{
			var reportSettings = new ReportSettings();
			reportSettings.ParameterCollection.AddRange(parameters);		
			return reportSettings;
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

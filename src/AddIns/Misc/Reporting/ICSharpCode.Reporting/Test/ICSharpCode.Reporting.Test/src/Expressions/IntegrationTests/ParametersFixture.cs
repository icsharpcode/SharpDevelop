// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.Expressions.InterationTests
{
	[TestFixture]
	public class ParametersFixture
	{
		Collection<ExportText> collection;
		
		
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
			var visitor = new ExpressionVisitor(reportSettings,null);
			
			var script = "=Parameters!param1 + Parameters!param2 + Parameters!param3";
			collection[0].Text = script;
			visitor.Visit(collection[0]);
			Assert.That (collection[0].Text,Is.EqualTo("SharpDevelop is great"));
		}
		
		
		[Test]
		public void ParameterNotExist() {
			var parameters = new ParameterCollection();
			parameters.Add(new BasicParameter() {
			               	ParameterName = "param1",
			               	ParameterValue = "SharpDevelop"
			               }
			              );
			var reportSettings = CreateReportSettings(parameters);
			var visitor = new ExpressionVisitor(reportSettings,null);
			
			var script = "=Parameters!paramNotExist";
			collection[0].Text = script;
			visitor.Visit(collection[0]);
			Assert.That (collection[0].Text.StartsWith("Missing"));
			Assert.That (collection[0].Text.Contains("paramNotExist"));
		}
		
		
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
			
	}
}

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
			var visitor = new ExpressionVisitor(reportSettings);
			
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
			var visitor = new ExpressionVisitor(reportSettings);
			
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
